using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    public class EventVendorUser {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventVendorUserId { get; set; }
        [Required]
        [ForeignKey("EventVendorId")]
        public virtual EventVendor EventVendor { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
