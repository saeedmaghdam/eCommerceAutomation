using System;
using System.Collections.Generic;
using eCommerceAutomation.Service.Framework.Constants;

namespace eCommerceAutomation.Service.Models.API.ViewModel
{
    public class Product
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

        public string ExternalId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public int? OriginalMinimumQuantity
        {
            get;
            set;
        }

        public decimal? OriginalPrice
        {
            get;
            set;
        }

        public string OriginalWholesalePrices
        {
            get;
            set;
        }

        public int? MinimumQuantity
        {
            get;
            set;
        }

        public decimal? Price
        {
            get;
            set;
        }

        public string WholesalePrices
        {
            get;
            set;
        }

        public bool IsReviewNeeded
        {
            get;
            set;
        }

        public bool IsDisabled
        {
            get;
            set;
        }

        public bool IsInitialized
        {
            get;
            set;
        }

        public IEnumerable<Source> Sources
        {
            get;
            set;
        }
    }
}
