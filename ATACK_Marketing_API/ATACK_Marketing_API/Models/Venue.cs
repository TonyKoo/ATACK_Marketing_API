using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATACK_Marketing_API.Models {
    public class Venue {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VenueId { get; set; }
        public String VenueName { get; set; }
        public String Website { get; set; }
    }
}
