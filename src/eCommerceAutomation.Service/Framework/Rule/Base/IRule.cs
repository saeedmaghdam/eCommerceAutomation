namespace eCommerceAutomation.Service.Framework.Rule.Base
{
    public interface IRule
    {
        void Initialize(string html);

        RuleResultBase Execute();
    }
}
