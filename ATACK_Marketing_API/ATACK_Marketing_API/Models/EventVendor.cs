using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    public class EventVendor {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventVendorId { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        [Required]
        [ForeignKey("VendorId")]
        public virtual Vendor Vendor { get; set; }
    }
}
