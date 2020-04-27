using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    public class VendorAudit {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VendorAuditId { get; set; }
        public DateTime EventDateTime { get; set; }
        public String UserUid { get; set; }
        public String UserEmail { get; set; }
        public String Operation { get; set; }
        public String Detail { get; set; }
    }
}
