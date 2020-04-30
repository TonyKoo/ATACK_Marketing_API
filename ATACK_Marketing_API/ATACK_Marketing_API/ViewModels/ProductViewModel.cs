using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.ViewModels {

    public class ProductMinViewModel {
        public int ProductId { get; set; }
        public String ProductName { get; set; }
        public decimal ProductPrice { get; set; }
    }

    public class ProductInputViewModel {
        [Required]
        public String ProductName { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPrice { get; set; }
    }

    public class ProductRetrieveViewModel {
        public int ProductId { get; set; }
        public String ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int EventId { get; set; }
        public String EventName { get; set; }
        public int EventVendorId { get; set; }
        public String EventVendorName { get; set; }
    }
}
