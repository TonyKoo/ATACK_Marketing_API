using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.ViewModels {

    public class EventGuestViewModel {
        public int EventId { get; set; }
        public String UserEmail { get; set; }
        public String EventName { get; set; }
        public bool Joined { get; set; }
    }

    public class UserEventListViewModel {
        public String UserEmail { get; set; }
        public ICollection<UserEventListEventDetailViewModel> EventsJoined { get; set; }
    }
    
    public class UserEventListEventDetailViewModel {
        public int EventId { get; set; }
        public String EventName { get; set; }
        public DateTime EventStartDateTime { get; set; }
    }
}
