using eCommerceAutomation.Service.Framework.Constants;

namespace eCommerceAutomation.Service.Models.Scrapper
{
    public class Request
    {
        public string RequestId
        {
            get;
            set;
        }

        public RequestType Type
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }
    }
}
