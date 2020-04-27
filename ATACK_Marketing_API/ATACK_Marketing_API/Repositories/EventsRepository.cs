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

        public (bool, Event) AddEvent(User requestingUser, EventAddModifyViewModel theNewEvent, Venue theVenue) {
            bool isSuccessful = false;
            Event newEvent = new Event {
                EventName = theNewEvent.EventName,
                EventDateTime = theNewEvent.EventStartDateTime,
                Venue = theVenue
            };

            try {
                _context.Events.Add(newEvent);
                _context.SaveChanges();


                _context.VendorAudit.Add(new VendorAudit {
                    EventDateTime = DateTime.Now,
                    UserUid = requestingUser.Uid,
                    UserEmail = requestingUser.Email,
                    Operation = $"Add Event",
                    Detail = $"Event: {newEvent.EventId} - {newEvent.EventName}"
                });
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {
                //Do Nothing
            }


            return (isSuccessful, newEvent);
        }

        public bool UpdateEvent(User requestingUser, Event theEvent, EventAddModifyViewModel theUpdatedEvent, Venue theVenue) {
            bool isSuccessful = false;
            try {
                theEvent.EventName = theUpdatedEvent.EventName;
                theEvent.EventDateTime = theUpdatedEvent.EventStartDateTime;
                theEvent.Venue = theVenue;

                _context.Events.Update(theEvent);
                _context.VendorAudit.Add(new VendorAudit {
                    EventDateTime = DateTime.Now,
                    UserUid = requestingUser.Uid,
                    UserEmail = requestingUser.Email,
                    Operation = $"Update Event",
                    Detail = $"Event: {theEvent.EventId} - {theEvent.EventName}"
                });

                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {
                //Do Nothing
            }


            return isSuccessful;
        }

        public bool RemoveEvent(User requestingUser, Event theEvent) {
            bool isSuccessful = false;
            try {
                _context.VendorAudit.Add(new VendorAudit {
                    EventDateTime = DateTime.Now,
                    UserUid = requestingUser.Uid,
                    UserEmail = requestingUser.Email,
                    Operation = $"Remove Event",
                    Detail = $"Event: {theEvent.EventId} - {theEvent.EventName}"
                });

                _context.Events.Remove(theEvent);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {
                //Do Nothing
            }


            return isSuccessful;
        }
    }
}
