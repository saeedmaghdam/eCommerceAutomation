using eCommerceAutomation.Service.Framework.Rule.Base;

namespace eCommerceAutomation.Service.Framework.Rule
{
    public class AvailabilityRuleResult : RuleResultBase
    {
        public bool IsInStock
        {
            get;
            set;
        }

        public override string ToString()
        {
            return IsInStock.ToString();
        }
    }
}
