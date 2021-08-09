using System.Collections.Generic;
using eCommerceAutomation.Service.Framework.Rule.Base;
using eCommerceAutomation.Service.Utils;

namespace eCommerceAutomation.Service.Rules
{
    public class RuleProviderFactory : IRuleFactoryProviderFactory
    {
        public IEnumerable<IRuleFactoryProvider> GetProviders() => ReflectiveEnumerator.GetEnumerableOfType<IRuleFactoryProvider>();
    }
}
