using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Repositories {
    public class VendorsRepository {
        private readonly MarketingDbContext _context;

        public VendorsRepository(MarketingDbContext context) {
            _context = context;
        }

        public EventVendorsViewModel GetAllEventVendors(Event theEvent) {
            ICollection<EventVendorMinDetailViewModel> vendors = _context.EventVendors.Where(ev => ev.Event == theEvent)
                                                                                   .OrderBy(ev => ev.Vendor.Name)
                                                                                   .Select(ev => new EventVendorMinDetailViewModel { 
                                                                                       EventVendorId = ev.EventVendorId,
                                                                                       VendorName = ev.Vendor.Name,
                                                                                       NumOfProducts = ev.Products.Count
                                                                                   })
                                                                                   .ToList();

            return new EventVendorsViewModel {
                EventId = theEvent.EventId,
                EventName = theEvent.EventName,
                EventStartDateTime = theEvent.EventDateTime,
                NumOfEventVendors = vendors.Count,
                Vendors = vendors
            };
        }

        public EventVendorViewModel GetEventVendor(Event theEvent, EventVendor theEventVendor) {
            EventVendorDetailViewModel vendor = _context.EventVendors.Where(ev => ev.Event == theEvent && ev == theEventVendor)
                                                                                   .Select(ev => new EventVendorDetailViewModel {
                                                                                       EventVendorId = ev.EventVendorId,
                                                                                       VendorName = ev.Vendor.Name,
                                                                                       Email = ev.Vendor.Email,
                                                                                       Description = ev.Vendor.Description,
                                                                                       Website = ev.Vendor.Website,
                                                                                       NumOfProducts = ev.Products.Count,
                                                                                       Products = _context.Products.Where(pr => pr.EventVendor == theEventVendor)
                                                                                                                   .OrderBy(pr => pr.ProductName)
                                                                                                                   .Select(pr => new ProductMinViewModel { 
                                                                                                                       ProductId = pr.ProductId,
                                                                                                                       ProductName = pr.ProductName
                                                                                                                   })
                                                                                                                   .ToList()
                                                                                   }).FirstOrDefault();

            return new EventVendorViewModel {
                EventId = theEvent.EventId,
                EventName = theEvent.EventName,
                EventStartDateTime = theEvent.EventDateTime,
                Vendor = vendor
            };
        }
    }
}
