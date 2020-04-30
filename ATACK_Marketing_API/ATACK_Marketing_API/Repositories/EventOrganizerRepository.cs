using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Repositories {
    public class EventOrganizerRepository {
        private readonly MarketingDbContext _context;

        public EventOrganizerRepository(MarketingDbContext context) {
            _context = context;
        }

        public int GetEventOrganizersCount(int eventId) {
            return _context.EventOrganizers.Where(eo => eo.Event.EventId == eventId).Count();
        }

        public UserEventOrganizerViewModel GetManagedEvents(User theUser) {
            return new UserEventOrganizerViewModel {
                UserId = theUser.UserId,
                UserEmail = theUser.Email,
                EventsOrganizing = _context.EventOrganizers.Where(eo => eo.User == theUser)
                                                           .Select(eo => new EventDetailViewModel {
                                                               EventId = eo.Event.EventId,
                                                               EventName = eo.Event.EventName,
                                                               EventStartDateTime = eo.Event.EventDateTime,
                                                               NumOfVendors = eo.Event.EventVendors.Count,
                                                               Venue = eo.Event.Venue
                                                           }).ToList()
            };
        }

        public EventOrganizerListViewModel GetEventOrganizers(Event theEvent) {
            return new EventOrganizerListViewModel {
                EventId = theEvent.EventId,
                EventName = theEvent.EventName,
                EventStartDateTime = theEvent.EventDateTime,
                VenueName = theEvent.Venue.VenueName,
                EventOrganizers = _context.EventOrganizers.Where(eo => eo.Event == theEvent)
                                                          .Select(eo => new EventOrganizerDetailViewModel {
                                                              EventOrganizerId = eo.EventOrganizerId,
                                                              UserId = eo.User.UserId,
                                                              UserEmail = eo.User.Email
                                                          }).ToList()
            };
        }

        public bool AddEventOrganizer(Event theEvent, User requestingUser, User targetUser) {
            bool isSuccessful = false;

            try {
                //Audit
                UserAudit userAudit = new UserAudit {
                    EventDateTime = DateTime.Now,
                    GranterUid = requestingUser.Uid,
                    GranterEmail = requestingUser.Email,
                    GrantPermission = true,
                    PermissionType = $"Event Organizer - Event ID: {theEvent.EventId}",
                    ModifiedUid = targetUser.Uid,
                    ModifiedEmail = targetUser.Email
                };
                _context.UserAudit.Add(userAudit);

                EventOrganizer newEventOrganizer = new EventOrganizer {
                    User = targetUser,
                    Event = theEvent
                };

                _context.EventOrganizers.Add(newEventOrganizer);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {

            }

            return isSuccessful;
        }

        public bool RemoveEventOrganizer(EventOrganizer eventOrganizerToRemove, User requestingUser, User targetUser) {
            bool isSuccessful = false;

            try {
                //Audit
                UserAudit userAudit = new UserAudit {
                    EventDateTime = DateTime.Now,
                    GranterUid = requestingUser.Uid,
                    GranterEmail = requestingUser.Email,
                    GrantPermission = false,
                    PermissionType = $"Event Organizer - Event ID: {eventOrganizerToRemove.Event.EventId}",
                    ModifiedUid = targetUser.Uid,
                    ModifiedEmail = targetUser.Email
                };
                _context.UserAudit.Add(userAudit);

                _context.EventOrganizers.Remove(eventOrganizerToRemove);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {

            }

            return isSuccessful;
        }
    }
}
