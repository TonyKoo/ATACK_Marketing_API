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

        public ICollection<VendorManagementViewModel> GetAllVendors() {
            return _context.Vendors.OrderBy(v => v.Name)
                                   .Select(v => new VendorManagementViewModel {
                                        VendorId = v.VendorId,
                                        Name = v.Name
                                    }).ToList();
        }

        public Vendor GetVendor(int vendorId) {
            return _context.Vendors.FirstOrDefault(v => v.VendorId == vendorId);
        }

        public (bool, Vendor) AddVendor(User requestingUser, VendorAddModifyViewModel theVendor) {
            bool isSuccessful = false;
            Vendor newVendor = new Vendor {
                Name = theVendor.Name,
                Description = theVendor.Description,
                Email = theVendor.Email,
                Website = theVendor.Website
            };

            try {
                _context.Vendors.Add(newVendor);
                _context.SaveChanges(); //Needed to Generate Vendor ID

                _context.VendorAudit.Add(new VendorAudit {
                    EventDateTime = DateTime.Now,
                    UserUid = requestingUser.Uid,
                    UserEmail = requestingUser.Email,
                    Operation = $"Add Vendor",
                    Detail = $"Vendor ID: {newVendor.VendorId} - {newVendor.Name}"
                });
                _context.SaveChanges();

                isSuccessful = true;
            } catch (Exception) {
                //Do Nothing
            }

            return (isSuccessful, newVendor);
        }

        public (bool, Vendor) UpdateVendor(User requestingUser, Vendor theVendor, VendorAddModifyViewModel updatedVendor) {
            bool isSuccessful = false;

            try {
                _context.VendorAudit.Add(new VendorAudit {
                    EventDateTime = DateTime.Now,
                    UserUid = requestingUser.Uid,
                    UserEmail = requestingUser.Email,
                    Operation = $"Update Vendor",
                    Detail = $"Vendor ID: {theVendor.VendorId} - {theVendor.Name}"
                });

                //Only Allow Name Update For Admin
                if (requestingUser.IsAdmin) {
                    theVendor.Name = updatedVendor.Name;
                }
                theVendor.Description = updatedVendor.Description;
                theVendor.Email = updatedVendor.Email;
                theVendor.Website = updatedVendor.Website;

                _context.Vendors.Update(theVendor);
                _context.SaveChanges();

                isSuccessful = true;
            } catch (Exception) {
                //Do Nothing
            }

            return (isSuccessful, theVendor);
        }

        public bool DeleteVendor(User requestingUser, Vendor vendorToDelete) {
            bool isSuccessful = false;

            try {
                _context.VendorAudit.Add(new VendorAudit {
                    EventDateTime = DateTime.Now,
                    UserUid = requestingUser.Uid,
                    UserEmail = requestingUser.Email,
                    Operation = $"Delete Vendor",
                    Detail = $"Vendor ID: {vendorToDelete.VendorId} - {vendorToDelete.Name}"
                });

                _context.Vendors.Remove(vendorToDelete);
                _context.SaveChanges();
                isSuccessful = true;
            } catch (Exception) {
                //Do Nothing
            }

            return isSuccessful;
        }

        public EventVendorsViewModel GetAllEventVendors(Event theEvent) {
            ICollection<EventVendorMinDetailViewModel> vendors = _context.EventVendors.Where(ev => ev.Event == theEvent)
                                                                                   .OrderBy(ev => ev.Vendor.Name)
                                                                                   .Select(ev => new EventVendorMinDetailViewModel { 
                                                                                       EventVendorId    = ev.EventVendorId,
                                                                                       VendorName       = ev.Vendor.Name,
                                                                                       NumOfProducts    = ev.Products.Count
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
                                                                                       EventVendorId    = ev.EventVendorId,
                                                                                       VendorName       = ev.Vendor.Name,
                                                                                       Email            = ev.Vendor.Email,
                                                                                       Description      = ev.Vendor.Description,
                                                                                       Website          = ev.Vendor.Website,
                                                                                       NumOfProducts    = ev.Products.Count,
                                                                                       Products = _context.Products.Where(pr => pr.EventVendor == theEventVendor)
                                                                                                                   .OrderBy(pr => pr.ProductName)
                                                                                                                   .Select(pr => new ProductMinViewModel { 
                                                                                                                       ProductId    = pr.ProductId,
                                                                                                                       ProductName  = pr.ProductName,
                                                                                                                       ProductPrice = pr.ProductPrice
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
