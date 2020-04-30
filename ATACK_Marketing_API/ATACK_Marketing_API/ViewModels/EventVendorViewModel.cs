using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.ViewModels {
    public class EventVendorAddRemoveViewModel {
        [Required]
        public int EventId { get; set; }
        [Required]
        public int VendorId { get; set; }
    }

    public class EventVendorResultViewModel {
        public int EventId { get; set; }
        public String EventName { get; set; }
        public DateTime EventStartDateTime { get; set; }
        public int VendorId { get; set; }
        public String VendorName { get; set; }
        public int EventVendorId { get; set; }
        public bool IsEventVendor { get; set; }
    }

    public class EventVendorRemoveInputViewModel {
        [Required]
        public String DeleteVendorString { get; set; }
    }
}
