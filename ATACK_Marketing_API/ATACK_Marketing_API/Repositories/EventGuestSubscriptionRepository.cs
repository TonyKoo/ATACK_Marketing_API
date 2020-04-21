using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Repositories {
    public class EventGuestSubscriptionRepository {
        private readonly MarketingDbContext _context;

        public EventGuestSubscriptionRepository(MarketingDbContext context) {
            _context = context;
        }

        public EventSubscriptionSummaryViewModel GetUsersSubscriptions(User theUser) {

            ICollection<EventSubscriptionDetailViewModel> userSubscriptions = _context.EventGuests.Where(eg => eg.User == theUser)
                                                                                                  .Select(eg => new EventSubscriptionDetailViewModel {
                                                                                                      EventId = eg.Event.EventId,
                                                                                                      EventName = eg.Event.EventName,
                                                                                                      EventStartDateTime = eg.Event.EventDateTime,
                                                                                                      EventSubscriptions = _context.EventGuestSubscriptions
                                                                                                                                   .Where(egs => egs.EventVendor.Event == eg.Event &&
                                                                                                                                                 egs.EventGuest.User == theUser)
                                                                                                                                   .Select(egs => new EventSubscriptionVendorDetailViewModel {
                                                                                                                                       EventVendorId = egs.EventVendor.EventVendorId,
                                                                                                                                       VendorName = egs.EventVendor.Vendor.Name
                                                                                                                                   })
                                                                                                                                   .ToList()
                                                                                                  }).ToList();

            //Filter Out Blanks
            userSubscriptions = userSubscriptions.Where(us => us.EventSubscriptions.Count > 0).ToList();

            return new EventSubscriptionSummaryViewModel {
                UserEmail = theUser.Email,
                Subscriptions = userSubscriptions
            };
        }

        public bool SubscribeToEventVendor(EventGuest theEventGuest, EventVendor theEventVendor) {
            bool isSuccessful = false;

            try {
                EventGuestSubscription newSubscription = new EventGuestSubscription {
                    EventGuest = theEventGuest,
                    EventVendor = theEventVendor
                };

                _context.EventGuestSubscriptions.Add(newSubscription);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {

            }

            return isSuccessful;
        }

        public bool UnsubscribeEventVendor(EventGuestSubscription theEventGuestSubscription) {
            bool isSuccessful = false;

            try {
                _context.EventGuestSubscriptions.Remove(theEventGuestSubscription);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {

            }

            return isSuccessful;
        }
    }
}
