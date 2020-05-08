using ATACK_Marketing_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.ViewModels {
    public class EventSubscriptionViewModel {
        public int EventId { get; set; }
        public String UserEmail { get; set; }
        public String EventName { get; set; }
        public int EventVendorId { get; set; }
        public String EventVendor { get; set; }
        public bool Subscribed { get; set; }
    }

    public class EventSubscriptionSummaryViewModel {
        public String UserEmail { get; set; }
        public ICollection<EventSubscriptionDetailViewModel> Subscriptions { get; set; }
    }

    public class EventSubscriptionDetailViewModel {
        public int EventId { get; set; }
        public String EventName { get; set; }
        public DateTime EventStartDateTime { get; set; }
        public Venue Venue { get; set; }
        public ICollection<EventSubscriptionVendorDetailViewModel> EventSubscriptions { get; set; }
    }

    public class EventSubscriptionVendorDetailViewModel {
        public int EventVendorId { get; set; }
        public String VendorName { get; set; }
    }
}
