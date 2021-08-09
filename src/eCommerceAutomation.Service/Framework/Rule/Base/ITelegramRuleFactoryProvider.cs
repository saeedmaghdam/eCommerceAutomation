using System.Collections.Generic;

namespace eCommerceAutomation.Service.Framework.Rule.Base
{
    public interface ITelegramRuleFactoryProvider : IRuleFactoryProvider
    {
        IEnumerable<ITelegramRuleFactory> GetFactories();
    }
}
