using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.ViewModels {
    public class VendorSubscriberReportViewModel {
        public int EventId { get; set; }
        public String EventName { get; set; }
        public DateTime EventStartDateTime { get; set; }
        public int EventVendorId { get; set; }
        public String VendorName { get; set; }
        public ICollection<VendorSubscriberDetailViewModel> Subscribers { get; set; }
    }

    public class VendorSubscriberDetailViewModel {
        public String UserEmail { get; set; }
    }
}
