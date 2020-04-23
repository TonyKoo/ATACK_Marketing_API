using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.ViewModels {
    public class EventOrganizerInputViewModel {
        [Required]
        public int EventId { get; set; }
        [Required]
        public String UserEmailToModify { get; set; }
    }

    public class EventOrganizerResultViewModel {
        public int EventId { get; set; }
        public String EventName { get; set; }
        public String UserEmailToModify { get; set; }
        public bool GrantedAccess { get; set; }
    }

    public class EventOrganizerListViewModel {
        public int EventId { get; set; }
        public String EventName { get; set; }
        public DateTime EventStartDateTime { get; set; }
        public String VenueName { get; set; }
        public ICollection<EventOrganizerDetailViewModel> EventOrganizers { get; set; }
    }

    public class EventOrganizerDetailViewModel {
        public int EventOrganizerId { get; set; }
        public int UserId { get; set; }
        public String UserEmail { get; set; }
    }

    public class UserEventOrganizerViewModel {
        public int UserId { get; set; }
        public String UserEmail { get; set; }
        public ICollection<EventDetailViewModel> EventsOrganizing { get; set; }
    }
}
