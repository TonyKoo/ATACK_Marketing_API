using System;
using System.Collections.Generic;
using System.Linq;
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
    public class EventOrganizerController : ControllerBase {
        private readonly MarketingDbContext _context;

        public EventOrganizerController(MarketingDbContext context) {
            _context = context;
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find Users Account</response>   
        [SwaggerResponse(200, "Users List Of Events They Are Organizing", typeof(UserEventOrganizerViewModel))]
        [SwaggerOperation(
            Summary = "Gets Events User Is Organizing",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult GetUserEventsOrganized() {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            } 

            //Retrieve Event Organizers For Event
            EventOrganizerRepository eventOrganizerRepo = new EventOrganizerRepository(_context);

            return Ok(eventOrganizerRepo.GetManagedEvents(theUser));
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Permissions To Modify Accounts</response>
        /// <response code="404">Cannot Find Users Account OR Event</response>   
        [SwaggerResponse(200, "List of Event Organizers For The Event", typeof(EventOrganizerListViewModel))]
        [SwaggerOperation(
            Summary = "Gets Event Organizers For An Event",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin"
        )]
        [Produces("application/json")]
        [HttpGet("{eventId}", Name = "geteventorganizer")]
        public IActionResult GetEventOrganizers(int eventId) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid And Admin
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            } else if (!theUser.IsAdmin) {
                return StatusCode(403, new { Message = "Insufficient Permissions To View Event Organizers" });
            }

            //Verify Event Is Valid
            Event theEvent = _context.Events.FirstOrDefault(e => e.EventId == eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Id Not Found" });
            }

            //Retrieve Event Organizers For Event
            EventOrganizerRepository eventOrganizerRepo = new EventOrganizerRepository(_context);

            return Ok(eventOrganizerRepo.GetEventOrganizers(theEvent));
        }


        /// <response code="400">User Already Has Permissions</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Rights To Modify Users</response>
        /// <response code="404">Cannot Find Users Account OR Event</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(201, "Added Event Organizer And Event Information", typeof(EventOrganizerResultViewModel))]
        [SwaggerOperation(
            Summary = "Adds A User To Manage Vendors For An Event (Event Organizer)",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin<br>" +
                          "**Audited Function**"
        )]
        [Produces("application/json")]
        [HttpPost("add")]
        public IActionResult AddEventOrganizer([FromBody] EventOrganizerInputViewModel eventOrganizerToAdd) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify Requesting User Valid and Has Admin Rights
            User requestingUser = Retrieve.User(HttpContext.User, _context);

            if (requestingUser == null) {
                return NotFound(new { Message = "Requesting User Not Found In DB" });
            } else if (!requestingUser.IsAdmin) {
                return StatusCode(403, new { Message = "Insufficient Permissions To Modify Users" });
            }

            //Check If Target User Is In DB
            String trimmedEmail = eventOrganizerToAdd.UserEmailToModify.Trim();
            User targetUser = _context.Users.FirstOrDefault(u => u.Email == trimmedEmail.ToLower());

            if (targetUser == null) {
                return NotFound(new { Message = "Target User Not Found In DB" });
            }

            //Check If Target Event Is Valid
            Event theEventToAdd = _context.Events.FirstOrDefault(e => e.EventId == eventOrganizerToAdd.EventId);

            if (theEventToAdd == null) {
                return NotFound(new { Message = "Target Event Id Not Found" });
            }

            //Check If Target User Is Already Organizer For This Event
            EventOrganizer eventOrganizer = _context.EventOrganizers.FirstOrDefault(eo => eo.Event.EventId == eventOrganizerToAdd.EventId &&
                                                                                          eo.User == targetUser);
            if (eventOrganizer != null) {
                return BadRequest(new { Message = "Target User Is Already An Event Organizer For This Event" });
            }

            EventOrganizerRepository eventOrganizerRepo = new EventOrganizerRepository(_context);

            //Grant Access To Event Organizer
            if (!eventOrganizerRepo.AddEventOrganizer(theEventToAdd, requestingUser, targetUser)) {
                return StatusCode(500, new { Message = "DB Error Has Occurred" });
            };

            return StatusCode(201, new EventOrganizerResultViewModel {
                EventId             = theEventToAdd.EventId,
                EventName           = theEventToAdd.EventName,
                UserEmailToModify   = targetUser.Email,
                GrantedAccess       = true
            });
        }

        /// <response code="400">User Not Event Organizer For This Event</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Rights To Modify Users</response>
        /// <response code="404">Cannot Find Users Account</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Removed Event Organizer And Event Information", typeof(EventOrganizerResultViewModel))]
        [SwaggerOperation(
            Summary = "Removes A User From Managing Vendors For An Event (Event Organizer)",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin<br>" +
                          "**Audited Function**"
        )]
        [Produces("application/json")]
        [HttpDelete("remove")]
        public IActionResult RemoveEventOrganizer([FromBody] EventOrganizerInputViewModel eventOrganizerToRemove) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify Requesting User Valid and Has Admin Rights
            User requestingUser = Retrieve.User(HttpContext.User, _context);

            if (requestingUser == null) {
                return NotFound(new { Message = "Requesting User Not Found In DB" });
            } else if (!requestingUser.IsAdmin) {
                return StatusCode(403, new { Message = "Insufficient Permissions To Modify Users" });
            }

            //Check If Target User Is In DB
            String trimmedEmail = eventOrganizerToRemove.UserEmailToModify.Trim();
            User targetUser = _context.Users.FirstOrDefault(u => u.Email == trimmedEmail.ToLower());

            if (targetUser == null) {
                return NotFound(new { Message = "Target User Not Found In DB" });
            }

            //Check If Target User Is Managing The Event
            EventOrganizer eventOrganizer = _context.EventOrganizers.FirstOrDefault(eo => eo.Event.EventId == eventOrganizerToRemove.EventId &&
                                                                                          eo.User == targetUser);
            if (eventOrganizer == null) {
                return BadRequest(new { Message = "Target User Is Not An Event Organizer For This Event" });
            }

            EventOrganizerRepository eventOrganizerRepo = new EventOrganizerRepository(_context);

            //Remove Access From Event Organizer
            if (!eventOrganizerRepo.RemoveEventOrganizer(eventOrganizer, requestingUser, targetUser)) {
                return StatusCode(500, new { Message = "DB Error Has Occurred" });
            };

            return Ok(new EventOrganizerResultViewModel {
                EventId = eventOrganizer.Event.EventId,
                EventName = eventOrganizer.Event.EventName,
                UserEmailToModify = targetUser.Email,
                GrantedAccess = false
            });
        }
    }
}