using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.ViewModels {
    public class EventVendorUserInputViewModel {
        [Required]
        public int EventVendorId { get; set; }
        [Required]
        public String UserEmailToModify { get; set; }
    }

    public class EventVendorUserResultViewModel { 
        public String EventName { get; set; }
        public String UserEmailToModify { get; set; }
        public int EventVendorId { get; set; }
        public bool GrantedAccess { get; set; }
    }

    public class EventVendorUserManagedViewModel {
        public String UserEmail { get; set; }
        public ICollection<EventVendorUserManagedDetailViewModel> UserEventVendors { get; set; }
    }

    public class EventVendorUserManagedDetailViewModel {
        public int EventVendorId { get; set; }
        public int EventId { get; set; }
        public String EventName { get; set; }
        public int VendorId { get; set; }
        public String VendorName { get; set; }
    }

    public class EventVendorUserListViewModel {
        public int EventVendorId { get; set; }
        public int EventId { get; set; }
        public String EventName { get; set; }
        public ICollection<EventVendorUserDetailViewModel> VendorUsers { get; set; }
    }

    public class EventVendorUserDetailViewModel {
        public String UserEmail { get; set; }
    }
}
