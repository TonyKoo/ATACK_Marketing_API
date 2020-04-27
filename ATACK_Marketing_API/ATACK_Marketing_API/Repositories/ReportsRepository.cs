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
