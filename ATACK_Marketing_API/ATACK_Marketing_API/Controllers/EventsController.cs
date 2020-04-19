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
                return NotFound(new { Message = "Event Not Found" });
            }

            return Ok(new EventDetailViewModel {
                EventId = theEvent.EventId,
                EventName = theEvent.EventName,
                EventStartDateTime = theEvent.EventDateTime,
                NumOfVendors = theEvent.EventVendors.Count,
                Venue = theEvent.Venue
            });
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="400">User Not Registered In DB</response>   
        //[SwaggerResponse(200, "Users Email and Admin Privileges", typeof(UserViewModel))]
        //[SwaggerOperation(
        //    Summary = "Get the user account from the database",
        //    Description = "Requires Authentication"
        //)]
        [Produces("application/json")]
        [HttpPost("{eventId}/join")]
        public IActionResult JoinEvent(int eventId) {
            if (!Validate.ValidUser(HttpContext.User, _context)) {
                return BadRequest(new { Message = "Unknown User" });
            }

            EventsRepository eventsRepo = new EventsRepository(_context);

            Event theEvent = eventsRepo.GetEvent(eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Not Found" });
            }

            EventGuestRepository eventGuestRepo = new EventGuestRepository(_context);

            string uid = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            User theUser = _context.Users.FirstOrDefault(u => u.Uid == uid);

            EventGuest eventToJoin = _context.EventGuests.FirstOrDefault(eg => eg.Event == theEvent && eg.User == theUser);

            if (eventToJoin != null) {
                return BadRequest(new { Message = "User Already Joined This Event" });
            }

            if (!eventGuestRepo.JoinEvent(theUser, theEvent)) {
                return StatusCode(500, new { Message = "Join Error (DB Issue)" });
            }

            return Ok(new EventGuestViewModel { 
                EventId = theEvent.EventId,
                EventName = theEvent.EventName,
                UserEmail = theUser.Email,
                Joined = true
            });
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="400">User Not Registered In DB</response>   
        //[SwaggerResponse(200, "Users Email and Admin Privileges", typeof(UserViewModel))]
        //[SwaggerOperation(
        //    Summary = "Get the user account from the database",
        //    Description = "Requires Authentication"
        //)]
        [Produces("application/json")]
        [HttpPost("{eventId}/leave")]
        public IActionResult LeaveEvent(int eventId) {
            if (!Validate.ValidUser(HttpContext.User, _context)) {
                return BadRequest(new { Message = "Unknown User" });
            }

            EventsRepository eventsRepo = new EventsRepository(_context);

            Event theEvent = eventsRepo.GetEvent(eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Not Found" });
            }

            EventGuestRepository eventGuestRepo = new EventGuestRepository(_context);

            string uid = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            User theUser = _context.Users.FirstOrDefault(u => u.Uid == uid);

            EventGuest eventToLeave = _context.EventGuests.FirstOrDefault(eg => eg.Event == theEvent && eg.User == theUser);

            if (eventToLeave == null) {
                return BadRequest(new { Message = "User Hasn't Joined This Event" });
            }

            if (!eventGuestRepo.LeaveEvent(eventToLeave)) {
                return StatusCode(500, new { Message = "Subscription Error (DB Issue)" });
            }

            return Ok(new EventGuestViewModel {
                EventId = theEvent.EventId,
                EventName = theEvent.EventName,
                UserEmail = theUser.Email,
                Joined = false
            });
        }
    }
}