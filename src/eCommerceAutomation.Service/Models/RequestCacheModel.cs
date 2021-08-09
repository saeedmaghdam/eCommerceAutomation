using MessagePack;

namespace eCommerceAutomation.Service.Models
{
    [MessagePackObject]
    public class RequestCacheModel
    {
        [Key(0)]
        public string RequestId
        {
            get;
            set;
        }

        [Key(1)]
        public long ProductId
        {
            get;
            set;
        }

        [Key(2)]
        public long SourceId
        {
            get;
            set;
        }
    }
}
