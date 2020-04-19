using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    public class Product {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        public String ProductName { get; set; }
        public String ProductDetails { get; set; }
        [Required]
        [ForeignKey("EventVendorId")]
        public virtual EventVendor EventVendor { get; set; }
    }
}
