using System.Threading;
using System.Threading.Tasks;
using DashFire;
using Microsoft.Extensions.Logging;

namespace eCommerceAutomation
{
    public class ProductUpdaterJob : Job
    {
        private readonly ILogger<ProductUpdaterJob> _logger;

        public override JobInformation JobInformation => JobInformationBuilder.CreateInstance()
            .RegistrationRequired()
            .SetCronSchedules(new[] { "0 */3 * * *" })
            .SetDescription("Fetch sources, analyze them and update product!")
            .SetDisplayName("Product Updater Job")
            .SetSystemName(nameof(ProductUpdaterJob))
            .Build();

        public ProductUpdaterJob(ILogger<ProductUpdaterJob> logger)
        {
            _logger = logger;
        }

        protected override Task StartInternallyAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
