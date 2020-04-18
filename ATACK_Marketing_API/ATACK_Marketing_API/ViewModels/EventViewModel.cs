using ATACK_Marketing_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.ViewModels {
    public class EventsViewModel {
        public int NumOfEvents { get; set; }
        public ICollection<EventDetailViewModel> Events { get; set; }
    }
    public class EventDetailViewModel {
        public int EventId { get; set; }
        public String EventName { get; set; }
        public DateTime DateTime { get; set; }
        public int NumOfVendors { get; set; }
        public Venue Venue { get; set; }
    }
}
