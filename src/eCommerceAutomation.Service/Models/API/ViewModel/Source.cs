using System;
using eCommerceAutomation.Service.Framework.Constants;

namespace eCommerceAutomation.Service.Models.API.ViewModel
{
    public class Source
    {
        public long Id
        {
            get;
            set;
        }

        public Guid ViewId
        {
            get;
            set;
        }

        public DateTime RecordInsertDateTime
        {
            get;
            set;
        }

        public DateTime RecordUpdateDateTime
        {
            get;
            set;
        }

        public RecordStatus RecordStatus
        {
            get;
            set;
        }

        public int Priority
        {
            get;
            set;
        }

        public SourceType SourceType
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }

        public string OldMetadata
        {
            get;
            set;
        }

        public string Metadata
        {
            get;
            set;
        }

        public string PriceAdjustment
        {
            get;
            set;
        }

        public string WholesalePriceAdjustment
        {
            get;
            set;
        }

        public bool IsDisabled
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }
    }
}
