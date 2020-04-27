using ATACK_Marketing_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Data {
    public class MarketingDbContext : DbContext {

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Vendor> Vendors{ get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<UserAudit> UserAudit { get; set; }
        public DbSet<VendorAudit> VendorAudit { get; set; }


        public DbSet<EventOrganizer> EventOrganizers { get; set; }
        public DbSet<EventVendor> EventVendors { get; set; }
        public DbSet<EventVendorUser> EventVendorUsers { get; set; }
        public DbSet<EventGuest> EventGuests { get; set; }
        public DbSet<EventGuestSubscription> EventGuestSubscriptions { get; set; }

        public MarketingDbContext(DbContextOptions<MarketingDbContext> options) : base(options) {
        }
    }
}
