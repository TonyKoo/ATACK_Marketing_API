using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Repositories {
    public class EventVendorRepository {
        private readonly MarketingDbContext _context;

        public EventVendorRepository(MarketingDbContext context) {
            _context = context;
        }

        public (bool, EventVendor) AddEventVendor(User requestingUser, Event theEvent, Vendor theVendor) {
            bool isSuccessful = false;
            EventVendor newEventVendor = new EventVendor {
                Event = theEvent,
                Vendor = theVendor
            };

            try {
                _context.EventVendors.Add(newEventVendor);
                _context.SaveChanges();

                _context.VendorAudit.Add(new VendorAudit {
                    EventDateTime = DateTime.Now,
                    UserUid = requestingUser.Uid,
                    UserEmail = requestingUser.Email,
                    Operation = $"Add Event Vendor",
                    Detail = $"Event: {theEvent.EventId} - {theEvent.EventName}, " +
                             $"Vendor: {theVendor.VendorId} - {theVendor.Name}, " +
                             $"EventVendor ID: {newEventVendor.EventVendorId}"
                });
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {

            }

            return (isSuccessful, newEventVendor);
        }

        public bool RemoveEventVendor(User requestingUser, EventVendor theEventVendor) {
            bool isSuccessful = false;

            try {
                _context.VendorAudit.Add(new VendorAudit {
                    EventDateTime = DateTime.Now,
                    UserUid = requestingUser.Uid,
                    UserEmail = requestingUser.Email,
                    Operation = $"Remove Event Vendor",
                    Detail = $"Event: {theEventVendor.Event.EventId} - {theEventVendor.Event.EventName}, " +
                             $"Vendor: {theEventVendor.Vendor.VendorId} - {theEventVendor.Vendor.Name}, " +
                             $"EventVendor ID: {theEventVendor.EventVendorId}"
                });

                _context.EventVendors.Remove(theEventVendor);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {

            }

            return isSuccessful;
        }
    }
}
