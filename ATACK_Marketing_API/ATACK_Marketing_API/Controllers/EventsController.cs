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
using Swashbuckle.AspNetCore.Annotations;

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
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="400">User Not Registered In DB</response>   
        [SwaggerResponse(200, "Listing Of All Events", typeof(EventsViewModel))]
        [SwaggerOperation(
            Summary = "Gets List Of All Events",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpGet(Name = "getevents")]
        public IActionResult GetEvents() {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            EventsRepository eventsRepo = new EventsRepository(_context);

            return Ok(eventsRepo.GetAllEvents());
        }

        /// <response code="400">User Not Registered In DB</response>   
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        [SwaggerResponse(200, "The Event", typeof(EventDetailViewModel))]
        [SwaggerOperation(
            Summary = "Retrieve An Event",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpGet("{eventId}", Name = "getevent")]
        public IActionResult GetEvent(int eventId) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
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

        /// <response code="400">User Not Registered In DB</response>   
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="500">Database Error</response>   
        [SwaggerResponse(200, "Joined Event Information", typeof(EventGuestViewModel))]
        [SwaggerOperation(
            Summary = "Join User To An Event",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpPost("{eventId}/join")]
        public IActionResult JoinEvent(int eventId) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            //Verify Event Valid
            EventsRepository eventsRepo = new EventsRepository(_context);
            Event theEvent = eventsRepo.GetEvent(eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Not Found" });
            }

            //Verify User Not Already In Event
            EventGuestRepository eventGuestRepo = new EventGuestRepository(_context);
            EventGuest eventToJoin = _context.EventGuests.FirstOrDefault(eg => eg.Event == theEvent && eg.User == theUser);

            if (eventToJoin != null) {
                return BadRequest(new { Message = "User Already Joined This Event" });
            }

            //Join Event
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

        /// <response code="400">User Not Registered In DB</response>   
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="500">Database Error</response>   
        [SwaggerResponse(200, "Event Information", typeof(EventGuestViewModel))]
        [SwaggerOperation(
            Summary = "Remove User From Event",
            Description = "Requires Authentication<br>" +
                          "**NOTE:** This Will Automatically Unsubscribe User From Any Vendors They Subscribed To At The Event"
        )]
        [Produces("application/json")]
        [HttpPost("{eventId}/leave")]
        public IActionResult LeaveEvent(int eventId) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            //Verify Event Valid
            EventsRepository eventsRepo = new EventsRepository(_context);
            Event theEvent = eventsRepo.GetEvent(eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Not Found" });
            }

            //Verify User Already In Event
            EventGuestRepository eventGuestRepo = new EventGuestRepository(_context);
            EventGuest eventToLeave = _context.EventGuests.FirstOrDefault(eg => eg.Event == theEvent && eg.User == theUser);

            if (eventToLeave == null) {
                return BadRequest(new { Message = "User Hasn't Joined This Event" });
            }

            //Leave The Event
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

        ///// <response code="400">User Already Has Permissions / Cannot Modify Your Own Account</response>
        ///// <response code="401">Missing Authentication Token</response>
        ///// <response code="403">Users Email is Not Verified / Insufficient Rights To Modify Users</response>
        ///// <response code="404">Cannot Find Users Account</response>   
        ////[SwaggerResponse(200, "Users Email and Admin Privileges", typeof(UserViewModel))]
        ////[SwaggerOperation(
        ////    Summary = "Grants Admin Rights To A Specified User",
        ////    Description = "Requires Authentication"
        ////)]
        //[Produces("application/json")]
        //[HttpPost]
        //public IActionResult AddEvent([FromBody] EventAddViewModel eventToAdd) {
        //    if (!Validate.VerifiedUser(HttpContext.User)) {
        //        return StatusCode(403, new { Message = "Unverified User" });
        //    }

        //    //Verify Requesting User Valid and Has Admin Rights
        //    User requestingUser = Retrieve.User(HttpContext.User, _context);

        //    if (requestingUser == null) {
        //        return NotFound(new { Message = "Rquesting User Not Found In DB" });
        //    } else if (!requestingUser.IsAdmin) {
        //        return StatusCode(403, new { Message = "Insufficient Permissions To Modify Users" });
        //    }

        //    return Ok("getevent");
        //}
    }
}