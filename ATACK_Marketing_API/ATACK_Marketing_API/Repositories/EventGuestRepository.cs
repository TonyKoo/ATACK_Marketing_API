using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Repositories {
    public class EventGuestRepository {
        private readonly MarketingDbContext _context;

        public EventGuestRepository(MarketingDbContext context) {
            _context = context;
        }

        public UserEventListViewModel GetUsersEvents(User theUser) {
            ICollection<UserEventListEventDetailViewModel> eventsJoined = _context.EventGuests.Where(eg => eg.User == theUser).Select(eg => new UserEventListEventDetailViewModel {
                EventId = eg.Event.EventId,
                EventName = eg.Event.EventName,
                EventStartDateTime = eg.Event.EventDateTime
            }).ToList();

            return new UserEventListViewModel {
                UserEmail = theUser.Email,
                EventsJoined = eventsJoined
            };
        }

        public bool JoinEvent(User theUser, Event theEvent) {
            bool isSuccessful = false;

            try {
                EventGuest joinEvent = new EventGuest {
                    User = theUser,
                    Event = theEvent
                };

                _context.EventGuests.Add(joinEvent);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {

            }

            return isSuccessful;
        }

        public bool LeaveEvent(EventGuest theEventJoined) {
            bool isSuccessful = false;

            try {
                _context.EventGuests.Remove(theEventJoined);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {

            }

            return isSuccessful;
        }
    }
}
