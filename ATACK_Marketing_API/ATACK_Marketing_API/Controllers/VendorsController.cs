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
    [Route("api/Events/{eventId}/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase {
        private readonly MarketingDbContext _context;

        public VendorsController(MarketingDbContext context) {
            _context = context;
        }


        /// <response code="401">Missing Authentication Token</response>
        /// <response code="400">User Not Registered In DB</response>   
        /// <response code="404">Event Not Found</response>   
        //[SwaggerResponse(200, "Users Email and Admin Privileges", typeof(UserViewModel))]
        //[SwaggerOperation(
        //    Summary = "Get the user account from the database",
        //    Description = "Requires Authentication"
        //)]
        [Produces("application/json")]
        [HttpGet(Name = "getvendors")]
        public IActionResult GetEventVendors(int eventId) {
            if (!Validate.ValidUser(HttpContext.User, _context)) {
                return BadRequest(new { Message = "Unknown User" });
            }

            Event theEvent = _context.Events.FirstOrDefault(e => e.EventId == eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Not Found" });
            }

            VendorsRepository vendorsRepo = new VendorsRepository(_context);

            return Ok(vendorsRepo.GetAllEventVendors(theEvent));
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="400">User Not Registered In DB</response>   
        /// <response code="404">Event or Event Vendor Not Found</response>   
        //[SwaggerResponse(200, "Users Email and Admin Privileges", typeof(UserViewModel))]
        //[SwaggerOperation(
        //    Summary = "Get the user account from the database",
        //    Description = "Requires Authentication"
        //)]
        [Produces("application/json")]
        [HttpGet("{eventVendorId}", Name = "getvendor")]
        public IActionResult GetEventVendor(int eventId, int eventVendorId) {
            if (!Validate.ValidUser(HttpContext.User, _context)) {
                return BadRequest(new { Message = "Unknown User" });
            }

            Event theEvent = _context.Events.FirstOrDefault(e => e.EventId == eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Not Found" });
            }

            EventVendor theEventVendor = _context.EventVendors.FirstOrDefault(ev => ev.EventVendorId == eventVendorId);

            if (theEventVendor == null) {
                return NotFound(new { Message = "Event Vendor Not Found" });
            }

            VendorsRepository vendorsRepo = new VendorsRepository(_context);

            return Ok(vendorsRepo.GetEventVendor(theEvent, theEventVendor));
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="400">User Not Registered In DB</response>   
        //[SwaggerResponse(200, "Users Email and Admin Privileges", typeof(UserViewModel))]
        //[SwaggerOperation(
        //    Summary = "Get the user account from the database",
        //    Description = "Requires Authentication"
        //)]
        [Produces("application/json")]
        [HttpPost("{eventVendorId}/subscribe")]
        public IActionResult SubscribeToEventVendor(int eventId, int eventVendorId) {
            if (!Validate.ValidUser(HttpContext.User, _context)) {
                return BadRequest(new { Message = "Unknown User" });
            }

            //Verify Event Exists
            EventsRepository eventsRepo = new EventsRepository(_context);

            Event theEvent = eventsRepo.GetEvent(eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Not Found" });
            }

            //Check Event Vendor Exists
            EventVendor theEventVendor = _context.EventVendors.FirstOrDefault(ev => ev.EventVendorId == eventVendorId &&
                                                                                    ev.Event.EventId == eventId);

            if (theEventVendor == null) {
                return NotFound(new { Message = "Event Vendor Not Found" });
            }

            //Check User Joined Event
            EventGuestRepository eventGuestRepo = new EventGuestRepository(_context);

            string uid = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            User theUser = _context.Users.FirstOrDefault(u => u.Uid == uid);

            EventGuest theEventGuest = _context.EventGuests.FirstOrDefault(eg => eg.Event == theEvent && eg.User == theUser);

            if (theEventGuest == null) {
                return BadRequest(new { Message = "User Must Join Event First" });
            }

            //Subscribe User
            EventGuestSubscriptionRepository eventGuestSubRepo = new EventGuestSubscriptionRepository(_context);

            EventGuestSubscription theEventGuestSub = _context.EventGuestSubscriptions
                                                              .FirstOrDefault(egs => egs.EventVendor == theEventVendor &&
                                                                                     egs.EventGuest == theEventGuest);

            if (theEventGuestSub != null) {
                return BadRequest(new { Message = "User Already Subscribed To This Event Vendor" });
            }
            if (!eventGuestSubRepo.SubscribeToEventVendor(theEventGuest, theEventVendor)) {
                return StatusCode(500, new { Message = "Subscribe Error (DB Issue)" });
            }

            return Ok(new EventSubscriptionViewModel {
                EventId = theEvent.EventId,
                EventName = theEvent.EventName,
                EventVendorId = theEventVendor.EventVendorId,
                EventVendor = theEventVendor.Vendor.Name,
                UserEmail = theUser.Email,
                Subscribed = true
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
        [HttpPost("{eventVendorId}/unsubscribe")]
        public IActionResult UnsubscribeToEventVendor(int eventId, int eventVendorId) {
            if (!Validate.ValidUser(HttpContext.User, _context)) {
                return BadRequest(new { Message = "Unknown User" });
            }

            //Verify Event Exists
            EventsRepository eventsRepo = new EventsRepository(_context);

            Event theEvent = eventsRepo.GetEvent(eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Not Found" });
            }

            //Check Event Vendor Exists
            EventVendor theEventVendor = _context.EventVendors.FirstOrDefault(ev => ev.EventVendorId == eventVendorId &&
                                                                                    ev.Event.EventId == eventId);

            if (theEventVendor == null) {
                return NotFound(new { Message = "Event Vendor Not Found" });
            }

            //Check User Joined Event
            EventGuestRepository eventGuestRepo = new EventGuestRepository(_context);

            string uid = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            User theUser = _context.Users.FirstOrDefault(u => u.Uid == uid);

            EventGuest theEventGuest = _context.EventGuests.FirstOrDefault(eg => eg.Event == theEvent && eg.User == theUser);

            if (theEventGuest == null) {
                return BadRequest(new { Message = "User Must Join Event First" });
            }

            //Unsubscribe User
            EventGuestSubscriptionRepository eventGuestSubRepo = new EventGuestSubscriptionRepository(_context);

            EventGuestSubscription theEventGuestSub = _context.EventGuestSubscriptions
                                                                       .FirstOrDefault(egs => egs.EventVendor == theEventVendor &&
                                                                                              egs.EventGuest == theEventGuest);
            if (theEventGuestSub == null) {
                return BadRequest(new { Message = "User Not Subscribed To This Event Vendor" });
            }
            if (!eventGuestSubRepo.UnsubscribeEventVendor(theEventGuestSub)) {
                return StatusCode(500, new { Message = "Unsubscribe Error (DB Issue)" });
            }

            return Ok(new EventSubscriptionViewModel {
                EventId = theEvent.EventId,
                EventName = theEvent.EventName,
                EventVendor = theEventVendor.Vendor.Name,
                UserEmail = theUser.Email,
                Subscribed = false
            });
        }

    }
}