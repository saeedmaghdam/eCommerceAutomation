using System.Collections.Generic;

namespace eCommerceAutomation.Service.Framework.Rule.Base
{
    public interface IWebsiteRuleFactory
    {
        string Key
        {
            get;
        }

        IEnumerable<IRule> GetRules();
    }
}
