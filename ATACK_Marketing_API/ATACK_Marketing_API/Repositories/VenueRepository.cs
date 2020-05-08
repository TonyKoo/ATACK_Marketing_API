using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Repositories {
    public class VenueRepository {
        private readonly MarketingDbContext _context;

        public VenueRepository(MarketingDbContext context) {
            _context = context;
        }

        public List<VenueViewModel> getVenues() {
            return _context.Venues.OrderBy(v => v.VenueName)
                .Select(v => new VenueViewModel { 
                VenueId = v.VenueId,
                VenueName = v.VenueName,
                Website = v.Website
            }).ToList();
        }

        public Venue getVenue(int venueId) {
            return _context.Venues.FirstOrDefault(v => v.VenueId == venueId);
        }

        public (bool, Venue) AddVenue(User requestingUser, VenueInputViewModel newVenue) {
            bool isSuccessful = false;
            Venue venue = new Venue {
                VenueName = newVenue.VenueName,
                Website = newVenue.Website
            };

            try {
                _context.Venues.Add(venue);
                _context.SaveChanges();

                _context.VendorAudit.Add(new VendorAudit {
                    EventDateTime = DateTime.Now,
                    UserUid = requestingUser.Uid,
                    UserEmail = requestingUser.Email,
                    Operation = $"Add Venue",
                    Detail = $"Venue: {venue.VenueId} - {venue.VenueName}"
                });
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {
                //Do Nothing
            }

            return (isSuccessful, venue);
        }

        public bool UpdateVenue(User requestingUser,Venue theVenue, VenueInputViewModel updatedVenue) {
            bool isSuccessful = false;

            try {
                theVenue.VenueName = updatedVenue.VenueName;
                theVenue.Website = updatedVenue.Website;

                _context.VendorAudit.Add(new VendorAudit {
                    EventDateTime = DateTime.Now,
                    UserUid = requestingUser.Uid,
                    UserEmail = requestingUser.Email,
                    Operation = $"Update Venue",
                    Detail = $"Venue: {theVenue.VenueId} - {theVenue.VenueName}"
                });
                _context.Venues.Update(theVenue);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {
                //Do Nothing
            }

            return isSuccessful;
        }

        public bool RemoveVenue(User requestingUser, Venue theVenue) {
            bool isSuccessful = false;

            try {
                _context.VendorAudit.Add(new VendorAudit {
                    EventDateTime = DateTime.Now,
                    UserUid = requestingUser.Uid,
                    UserEmail = requestingUser.Email,
                    Operation = $"Remove Venue",
                    Detail = $"Venue: {theVenue.VenueId} - {theVenue.VenueName}"
                });
                _context.Venues.Remove(theVenue);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {
                //Do Nothing
            }

            return isSuccessful;
        }

    }
}
