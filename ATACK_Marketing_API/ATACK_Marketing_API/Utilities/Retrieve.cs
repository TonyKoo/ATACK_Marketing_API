using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Utilities {
    public class Retrieve {
        public static User User (ClaimsPrincipal userClaims, MarketingDbContext context) {
            string uid = userClaims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return context.Users.FirstOrDefault(u => u.Uid == uid);
        }
    }
}
