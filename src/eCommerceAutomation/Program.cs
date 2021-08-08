using DashFire;
using Microsoft.Extensions.Hosting;

namespace eCommerceAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddJob<ProductUpdaterJob>();
                })
                .UseDashFire()
                .Build()
                .Run();
        }
    }
}
