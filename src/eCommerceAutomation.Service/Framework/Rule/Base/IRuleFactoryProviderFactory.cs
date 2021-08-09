using System.Collections.Generic;

namespace eCommerceAutomation.Service.Framework.Rule.Base
{
    public interface IRuleFactoryProviderFactory
    {
        IEnumerable<IRuleFactoryProvider> GetProviders();
    }
}
