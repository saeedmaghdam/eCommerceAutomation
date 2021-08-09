using System;

namespace eCommerceAutomation.Service.Framework.Rule.Base
{
    public abstract class RuleBase
    {
        public abstract RuleResultBase Execute();

        protected string Html
        {
            get;
            private set;
        }

        protected bool IsInitialized
        {
            get;
            private set;
        }

        protected void SetInitialized() => IsInitialized = true;

        public virtual void Initialize(string html)
        {
            Html = html;

            SetInitialized();
        }

        protected abstract RuleResultBase Process();

        protected RuleResultBase ExecuteRule()
        {
            if (!IsInitialized)
                throw new Exception("Rule is not initialized.");

            return Process();
        }
    }
}
