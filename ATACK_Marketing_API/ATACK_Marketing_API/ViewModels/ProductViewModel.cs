using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.ViewModels {

    public class ProductMinViewModel {
        public int ProductId { get; set; }
        public String ProductName { get; set; }
    }

    public class ProductInputViewModel {
        [Required]
        public String ProductName { get; set; }
        [Required]
        public String ProductDetails { get; set; }
    }

    public class ProductRetrieveViewModel {
        public int ProductId { get; set; }
        public String ProductName { get; set; }
        public String ProductDetails { get; set; }
        public int EventId { get; set; }
        public String EventName { get; set; }
        public int EventVendorId { get; set; }
        public String EventVendorName { get; set; }
    }
}
