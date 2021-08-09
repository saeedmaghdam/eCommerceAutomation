using eCommerceAutomation.Service.Framework.Rule.Base;

namespace eCommerceAutomation.Service.Framework.Rule
{
    public class OldPriceRuleResult : RuleResultBase
    {
        public decimal Price
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Price.ToString();
        }
    }
}
