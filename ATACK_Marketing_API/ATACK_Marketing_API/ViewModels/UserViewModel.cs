﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.ViewModels {
    public class UserViewModel {
        public String Email { get; set; }
        public bool IsAdmin { get; set; }
        public int EventsOrganizing { get; set; }
        public int VendorsManaged { get; set; }
    }

    public class UpdateUserAdminViewModel {
        public bool IsAdmin { get; set; }
    }

    public class UserAdminInputViewModel {
        [Required]
        public String UserEmailToModify { get; set; }
    }
}
