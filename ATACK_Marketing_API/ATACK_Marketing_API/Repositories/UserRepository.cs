using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Repositories {
    public class UserRepository {
        private readonly MarketingDbContext _context;

        public UserRepository(MarketingDbContext context) {
            _context = context;
        }

        public bool CreateUser(string uid, string userEmail) {
            bool isSuccessful = false;

            try {
                User newUser = new User {
                    Uid = uid,
                    Email = userEmail,
                    IsAdmin = false
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {
                //Do Nothing
            }

            return isSuccessful;

        }
    }
}
