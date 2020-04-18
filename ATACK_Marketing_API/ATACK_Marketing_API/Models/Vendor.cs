using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    public class Vendor {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VendorId { get; set; }
        public String VendorName { get; set; }
        public String PhoneNumber { get; set; }
        public String Email { get; set; }
        public String Address { get; set; }
        public String City { get; set; }
        public String Province { get; set; }
        public String PostalCode { get; set; }
        public String Country { get; set; }
    }
}
