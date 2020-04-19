using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Repositories {
    public class EventsRepository {
        private readonly MarketingDbContext _context;

        public EventsRepository(MarketingDbContext context) {
            _context = context;
        }

        public EventsViewModel GetAllEvents() {
            ICollection<EventDetailViewModel> events = _context.Events.OrderBy(e => e.EventDateTime)
                                                                      .Select(e => new EventDetailViewModel {
                                                                          EventId = e.EventId,
                                                                          EventName = e.EventName,
                                                                          EventStartDateTime = e.EventDateTime,
                                                                          NumOfVendors = e.EventVendors.Count,
                                                                          Venue = e.Venue
                                                                      })
                                                                      .ToList();

            return new EventsViewModel {
                NumOfEvents = events.Count,
                Events = events
            };
        }

        public Event GetEvent(int eventId) {
            return _context.Events.FirstOrDefault(e => e.EventId == eventId);
        }
    }
}
