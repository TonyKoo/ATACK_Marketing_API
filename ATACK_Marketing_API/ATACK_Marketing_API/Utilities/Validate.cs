using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Utilities {
    public static class Validate {
        public static bool ValidUser(ClaimsPrincipal user, MarketingDbContext context) {
            bool isValidUser = false;
            string uid = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            User theUser = context.Users.FirstOrDefault(u => u.Uid == uid);

            if (theUser != null) {
                isValidUser = true;
            }

            return isValidUser;
        }
    }
}
