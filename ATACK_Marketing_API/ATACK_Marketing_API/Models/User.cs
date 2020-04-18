using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    public class User {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public bool IsAdmin { get; set; }
        public String Email { get; set; }
        [Required]
        public String Uid { get; set; }
        //[Required]
        //public virtual Role Role { get; set; }
    }
}
