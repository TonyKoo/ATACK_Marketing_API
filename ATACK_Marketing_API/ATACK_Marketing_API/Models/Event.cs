using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    public class Event {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }
        public String EventName { get; set; }
        public DateTime EventDateTime { get; set; }
        [Required]
        [ForeignKey("VenueId")]
        public virtual Venue Venue { get; set; }
        public virtual ICollection<EventVendor> EventVendors { get; set; }
    }
}
