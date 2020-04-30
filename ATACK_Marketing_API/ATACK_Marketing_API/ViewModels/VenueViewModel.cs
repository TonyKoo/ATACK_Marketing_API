using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.ViewModels {
    public class VenueViewModel {
        public int VenueId { get; set; }
        public String VenueName { get; set; }
        public String Website { get; set; }
    }

    public class VenueInputViewModel {
        [Required]
        public String VenueName { get; set; }
        [Required]
        public String Website { get; set; }
    }

    public class VenueDeleteInputViewModel {
        [Required]
        public String ConfirmDeleteVenue { get; set; }
    }
}
