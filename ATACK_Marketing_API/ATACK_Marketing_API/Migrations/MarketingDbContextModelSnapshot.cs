﻿// <auto-generated />
using System;
using ATACK_Marketing_API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ATACK_Marketing_API.Migrations
{
    [DbContext(typeof(MarketingDbContext))]
    partial class MarketingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ATACK_Marketing_API.Models.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("EventDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("EventName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("VenueId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("EventId");

                    b.HasIndex("VenueId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.EventGuest", b =>
                {
                    b.Property<int>("EventGuestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("EventId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("EventGuestId");

                    b.HasIndex("EventId");

                    b.HasIndex("UserId");

                    b.ToTable("EventGuests");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.EventGuestSubscription", b =>
                {
                    b.Property<int>("EventGuestSubscriptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("EventGuestId")
                        .HasColumnType("int");

                    b.Property<int?>("EventVendorId")
                        .HasColumnType("int");

                    b.HasKey("EventGuestSubscriptionId");

                    b.HasIndex("EventGuestId");

                    b.HasIndex("EventVendorId");

                    b.ToTable("EventGuestSubscriptions");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.EventOrganizer", b =>
                {
                    b.Property<int>("EventOrganizerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("EventId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("EventOrganizerId");

                    b.HasIndex("EventId");

                    b.HasIndex("UserId");

                    b.ToTable("EventOrganizers");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.EventVendor", b =>
                {
                    b.Property<int>("EventVendorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("EventId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int?>("VendorId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("EventVendorId");

                    b.HasIndex("EventId");

                    b.HasIndex("VendorId");

                    b.ToTable("EventVendors");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.EventVendorUser", b =>
                {
                    b.Property<int>("EventVendorUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("EventVendorId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("EventVendorUserId");

                    b.HasIndex("EventVendorId");

                    b.HasIndex("UserId");

                    b.ToTable("EventVendorUsers");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("EventVendorId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("ProductPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ProductId");

                    b.HasIndex("EventVendorId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Uid")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.UserAudit", b =>
                {
                    b.Property<int>("UserAuditId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("EventDateTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("GrantPermission")
                        .HasColumnType("bit");

                    b.Property<string>("GranterEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GranterUid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModifiedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModifiedUid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PermissionType")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserAuditId");

                    b.ToTable("UserAudit");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.Vendor", b =>
                {
                    b.Property<int>("VendorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Website")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VendorId");

                    b.ToTable("Vendors");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.VendorAudit", b =>
                {
                    b.Property<int>("VendorAuditId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Detail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EventDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Operation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserUid")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VendorAuditId");

                    b.ToTable("VendorAudit");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.Venue", b =>
                {
                    b.Property<int>("VenueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("VenueName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Website")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VenueId");

                    b.ToTable("Venues");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.Event", b =>
                {
                    b.HasOne("ATACK_Marketing_API.Models.Venue", "Venue")
                        .WithMany()
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.EventGuest", b =>
                {
                    b.HasOne("ATACK_Marketing_API.Models.Event", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ATACK_Marketing_API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.EventGuestSubscription", b =>
                {
                    b.HasOne("ATACK_Marketing_API.Models.EventGuest", "EventGuest")
                        .WithMany("Subscriptions")
                        .HasForeignKey("EventGuestId");

                    b.HasOne("ATACK_Marketing_API.Models.EventVendor", "EventVendor")
                        .WithMany()
                        .HasForeignKey("EventVendorId");
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.EventOrganizer", b =>
                {
                    b.HasOne("ATACK_Marketing_API.Models.Event", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ATACK_Marketing_API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.EventVendor", b =>
                {
                    b.HasOne("ATACK_Marketing_API.Models.Event", "Event")
                        .WithMany("EventVendors")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ATACK_Marketing_API.Models.Vendor", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.EventVendorUser", b =>
                {
                    b.HasOne("ATACK_Marketing_API.Models.EventVendor", "EventVendor")
                        .WithMany()
                        .HasForeignKey("EventVendorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ATACK_Marketing_API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ATACK_Marketing_API.Models.Product", b =>
                {
                    b.HasOne("ATACK_Marketing_API.Models.EventVendor", "EventVendor")
                        .WithMany("Products")
                        .HasForeignKey("EventVendorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
