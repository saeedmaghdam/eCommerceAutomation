using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DashFire;
using eCommerceAutomation.Service.Framework;
using eCommerceAutomation.Service.Framework.Rule.Base;
using eCommerceAutomation.Service.Models;
using eCommerceAutomation.Service.Rules;
using eCommerceAutomation.Service.Services;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace eCommerceAutomation.Service
{
    public class ProductUpdaterJob : Job
    {
        public override JobInformation JobInformation => JobInformationBuilder.CreateInstance()
            .RegistrationRequired()
            .SetCronSchedules(new[] { "0 */3 * * *" })
            .SetDescription("Fetch sources, analyze them and update product!")
            .SetDisplayName("Product Updater Job")
            .SetSystemName(nameof(ProductUpdaterJob))
            .Build();

        private readonly ILogger<ProductUpdaterJob> _logger;
        private readonly ProductService _productService;
        private readonly IOptions<DashOptions> _options;
        private readonly IOptions<ApplicationOptions> _applicationOptions;
        private readonly IDistributedCache _cache;

        private IEnumerable<Models.API.ViewModel.Product> _products;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private Dictionary<string, IWebsiteRuleFactory> _websiteRuleFactories;
        private Dictionary<string, ITelegramRuleFactory> _telegramRuleFactories;

        public ProductUpdaterJob(ILogger<ProductUpdaterJob> logger, ProductService productService, IOptions<DashOptions> options, IOptions<ApplicationOptions> applicationOptions, IDistributedCache cache, RuleProviderFactory ruleProviderFactory)
        {
            _logger = logger;
            _productService = productService;
            _options = options;
            _applicationOptions = applicationOptions;
            _cache = cache;

            var factory = new ConnectionFactory() { Uri = new Uri(_options.Value.RabbitMqConnectionString) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            InitializeTabbitMq();

            _websiteRuleFactories = ruleProviderFactory.GetProviders().Where(x => x.Key == "Website").Select(x => ((IWebsiteRuleFactoryProvider)x).GetFactories()).SelectMany(x => x).ToDictionary(x => x.Key, x => x);
            _telegramRuleFactories = ruleProviderFactory.GetProviders().Where(x => x.Key == "Telegram").Select(x => ((ITelegramRuleFactoryProvider)x).GetFactories()).SelectMany(x => x).ToDictionary(x => x.Key, x => x);
        }

        protected override async Task StartInternallyAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                await ConsumerReceivedAsync(model, ea, cancellationToken);
            };
            _channel.BasicConsume(_applicationOptions.Value.ResponseQueueName, false, consumer);

            _products = await _productService.GetAsync(null, false, null, false, cancellationToken);
            if (!_products.Any())
                return;

            foreach (var product in _products)
            {
                foreach (var source in product.Sources)
                {
                    if (source.SourceType == Framework.Constants.SourceType.Website)
                    {
                        var url = JsonSerializer.Deserialize<string[]>(source.Address)[0];

                        var request = new Models.Scrapper.Request()
                        {
                            Address = url,
                            RequestId = Guid.NewGuid().ToString(),
                            Type = Framework.Constants.RequestType.Website
                        };
                        PublishRequest(request);

                        await CacheRequestAsync(request.RequestId, product.Id, source.Id, cancellationToken);
                    }
                }
            }
        }

        private void PublishRequest(Models.Scrapper.Request request)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = false;
            properties.Headers = new Dictionary<string, object>()
                        {
                            {
                                "type", "request"
                            }
                        };
            var messageBodyBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
            _channel.BasicPublish(_applicationOptions.Value.ExchangeName, "", properties, messageBodyBytes);
        }

        private async Task CacheRequestAsync(string requestId, long productId, long sourceId, CancellationToken cancellationToken)
        {
            var cacheKey = $"{productId}_{sourceId}";

            var cacheModel = new RequestCacheModel()
            {
                RequestId = requestId,
                ProductId = productId,
                SourceId = sourceId
            };

            var currentRequestData = await _cache.GetAsync(cacheKey, cancellationToken);
            if (currentRequestData == null)
            {
                var data = MessagePackSerializer.Serialize(cacheModel);
                await _cache.SetAsync(cacheKey, data, cancellationToken);
            }

            currentRequestData = await _cache.GetAsync(requestId, cancellationToken);
            if (currentRequestData == null)
            {
                var data = MessagePackSerializer.Serialize(cacheModel);
                await _cache.SetAsync(requestId, data, cancellationToken);
            }

            var currentRequestsData = await _cache.GetAsync("requests", cancellationToken);
            var currentRequests = default(Dictionary<string, RequestCacheModel>);
            bool isCacheRequestsChanged = false;
            if (currentRequestsData == null)
            {
                currentRequests = new Dictionary<string, RequestCacheModel>();
                currentRequests.Add(cacheKey, cacheModel);
                isCacheRequestsChanged = true;
            }
            else
            {
                currentRequests = await MessagePackSerializer.DeserializeAsync<Dictionary<string, RequestCacheModel>>(new MemoryStream(currentRequestsData), cancellationToken: cancellationToken);
                if (!currentRequests.ContainsKey(cacheKey))
                {
                    currentRequests.Add(cacheKey, cacheModel);
                    isCacheRequestsChanged = true;
                }
            }

            if (!isCacheRequestsChanged)
                return;

            currentRequestsData = MessagePackSerializer.Serialize(currentRequests);
            await _cache.SetAsync("requests", currentRequestsData, cancellationToken);
        }

        private async Task ConsumerReceivedAsync(object model, BasicDeliverEventArgs ea, CancellationToken cancellationToken)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var response = JsonSerializer.Deserialize<Models.Scrapper.Response>(message);
            await ProcessResponseAsync(response, ea, cancellationToken);
        }

        private async Task ProcessResponseAsync(Models.Scrapper.Response response, BasicDeliverEventArgs ea, CancellationToken cancellationToken)
        {
            _logger.LogInformation(response.Content);

            var requestCacheModelData = await _cache.GetAsync(response.RequestId, cancellationToken);
            if (requestCacheModelData != null && requestCacheModelData.Length != 0)
            {
                var requestCacheModel = await MessagePackSerializer.DeserializeAsync<RequestCacheModel>(new MemoryStream(requestCacheModelData), cancellationToken: cancellationToken);

                var product = _products.SingleOrDefault(x => x.Id == requestCacheModel.ProductId);
                if (product == null)
                    throw new Exception("Product not found!");

                var source = product.Sources.SingleOrDefault(x => x.Id == requestCacheModel.SourceId);
                if (source == null)
                    throw new Exception("Source not found!");

                // Fetch new metadata using the rules, compare to old model and do the best action!
            }

            await RemoveCacheRequestAsync(response.RequestId, cancellationToken);
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }

        private async Task RemoveCacheRequestAsync(string requestId, CancellationToken cancellationToken)
        {
            var currentRequestData = await _cache.GetAsync(requestId, cancellationToken);
            var currentRequest = default(RequestCacheModel);
            var cacheKey = default(string);
            if (currentRequestData != null)
            {
                currentRequest = await MessagePackSerializer.DeserializeAsync<RequestCacheModel>(new MemoryStream(currentRequestData), cancellationToken: cancellationToken);
                cacheKey = $"{currentRequest.ProductId}_{currentRequest.SourceId}";

                await _cache.RemoveAsync(cacheKey, cancellationToken);

                await _cache.RemoveAsync(requestId, cancellationToken);
            }

            if (currentRequest != null)
            {
                var currentRequestsData = await _cache.GetAsync("requests", cancellationToken);
                var currentRequests = default(Dictionary<string, RequestCacheModel>);
                bool isCacheRequestsChanged = false;
                if (currentRequestsData != null)
                {
                    currentRequests = await MessagePackSerializer.DeserializeAsync<Dictionary<string, RequestCacheModel>>(new MemoryStream(currentRequestsData), cancellationToken: cancellationToken);
                    if (currentRequests.ContainsKey(cacheKey))
                    {
                        currentRequests.Remove(cacheKey);
                        isCacheRequestsChanged = true;
                    }
                }

                if (!isCacheRequestsChanged)
                    return;

                currentRequestsData = MessagePackSerializer.Serialize(currentRequests);
                await _cache.SetAsync("requests", currentRequestsData, cancellationToken);
            }
        }

        private void InitializeTabbitMq()
        {
            _channel.ExchangeDeclare(_applicationOptions.Value.ExchangeName, "headers", true);

            // Request
            _channel.QueueDeclare(queue: _applicationOptions.Value.RequestQueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.QueueBind(_applicationOptions.Value.RequestQueueName, _applicationOptions.Value.ExchangeName, string.Empty, new Dictionary<string, object>()
            {
                {
                    "type", "request"
                }
            });

            // Response
            _channel.QueueDeclare(queue: _applicationOptions.Value.ResponseQueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            _channel.QueueBind(_applicationOptions.Value.ResponseQueueName, _applicationOptions.Value.ExchangeName, string.Empty, new Dictionary<string, object>()
            {
                {
                    "type", "response"
                }
            });
        }
    }
}
