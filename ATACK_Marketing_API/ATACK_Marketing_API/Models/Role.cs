using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    //public class Role {
    //    [Required]
    //    public String RoleId { get; set; }
    //}

    public static class Role {
        public const string Admin = "Admin";
        public const string EventOrganizer = "EventOrganizer";
        public const string EventVendor = "EventVendor";
        public const string User = "User";
    }
}
