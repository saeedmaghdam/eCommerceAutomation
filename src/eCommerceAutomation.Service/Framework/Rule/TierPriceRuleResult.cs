using System.Collections.Generic;
using eCommerceAutomation.Service.Framework.Rule.Base;

namespace eCommerceAutomation.Service.Framework.Rule
{
    public class TierPriceRuleResult : RuleResultBase
    {
        public TierPriceRuleResult() => Data = new List<TierPriceItemModel>();

        public ICollection<TierPriceItemModel> Data
        {
            get;
            set;
        }

        public override string ToString()
        {
            var result = "";

            foreach (var tierPriceTableStruct in Data)
                result += $"{tierPriceTableStruct.Quantity}-{tierPriceTableStruct.Price}   ";

            return result;
        }
    }
}
