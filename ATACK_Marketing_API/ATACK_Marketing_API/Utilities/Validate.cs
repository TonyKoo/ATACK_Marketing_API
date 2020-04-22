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
        public static bool VerifiedUser(ClaimsPrincipal userClaims) {
            bool isValidUser = false;

            var emailVerified = userClaims.Claims.ElementAt(8).Value;

            if (emailVerified == "true") {
                isValidUser = true;
            }

            return isValidUser;
        }
    }
}
