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

        //User Examples
        public class UserViewModelExample : IExamplesProvider<UserViewModel> {
            public UserViewModel GetExamples() {
                return new UserViewModel {
                    Email = "Same_Same@Different.com",
                    IsAdmin = false
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
                                ProductName = "Java For Noobs"
                            },
                            new ProductMinViewModel {
                                ProductId = 2,
                                ProductName = "Opportunities To Earn Marks"
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
    }
}
