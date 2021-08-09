using System.Collections.Generic;

namespace eCommerceAutomation.Service.Framework.Rule.Base
{
    public interface ITelegramRuleFactory
    {
        string Key
        {
            get;
        }

        IEnumerable<IRule> GetRules();
    }
}
