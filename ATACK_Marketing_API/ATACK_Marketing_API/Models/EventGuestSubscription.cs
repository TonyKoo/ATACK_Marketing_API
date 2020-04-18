using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    public class EventGuestSubscription {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventGuestSubscriptionId { get; set; }
        public virtual EventGuest EventGuest { get; set; }
        public virtual EventVendor EventVendor { get; set; }
    }
}
