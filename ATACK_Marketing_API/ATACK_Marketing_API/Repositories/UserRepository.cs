using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.ViewModels;
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

        public UserViewModel GetUser(User theUser) {
            return _context.Users.Where(u => u == theUser)
                                 .Select(u => new UserViewModel {
                                     Email = u.Email,
                                     IsAdmin = u.IsAdmin,
                                     IsEventOrganizer = _context.EventOrganizers.Any(eo => eo.User == u),
                                     IsVendor = _context.EventVendorUsers.Any(evu => evu.User == u)
                                 }).FirstOrDefault();
        }

        public List<UserViewModel> GetAllUsers() {
            return _context.Users.OrderBy(u => u.Email)
                                 .Select(u => new UserViewModel {
                                     Email = u.Email,
                                     IsAdmin = u.IsAdmin,
                                     IsEventOrganizer = _context.EventOrganizers.Any(eo => eo.User == u),
                                     IsVendor = _context.EventVendorUsers.Any(evu => evu.User == u)
                                 }).ToList();
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

        public bool ElevateAdminUser(User requestingUser, User targetUser) {
            bool isSuccessful = false;

            try {
                //Audit
                UserAudit userAudit = new UserAudit {
                    EventDateTime = DateTime.Now,
                    GranterUid = requestingUser.Uid,
                    GranterEmail = requestingUser.Email,
                    GrantPermission = true,
                    PermissionType = "Admin Rights",
                    ModifiedUid = targetUser.Uid,
                    ModifiedEmail = targetUser.Email
                };
                _context.UserAudit.Add(userAudit);

                //Elevate
                targetUser.IsAdmin = true;
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {
                //Do Nothing
            }

            return isSuccessful;

        }

        public bool DemoteAdminUser(User requestingUser, User targetUser) {
            bool isSuccessful = false;

            try {
                //Audit
                UserAudit userAudit = new UserAudit {
                    EventDateTime = DateTime.Now,
                    GranterUid = requestingUser.Uid,
                    GranterEmail = requestingUser.Email,
                    GrantPermission = false,
                    PermissionType = "Admin Rights",
                    ModifiedUid = targetUser.Uid,
                    ModifiedEmail = targetUser.Email
                };
                _context.UserAudit.Add(userAudit);

                //Demote
                targetUser.IsAdmin = false;
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {
                //Do Nothing
            }

            return isSuccessful;

        }
    }
}
