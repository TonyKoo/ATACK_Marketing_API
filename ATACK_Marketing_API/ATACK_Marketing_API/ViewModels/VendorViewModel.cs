using ATACK_Marketing_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public String Description { get; set; }
        public String Email { get; set; }
        public String Website { get; set; }
        public int NumOfProducts { get; set; }
        public ICollection<ProductMinViewModel> Products { get; set; }
    }

    public class VendorManagementViewModel {
        public int VendorId { get; set; }
        public String Name { get; set; }
    }

    public class VendorAddModifyViewModel {
        [Required]
        public String Name { get; set; }
        [Required]
        public String Description { get; set; }
        public String Email { get; set; }
        public String Website { get; set; }
    }

    public class VendorDeleteViewModel {
        [Required]
        public String ConfirmDeleteName { get; set; }
    }
}
