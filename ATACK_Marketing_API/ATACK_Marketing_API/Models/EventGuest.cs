using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    public class EventGuest {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventGuestId { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [Required]
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        public virtual ICollection<EventGuestSubscription> Subscriptions { get; set; }
    }
}
