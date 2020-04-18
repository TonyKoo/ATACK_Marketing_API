using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    public class EventOrganizer {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventOrganizerId { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [Required]
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }
    }
}
