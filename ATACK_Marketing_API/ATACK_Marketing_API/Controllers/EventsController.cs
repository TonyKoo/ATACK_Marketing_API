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

        /// <response code="400">User Alredy Joined Event</response>   
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

        /// <response code="400">User Hasn't Joined Event</response>   
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

        /// <response code="400">Invalid Venue</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Rights To Modify Users</response>
        /// <response code="404">Cannot Find Users Account</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(201, "Added Event Information", typeof(EventDetailViewModel))]
        [SwaggerOperation(
            Summary = "Adds An Event",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin<br>" +
                          "**Audited Function**"
        )]
        [Produces("application/json")]
        [HttpPost("add")]
        public IActionResult AddEvent([FromBody] EventAddModifyViewModel eventToAdd) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify Requesting User Valid and Has Admin Rights
            User requestingUser = Retrieve.User(HttpContext.User, _context);

            if (requestingUser == null) {
                return NotFound(new { Message = "Requesting User Not Found In DB" });
            } else if (!requestingUser.IsAdmin) {
                return StatusCode(403, new { Message = "Insufficient Permissions To Add Events (Admin)" });
            }

            //Verify Venue Is Valid
            Venue theVenue = _context.Venues.FirstOrDefault(v => v.VenueId == eventToAdd.VenueId);

            if (theVenue == null) {
                return BadRequest(new { Message = "Invalid Venue Specified" });
            }

            EventsRepository eventRepo = new EventsRepository(_context);

            (bool isSuccessful, Event newEvent) = eventRepo.AddEvent(requestingUser, eventToAdd, theVenue);
            if (!isSuccessful) {
                return StatusCode(500, new { Message = "Add Event Error (DB Issue)" });
            }

            return StatusCode(201, new EventDetailViewModel { 
                EventId = newEvent.EventId,
                EventName = newEvent.EventName,
                EventStartDateTime = newEvent.EventDateTime,
                NumOfVendors = 0,
                Venue = newEvent.Venue
            });
        }

        /// <response code="400">Invalid Venue</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Rights To Modify Users</response>
        /// <response code="404">Cannot Find Users Account OR Event</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Updated Event Information", typeof(EventDetailViewModel))]
        [SwaggerOperation(
            Summary = "Updates An Event",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin<br>" +
                          "**Audited Function**"
        )]
        [Produces("application/json")]
        [HttpPut("update/{eventId}")]
        public IActionResult UpdateEvent(int eventId, [FromBody] EventAddModifyViewModel eventToUpdate) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify Requesting User Valid and Has Admin Rights
            User requestingUser = Retrieve.User(HttpContext.User, _context);

            if (requestingUser == null) {
                return NotFound(new { Message = "Requesting User Not Found In DB" });
            } else if (!requestingUser.IsAdmin) {
                return StatusCode(403, new { Message = "Insufficient Permissions To Add Events (Admin)" });
            }

            //Verify Event Exists
            EventsRepository eventRepo = new EventsRepository(_context);
            Event theEvent = eventRepo.GetEvent(eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Not Found" });
            }

            //Verify Venue Is Valid (If Changed)
            Venue theVenue = theEvent.Venue;
            if (theVenue.VenueId != eventToUpdate.VenueId) {
                theVenue = _context.Venues.FirstOrDefault(v => v.VenueId == eventToUpdate.VenueId);

                if (theVenue == null) {
                    return BadRequest(new { Message = "Invalid Venue Specified" });
                }
            } 

            //Update Event
            if (!eventRepo.UpdateEvent(requestingUser, theEvent, eventToUpdate, theVenue)) {
                return StatusCode(500, new { Message = "Update Event Error (DB Issue)" });
            }

            return Ok(new EventDetailViewModel {
                EventId = theEvent.EventId,
                EventName = theEvent.EventName,
                EventStartDateTime = theEvent.EventDateTime,
                NumOfVendors = theEvent.EventVendors.Count,
                Venue = theEvent.Venue
            });
        }

        /// <response code="400">User Already Has Permissions / Vendors or Event Organizers Attached To Event</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Rights To Modify Users</response>
        /// <response code="404">Cannot Find Users Account OR Event</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Removed Event Information", typeof(EventDetailViewModel))]
        [SwaggerOperation(
            Summary = "Removes An Event",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin<br>" +
                          "**Audited Function**<br>" +
                          "**Note:** You Cannot Remove An Event Once Event Vendors Are Attached / Users Subscribed" 
        )]
        [Produces("application/json")]
        [HttpDelete("remove/{eventId}")]
        public IActionResult RemoveEvent(int eventId, [FromBody] EventDeleteInputViewModel deleteConfirm) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify Requesting User Valid and Has Admin Rights
            User requestingUser = Retrieve.User(HttpContext.User, _context);

            if (requestingUser == null) {
                return NotFound(new { Message = "Requesting User Not Found In DB" });
            } else if (!requestingUser.IsAdmin) {
                return StatusCode(403, new { Message = "Insufficient Permissions To Add Events (Admin)" });
            }

            //Verify Event Exists
            EventsRepository eventRepo = new EventsRepository(_context);
            Event theEvent = eventRepo.GetEvent(eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Not Found" });
            }

            //Verify Confirm Delete
            if (!(deleteConfirm.DeleteConfirmation == $"ConfirmDELETE - {theEvent.EventName}")) {
                return BadRequest(new { Message = "Event Delete String Invalid" });
            }

            //Check Event Vendors
            EventVendorRepository eventVendorRepo = new EventVendorRepository(_context);
            if (eventVendorRepo.GetVendorsAttchedToEventCount(eventId) > 0) {
                return BadRequest(new { Message = "Event Has Vendors Attached" });
            }

            //Check Event Organizer
            EventOrganizerRepository eventOrganizerRepo = new EventOrganizerRepository(_context);
            if (eventOrganizerRepo.GetEventOrganizersCount(eventId) > 0) {
                return BadRequest(new { Message = "Event Has Event Organizers Attached" });
            }

            //Remove Event
            if (!eventRepo.RemoveEvent(requestingUser, theEvent)) {
                return StatusCode(500, new { Message = "Remove Event Error (DB Issue)" });
            }

            return Ok(new EventDetailViewModel {
                EventId = theEvent.EventId,
                EventName = theEvent.EventName,
                EventStartDateTime = theEvent.EventDateTime,
                NumOfVendors = 0,
                Venue = theEvent.Venue
            });
        }
    }
}