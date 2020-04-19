using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.Repositories;
using ATACK_Marketing_API.Utilities;
using ATACK_Marketing_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ATACK_Marketing_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase {
        private readonly MarketingDbContext _context;

        public EventsController(MarketingDbContext context) {
            _context = context;
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="400">User Not Registered In DB</response>   
        //[SwaggerResponse(200, "Users Email and Admin Privileges", typeof(UserViewModel))]
        //[SwaggerOperation(
        //    Summary = "Get the user account from the database",
        //    Description = "Requires Authentication"
        //)]
        [Produces("application/json")]
        [HttpGet(Name = "getevents")]
        public IActionResult GetEvents() { 
            if (!Validate.ValidUser(HttpContext.User, _context)) {
                return BadRequest(new { Message = "Unknown User" });
            }

            EventsRepository eventsRepo = new EventsRepository(_context);

            return Ok(eventsRepo.GetAllEvents());
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="400">User Not Registered In DB</response>   
        //[SwaggerResponse(200, "Users Email and Admin Privileges", typeof(UserViewModel))]
        //[SwaggerOperation(
        //    Summary = "Get the user account from the database",
        //    Description = "Requires Authentication"
        //)]
        [Produces("application/json")]
        [HttpGet("{eventId}", Name = "getevent")]
        public IActionResult GetEvent(int eventId) {
            if (!Validate.ValidUser(HttpContext.User, _context)) {
                return BadRequest(new { Message = "Unknown User" });
            }

            EventsRepository eventsRepo = new EventsRepository(_context);

            Event theEvent = eventsRepo.GetEvent(eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "No Event Found" });
            }

            return Ok(new EventDetailViewModel {
                EventId = theEvent.EventId,
                EventName = theEvent.EventName,
                EventStartDateTime = theEvent.EventDateTime,
                NumOfVendors = theEvent.EventVendors.Count,
                Venue = theEvent.Venue
            });
        }
    }
}