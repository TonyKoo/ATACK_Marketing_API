using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    public class UserAudit {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserAuditId { get; set; }
        public DateTime EventDateTime { get; set; }
        public String GranterUid { get; set; }
        public String GranterEmail { get; set; }
        public bool GrantPermission { get; set; }
        public String PermissionType { get; set; }
        public String ModifiedUid { get; set; }
        public String ModifiedEmail { get; set; }
    }
}
