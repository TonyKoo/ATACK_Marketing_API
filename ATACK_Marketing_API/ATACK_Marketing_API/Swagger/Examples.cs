using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.ViewModels;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Swagger {
    public class Examples {

        //Events Examples

        //========= Input =========
        public class EventAddModifyViewModelExample : IExamplesProvider<EventAddModifyViewModel> {
            public EventAddModifyViewModel GetExamples() {
                return new EventAddModifyViewModel {
                    EventName = "Dogz and Catz",
                    EventStartDateTime = new DateTime(2020, 08, 22, 19, 30, 00),
                    VenueId = 1
                };
            }
        }

        public class EventDeleteInputViewModelExample : IExamplesProvider<EventDeleteInputViewModel> {
            public EventDeleteInputViewModel GetExamples() {
                return new EventDeleteInputViewModel {
                    DeleteConfirmation = "ConfirmDELETE - <Event Name>"
                };
            }
        }

        public class EventsViewModelExample : IExamplesProvider<EventsViewModel> {
            public EventsViewModel GetExamples() {
                return new EventsViewModel {
                    NumOfEvents = 2,
                    Events = new List<EventDetailViewModel> {
                        new EventDetailViewModel {
                            EventId = 1,
                            EventName = "I Know I Know",
                            EventStartDateTime = new DateTime(2020, 03, 22, 17, 30, 00),
                            NumOfVendors = 5,
                            Venue = new Venue {
                                VenueId = 1,
                                VenueName = "Rogers Arena",
                                Website = "https://rogersarena.com/"
                            }
                        },
                        new EventDetailViewModel {
                            EventId = 2,
                            EventName = "Same Same But Different",
                            EventStartDateTime = new DateTime(2020, 07, 11, 12, 00, 00),
                            NumOfVendors = 5,
                            Venue = new Venue {
                                VenueId = 2,
                                VenueName = "Vancouver Convention Centre",
                                Website = "https://www.vancouverconventioncentre.com/"
                            }
                        }
                    }
                };
            }
        }

        public class EventDetailViewModelExample : IExamplesProvider<EventDetailViewModel> {
            public EventDetailViewModel GetExamples() {
                return new EventDetailViewModel {
                    EventId = 1,
                    EventName = "I Know I Know",
                    EventStartDateTime = new DateTime(2020, 03, 22, 17, 30, 00),
                    NumOfVendors = 5,
                    Venue = new Venue {
                        VenueId = 1,
                        VenueName = "Rogers Arena",
                        Website = "https://rogersarena.com/"
                    }
                };
            }
        }

        public class EventGuestViewModelExample : IExamplesProvider<EventGuestViewModel> {
            public EventGuestViewModel GetExamples() {
                return new EventGuestViewModel {
                    EventId = 1,
                    UserEmail = "Gina@cat.com",
                    EventName = "I Know I Know",
                    Joined = true
                };
            }
        }

        // ========== User Examples ==========
        // ========== Responses ==========
        public class UserViewModelExample : IExamplesProvider<UserViewModel> {
            public UserViewModel GetExamples() {
                return new UserViewModel {
                    Email = "Same_Same@Different.com",
                    IsAdmin = false,
                    IsEventOrganizer = false,
                    IsVendor = false
                };
            }
        }

        public class UserEventListExample : IExamplesProvider<UserEventListViewModel> {
            public UserEventListViewModel GetExamples() {
                return new UserEventListViewModel {
                    UserEmail = "Same_Same@Different.com",
                    EventsJoined = new List<UserEventListEventDetailViewModel> { 
                        new UserEventListEventDetailViewModel {
                            EventId = 1,
                            EventName = "I Know I Know",
                            EventStartDateTime = new DateTime(2020, 03, 22, 17, 30, 00)
                        },
                        new UserEventListEventDetailViewModel {
                            EventId = 2,
                            EventName = "Coffee Snobs",
                            EventStartDateTime = new DateTime(2020, 07, 11, 12, 00, 00)
                        }
                    }
                };
            }
        }

        public class EventSubscriptionSummaryExample : IExamplesProvider<EventSubscriptionSummaryViewModel> {
            public EventSubscriptionSummaryViewModel GetExamples() {
                return new EventSubscriptionSummaryViewModel {
                    UserEmail = "Same_Same@Different.com",
                    Subscriptions = new List<EventSubscriptionDetailViewModel> {
                        new EventSubscriptionDetailViewModel {
                            EventId = 1,
                            EventName = "I Know I Know",
                            EventStartDateTime = new DateTime(2020, 03, 22, 17, 30, 00),
                            Venue = new Venue{
                                VenueId = 1,
                                VenueName = "Rogers Arena",
                                Website = "https://rogersarena.ca"
                            },
                            EventSubscriptions = new List<EventSubscriptionVendorDetailViewModel> {
                                new EventSubscriptionVendorDetailViewModel {
                                    EventVendorId = 1,
                                    VendorName = "Tong Enterprises"
                                },
                                new EventSubscriptionVendorDetailViewModel {
                                    EventVendorId = 4,
                                    VendorName = "Maple Leaf Foods"
                                }
                            }
                        },
                        new EventSubscriptionDetailViewModel {
                            EventId = 2,
                            EventName = "Coffee Snobs",
                            EventStartDateTime = new DateTime(2020, 07, 11, 12, 00, 00),
                            EventSubscriptions = new List<EventSubscriptionVendorDetailViewModel> {
                                new EventSubscriptionVendorDetailViewModel {
                                    EventVendorId = 21,
                                    VendorName = "Keurig Canada"
                                },
                                new EventSubscriptionVendorDetailViewModel {
                                    EventVendorId = 24,
                                    VendorName = "Nabob"
                                },
                                new EventSubscriptionVendorDetailViewModel {
                                    EventVendorId = 20,
                                    VendorName = "Tim Hortons"
                                }
                            }
                        }
                    }
                };
            }
        }

        // ========== Inputs ==========
        public class UserAdminInputViewModelExample : IExamplesProvider<UserAdminInputViewModel> {
            public UserAdminInputViewModel GetExamples() {
                return new UserAdminInputViewModel {
                    UserEmailToModify = "@"
                };
            }
        }

        //Vendor Examples

        public class EventVendorsViewModelExample : IExamplesProvider<EventVendorsViewModel> {
            public EventVendorsViewModel GetExamples() {
                return new EventVendorsViewModel {
                    EventId = 1,
                    EventName = "I Know I Know",
                    EventStartDateTime = new DateTime(2020, 03, 22, 17, 30, 00),
                    NumOfEventVendors = 3,
                    Vendors = new List<EventVendorMinDetailViewModel> {
                        new EventVendorMinDetailViewModel {
                            EventVendorId = 1,
                            VendorName = "Tong Enterprises",
                            NumOfProducts = 2
                        },
                        new EventVendorMinDetailViewModel {
                            EventVendorId = 2,
                            VendorName = "Keurig Canada",
                            NumOfProducts = 1
                        },
                        new EventVendorMinDetailViewModel {
                            EventVendorId = 3,
                            VendorName = "BCIT",
                            NumOfProducts = 1
                        },
                    }
                };
            }
        }

        public class EventVendorViewModelExample : IExamplesProvider<EventVendorViewModel> {
            public EventVendorViewModel GetExamples() {
                return new EventVendorViewModel {
                    EventId = 1,
                    EventName = "I Know I Know",
                    EventStartDateTime = new DateTime(2020, 03, 22, 17, 30, 00),
                    Vendor = new EventVendorDetailViewModel {
                        EventVendorId = 1,
                        VendorName = "Tong Enterprises",
                        Description = "More Opportunities To Earn Money",
                        Email = "Mr.Tong@IKnowIKnow.com",
                        Website = "https://www.tong-enterprises.ca/",
                        NumOfProducts = 2,
                        Products = new List<ProductMinViewModel> {
                            new ProductMinViewModel {
                                ProductId = 1,
                                ProductName = "Java For Noobs",
                                ProductPrice = 54.95m
                            },
                            new ProductMinViewModel {
                                ProductId = 2,
                                ProductName = "Opportunities To Earn Marks",
                                ProductPrice = 109.99m
                            }
                        }
                    }
                };
            }
        }

        public class EventSubscriptionViewModelExample : IExamplesProvider<EventSubscriptionViewModel> {
            public EventSubscriptionViewModel GetExamples() {
                return new EventSubscriptionViewModel {
                    EventId = 1,
                    UserEmail = "Gina@cat.com",
                    EventName = "I Know I Know",
                    EventVendorId = 1,
                    EventVendor = "Tong Enterprises",
                    Subscribed = true
                };
            }
        }

        //Event Organizer Examples
        //========== Input ==========
        public class EventOrganizerViewModelExample : IExamplesProvider<EventOrganizerInputViewModel> {
            public EventOrganizerInputViewModel GetExamples() {
                return new EventOrganizerInputViewModel {
                    EventId = 1,
                    UserEmailToModify = "@"
                };
            }
        }

        //========== Results ==========

        public class EventOrganizerResultViewModelExample : IExamplesProvider<EventOrganizerResultViewModel> {
            public EventOrganizerResultViewModel GetExamples() {
                return new EventOrganizerResultViewModel {
                    EventId = 1,
                    EventName = "I Know I Know",
                    UserEmailToModify = "Albert@rocks.com",
                    GrantedAccess = true
                };
            }
        }

        public class EventOrganizerListViewModelExample : IExamplesProvider<EventOrganizerListViewModel> {
            public EventOrganizerListViewModel GetExamples() {
                return new EventOrganizerListViewModel {
                    EventId = 1,
                    EventName = "I Know I Know",
                    EventStartDateTime = new DateTime(2020, 03, 22, 17, 30, 00),
                    VenueName = "Rogers Arena",
                    EventOrganizers = new List<EventOrganizerDetailViewModel> {
                        new EventOrganizerDetailViewModel {
                            EventOrganizerId = 5,
                            UserId = 42,
                            UserEmail = "Athena@amazing.com"
                        },
                        new EventOrganizerDetailViewModel {
                            EventOrganizerId = 7,
                            UserId = 21,
                            UserEmail = "Crystal@github-rules.com"
                        },
                    }
                };
            }
        }

        public class UserEventOrganizerViewModelExample : IExamplesProvider<UserEventOrganizerViewModel> {
            public UserEventOrganizerViewModel GetExamples() {
                return new UserEventOrganizerViewModel {
                    UserId = 5,
                    UserEmail = "Ahmed@camel.com",
                    EventsOrganizing = new List<EventDetailViewModel> {
                        new EventDetailViewModel {
                            EventId = 1,
                            EventName = "I Know I Know",
                            EventStartDateTime = new DateTime(2020, 03, 22, 17, 30, 00),
                            NumOfVendors = 5,
                            Venue = new Venue {
                                VenueId = 1,
                                VenueName = "Rogers Arena",
                                Website = "https://rogersarena.com/"
                            }
                        },
                        new EventDetailViewModel {
                            EventId = 2,
                            EventName = "Same Same But Different",
                            EventStartDateTime = new DateTime(2020, 07, 11, 12, 00, 00),
                            NumOfVendors = 5,
                            Venue = new Venue {
                                VenueId = 2,
                                VenueName = "Vancouver Convention Centre",
                                Website = "https://www.vancouverconventioncentre.com/"
                            }
                        }
                    }
                };
            }
        }

        // Vendor Management
        public class VendorAddModifyViewModelExample : IExamplesProvider<VendorAddModifyViewModel> {
            public VendorAddModifyViewModel GetExamples() {
                return new VendorAddModifyViewModel {
                    Name = "7-11 Canada",
                    Description = "At 7-Eleven Canada, we never close. We pride ourselves on being your neighbourhood go-to store - 24/7/365.",
                    Email = "ask.slurpee.eh@7-11.com",
                    Website = "https://7-eleven.ca/"
                };
            }
        }

        public class VendorExample : IExamplesProvider<Vendor> {
            public Vendor GetExamples() {
                return new Vendor {
                    VendorId = 1,
                    Name = "7-11 Canada",
                    Description = "At 7-Eleven Canada, we never close. We pride ourselves on being your neighbourhood go-to store - 24/7/365.",
                    Email = "ask.slurpee.eh@7-11.com",
                    Website = "https://7-eleven.ca/"
                };
            }
        }

        public class VendorManagementViewModelExample : IExamplesProvider<VendorManagementViewModel> {
            public VendorManagementViewModel GetExamples() {
                return new VendorManagementViewModel {
                    VendorId = 1,
                    Name = "7-11 Canada"
                };
            }
        }
        
        public class VendorDeleteViewModelExample : IExamplesProvider<VendorInputDeleteViewModel> {
            public VendorInputDeleteViewModel GetExamples() {
                return new VendorInputDeleteViewModel {
                    ConfirmDeleteName = "ConfirmDELETE - <Vendor Name>"
                };
            }
        }

        //Event Vendor 
        public class EventVendorAddRemoveViewModelExample : IExamplesProvider<EventVendorAddRemoveViewModel> {
            public EventVendorAddRemoveViewModel GetExamples() {
                return new EventVendorAddRemoveViewModel {
                    EventId = 1,
                    VendorId = 1
                };
            }
        }

        public class ProductInputViewModelExample : IExamplesProvider<ProductInputViewModel> {
            public ProductInputViewModel GetExamples() {
                return new ProductInputViewModel {
                    ProductName = "Apple Pie",
                    ProductPrice = 7.99m
                };
            }
        }

        public class EventVendorRemoveInputViewModelExample : IExamplesProvider<EventVendorRemoveInputViewModel> {
            public EventVendorRemoveInputViewModel GetExamples() {
                return new EventVendorRemoveInputViewModel {
                    DeleteVendorString = "<Event Name> - <Vendor Name>"
                };
            }
        }

        // ========= Result =========
        public class EventVendorResultViewModelExample : IExamplesProvider<EventVendorResultViewModel> {
            public EventVendorResultViewModel GetExamples() {
                return new EventVendorResultViewModel {
                    EventVendorId = 1,
                    EventId = 1,
                    EventName = "I Know I Know",
                    EventStartDateTime = new DateTime(2020, 03, 22, 17, 30, 00),
                    VendorId = 2,
                    VendorName = "Amazon",
                    IsEventVendor = true
                };
            }
        }

        public class ProductRetrieveViewModelExample : IExamplesProvider<ProductRetrieveViewModel> {
            public ProductRetrieveViewModel GetExamples() {
                return new ProductRetrieveViewModel {
                    ProductId = 21,
                    ProductName = "Slurpee",
                    ProductPrice = 2.99m,
                    EventId = 1,
                    EventName = "I Know I Know",
                    EventVendorId = 7,
                    EventVendorName = "7-11 Canada"
                };
            }
        }

        

        //Event Vendor User
        public class EventVendorUserManagedViewModelExample : IExamplesProvider<EventVendorUserManagedViewModel> {
            public EventVendorUserManagedViewModel GetExamples() {
                return new EventVendorUserManagedViewModel {
                    UserEmail = "Athena@rocks.com",
                    UserEventVendors = new List<EventVendorUserManagedDetailViewModel> {
                        new EventVendorUserManagedDetailViewModel {
                            EventVendorId = 1,
                            VendorId = 5,
                            VendorName = "Tong Enterprises",
                            EventId = 1,
                            EventName = "I Know I Know",
                            EventStartDateTime = new DateTime(2020, 03, 22, 17, 30, 00),
                            Venue = new Venue {
                                VenueId = 1,
                                VenueName = "Rogers Arena",
                                Website = "https://rogersarena.com/"
                            }
                        },
                        new EventVendorUserManagedDetailViewModel {
                            EventVendorId = 5,
                            VendorId = 10,
                            VendorName = "Petcetera",
                            EventId = 2,
                            EventName = "Catz & Dogz",
                            EventStartDateTime = new DateTime(2020, 05, 28, 18, 30, 00),
                            Venue = new Venue {
                                VenueId = 2,
                                VenueName = "Vancouver Convention Centre",
                                Website = "https://www.vancouverconventioncentre.com/"
                            }
                        }
                    }
                };
            }
        }

    public class EventVendorUserListViewModelExample : IExamplesProvider<EventVendorUserListViewModel> {
            public EventVendorUserListViewModel GetExamples() {
                return new EventVendorUserListViewModel {
                    EventVendorId = 3,
                    VendorName = "7-11 Canada",
                    EventId = 1,
                    EventName = "I Know I Know",
                    VendorUsers = new List<EventVendorUserDetailViewModel> {
                        new EventVendorUserDetailViewModel {
                            UserEmail = "Ahmed@camel.com"
                        },
                        new EventVendorUserDetailViewModel {
                            UserEmail = "Albert@thebest.com"
                        }
                    }
                };
            }
        }

        

        public class EventVendorUserInputViewModelExample : IExamplesProvider<EventVendorUserInputViewModel> {
            public EventVendorUserInputViewModel GetExamples() {
                return new EventVendorUserInputViewModel {
                    EventVendorId = 1,
                    UserEmailToModify = "@"
                };
            }
        }

        public class EventVendorUserResultViewModelExample : IExamplesProvider<EventVendorUserResultViewModel> {
            public EventVendorUserResultViewModel GetExamples() {
                return new EventVendorUserResultViewModel {
                    EventName = "I Know I Know",
                    UserEmailToModify = "Crystal@catz.com",
                    EventVendorId = 12,
                    GrantedAccess = false
                };
            }
        }

        //Reports
        public class VendorSubscriberReportViewModelExample : IExamplesProvider<VendorSubscriberReportViewModel> {
            public VendorSubscriberReportViewModel GetExamples() {
                return new VendorSubscriberReportViewModel {
                    EventId = 1,
                    EventName = "I Know I Know",
                    EventStartDateTime = new DateTime(2020, 03, 22, 17, 30, 00),
                    EventVendorId = 1,
                    VendorName = "Tong Enterprises",
                    Subscribers = new List<VendorSubscriberDetailViewModel> {
                        new VendorSubscriberDetailViewModel {
                            UserEmail = "Ahmed@camel.com"
                        },
                        new VendorSubscriberDetailViewModel {
                            UserEmail = "Tony@tiger.com"
                        }
                    }

                };
            }
        }

        //Venue
        public class VenueViewModelExample : IExamplesProvider<VenueViewModel> {
            public VenueViewModel GetExamples() {
                return new VenueViewModel {
                    VenueId = 1, 
                    VenueName = "Rogers Arena",
                    Website = "https://rogersarena.com/"
                };
            }
        }

        public class VenueInputViewModelExample : IExamplesProvider<VenueInputViewModel> {
            public VenueInputViewModel GetExamples() {
                return new VenueInputViewModel {
                    VenueName = "Rogers Arena",
                    Website = "https://rogersarena.com/"
                };
            }
        }

        public class VenueDeleteInputViewModelExample : IExamplesProvider<VenueDeleteInputViewModel> {
            public VenueDeleteInputViewModel GetExamples() {
                return new VenueDeleteInputViewModel {
                    ConfirmDeleteVenue = "ConfirmDELETE - <Venue Name>"
                };
            }
        }

    }
}
