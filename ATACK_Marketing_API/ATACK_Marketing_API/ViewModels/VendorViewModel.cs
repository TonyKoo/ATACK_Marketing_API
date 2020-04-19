using ATACK_Marketing_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.ViewModels {
    public class EventVendorsViewModel {
        public int EventId { get; set; }
        public String EventName { get; set; }
        public DateTime EventStartDateTime { get; set; }
        public int NumOfEventVendors { get; set; }
        public ICollection<EventVendorMinDetailViewModel> Vendors { get; set; }
    }

    public class EventVendorViewModel {
        public int EventId { get; set; }
        public String EventName { get; set; }
        public DateTime EventStartDateTime { get; set; }
        public EventVendorDetailViewModel Vendor { get; set; }
    }

    public class EventVendorMinDetailViewModel {
        public int EventVendorId { get; set; }
        public String VendorName { get; set; }
        public int NumOfProducts { get; set; }
    }

    public class EventVendorDetailViewModel {
        public int EventVendorId { get; set; }
        public String VendorName { get; set; }
        public String PhoneNumber { get; set; }
        public String Email { get; set; }
        public String Address { get; set; }
        public String City { get; set; }
        public String Province { get; set; }
        public String PostalCode { get; set; }
        public String Country { get; set; }
        public int NumOfProducts { get; set; }
        public ICollection<ProductMinViewModel> Products { get; set; }
    }

}
