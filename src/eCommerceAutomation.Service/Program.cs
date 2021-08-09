using DashFire;
using eCommerceAutomation.Service.Framework;
using eCommerceAutomation.Service.Rules;
using eCommerceAutomation.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eCommerceAutomation.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<ApplicationOptions>(options => hostContext.Configuration.GetSection("ApplicationOptions").Bind(options));

                    services.AddHttpClient("api", client =>
                    {
                        client.BaseAddress = new System.Uri("http://localhost:1356/api/v1/​");
                    });

                    services.AddDistributedRedisCache(option =>
                    {
                        option.Configuration = hostContext.Configuration.GetValue<string>("ApplicationOptions:RedisOptions:ConnectionString"); ;
                        option.InstanceName = hostContext.Configuration.GetValue<string>("ApplicationOptions:RedisOptions:InstanceName");
                    });

                    services.AddTransient<ProductService>();
                    services.AddTransient<RuleProviderFactory>();

                    services.AddJob<ProductUpdaterJob>();
                })
                .UseDashFire()
                .Build()
                .Run();
        }
    }
}
