using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Repositories {
    public class EventVendorUserRepository {
        private readonly MarketingDbContext _context;

        public EventVendorUserRepository(MarketingDbContext context) {
            _context = context;
        }

        public EventVendorUserManagedViewModel GetEventVendorUserList(User theUser) {
            return new EventVendorUserManagedViewModel {
                UserEmail = theUser.Email,
                UserEventVendors = _context.EventVendorUsers.Where(evu => evu.User == theUser)
                                                            .OrderBy(evu => evu.EventVendor.Vendor.Name)
                                                            .Select(evu => new EventVendorUserManagedDetailViewModel {
                                                                EventVendorId = evu.EventVendor.EventVendorId,
                                                                EventId = evu.EventVendor.Event.EventId,
                                                                EventName = evu.EventVendor.Event.EventName,
                                                                VendorId = evu.EventVendor.Vendor.VendorId,
                                                                VendorName = evu.EventVendor.Vendor.Name
                                                            }).ToList()
            };
        }

        public bool AddEventVendorUser(EventVendor eventVendor, User requestingUser, User targetUser) {
            bool isSuccessful = false;

            try {
                //Audit
                UserAudit userAudit = new UserAudit {
                    EventDateTime = DateTime.Now,
                    GranterUid = requestingUser.Uid,
                    GranterEmail = requestingUser.Email,
                    GrantPermission = true,
                    PermissionType = $"EventVendorUser - EventVendor: {eventVendor.EventVendorId}, " +
                                     $"Vendor: {eventVendor.Vendor.VendorId}, " +
                                     $"Event: {eventVendor.Event.EventId}",
                    ModifiedUid = targetUser.Uid,
                    ModifiedEmail = targetUser.Email
                };
                _context.UserAudit.Add(userAudit);

                EventVendorUser newEventVendorUser = new EventVendorUser {
                    User = targetUser,
                    EventVendor = eventVendor
                };

                _context.EventVendorUsers.Add(newEventVendorUser);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {

            }

            return isSuccessful;
        }

        public bool RemoveEventVendorUser(EventVendorUser eventVendorUser, User requestingUser, User targetUser) {
            bool isSuccessful = false;

            try {
                //Audit
                UserAudit userAudit = new UserAudit {
                    EventDateTime = DateTime.Now,
                    GranterUid = requestingUser.Uid,
                    GranterEmail = requestingUser.Email,
                    GrantPermission = false,
                    PermissionType = $"EventVendorUser - EventVendor: {eventVendorUser.EventVendor.EventVendorId}, " +
                                     $"Vendor: {eventVendorUser.EventVendor.Vendor.VendorId}, " +
                                     $"Event: {eventVendorUser.EventVendor.Event.EventId}",
                    ModifiedUid = targetUser.Uid,
                    ModifiedEmail = targetUser.Email
                };
                _context.UserAudit.Add(userAudit);

                _context.EventVendorUsers.Remove(eventVendorUser);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {

            }

            return isSuccessful;
        }

    }
}
