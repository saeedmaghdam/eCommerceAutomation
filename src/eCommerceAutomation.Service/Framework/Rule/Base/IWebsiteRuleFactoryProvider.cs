using System.Collections.Generic;

namespace eCommerceAutomation.Service.Framework.Rule.Base
{
    public interface IWebsiteRuleFactoryProvider : IRuleFactoryProvider
    {
        IEnumerable<IWebsiteRuleFactory> GetFactories();
    }
}
