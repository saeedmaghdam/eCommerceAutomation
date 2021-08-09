using eCommerceAutomation.Service.Framework.Rule.Base;

namespace eCommerceAutomation.Service.Framework.Rule
{
    public class InStockQuantityRuleResult : RuleResultBase
    {
        public int TotalAvailableInStock
        {
            get;
            set;
        }

        public override string ToString()
        {
            return TotalAvailableInStock.ToString();
        }
    }
}
