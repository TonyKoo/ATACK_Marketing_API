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
        public String Name { get; set; }
        public String Description { get; set; }
        public String Email { get; set; }
        public String Website { get; set; }
    }
}
