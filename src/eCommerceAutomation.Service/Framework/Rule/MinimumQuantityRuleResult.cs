using eCommerceAutomation.Service.Framework.Rule.Base;

namespace eCommerceAutomation.Service.Framework.Rule
{
    public class MinimumQuantityRuleResult : RuleResultBase
    {
        public int MinimumQuantity
        {
            get;
            set;
        }

        public override string ToString()
        {
            return MinimumQuantity.ToString();
        }
    }
}
