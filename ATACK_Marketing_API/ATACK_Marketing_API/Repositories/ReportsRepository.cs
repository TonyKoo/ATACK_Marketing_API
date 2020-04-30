using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Repositories {
    public class ReportsRepository {
        private readonly MarketingDbContext _context;

        public ReportsRepository(MarketingDbContext context) {
            _context = context;
        }

        public List<VendorSubscriberReportViewModel> GetAllSubscribers(User theUser) {

            return _context.EventVendorUsers.Where(evu => evu.User == theUser)
                .Select(evu => new VendorSubscriberReportViewModel{
                    EventId = evu.EventVendor.Event.EventId,
                    EventName = evu.EventVendor.Event.EventName,
                    EventStartDateTime = evu.EventVendor.Event.EventDateTime,
                    EventVendorId = evu.EventVendor.EventVendorId,
                    VendorName = evu.EventVendor.Vendor.Name,
                    Subscribers = _context.EventGuestSubscriptions.Where(egs => egs.EventVendor == evu.EventVendor)
                                                                  .Select(egs => new VendorSubscriberDetailViewModel {
                                                                      UserEmail = egs.EventGuest.User.Email
                                                                  }).ToList()
                }).ToList();
        }

        //public EventVendorUserManagedViewModel GetEventVendorUserList(User theUser) {
        //    return new EventVendorUserManagedViewModel {
        //        UserEmail = theUser.Email,
        //        UserEventVendors = _context.EventVendorUsers.Where(evu => evu.User == theUser)
        //                                                    .OrderBy(evu => evu.EventVendor.Vendor.Name)
        //                                                    .Select(evu => new EventVendorUserManagedDetailViewModel {
        //                                                        EventVendorId = evu.EventVendor.EventVendorId,
        //                                                        EventId = evu.EventVendor.Event.EventId,
        //                                                        EventName = evu.EventVendor.Event.EventName,
        //                                                        VendorId = evu.EventVendor.Vendor.VendorId,
        //                                                        VendorName = evu.EventVendor.Vendor.Name
        //                                                    }).ToList()
        //    };
        //}

        public VendorSubscriberReportViewModel GetEventSubscribers(EventVendor eventVendor) {
            return new VendorSubscriberReportViewModel {
                EventId = eventVendor.Event.EventId,
                EventName = eventVendor.Event.EventName,
                EventStartDateTime = eventVendor.Event.EventDateTime,
                EventVendorId = eventVendor.EventVendorId,
                VendorName = eventVendor.Vendor.Name,
                Subscribers = _context.EventGuestSubscriptions.Where(egs => egs.EventVendor == eventVendor)
                                                              .Select(egs => new VendorSubscriberDetailViewModel {
                                                                    UserEmail = egs.EventGuest.User.Email
                                                              }).ToList()
            };
        }
    }
}
