using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using eCommerceAutomation.Service.Models.API.ViewModel;

namespace eCommerceAutomation.Service.Services
{
    public class ProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        public async Task<IEnumerable<Product>> GetAsync(bool? IsReviewNeeded, bool? IsDisabled, bool? IsInitialized, bool? IsSourcesDisabled, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.GetAsync("Product");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Product GET service thrown an error.");
            }

            var content = await response.Content.ReadAsStreamAsync();
            if (content.Length == 0)
                return new List<Product>();

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            return await JsonSerializer.DeserializeAsync<IEnumerable<Product>>(content, serializeOptions);
        }
    }
}
