using System.Collections.Generic;
using eCommerceAutomation.Service.Framework.Rule.Base;

namespace eCommerceAutomation.Service.Framework.Rule
{
    public class PriceListRuleResult : RuleResultBase
    {
        public List<decimal> Prices
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Join(", ", Prices).ToString();
        }
    }
}
