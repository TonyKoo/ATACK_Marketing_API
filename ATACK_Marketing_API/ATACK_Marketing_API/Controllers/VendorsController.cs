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
    [Route("api/Events/{eventId}/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase {
        private readonly MarketingDbContext _context;

        public VendorsController(MarketingDbContext context) {
            _context = context;
        }


        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Event Not Found</response>   
        [SwaggerResponse(200, "Event Vendors", typeof(EventVendorsViewModel))]
        [SwaggerOperation(
            Summary = "Gets List Of Vendors For An Event",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult GetEventVendors(int eventId) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            Event theEvent = _context.Events.FirstOrDefault(e => e.EventId == eventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Not Found" });
            }

            VendorsRepository vendorsRepo = new VendorsRepository(_context);

            return Ok(vendorsRepo.GetAllEventVendors(theEvent));
        }
 
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Event or Event Vendor Not Found</response>   
        [SwaggerResponse(200, "Vendor Detail and Product List", typeof(EventVendorViewModel))]
        [SwaggerOperation(
            Summary = "Gets Vendor Details and Product List",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpGet("{eventVendorId}")]
        public IActionResult GetEventVendor(int eventId, int eventVendorId) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
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

        /// <response code="400">User Not A Guest Of Event / Already Subscribed</response>   
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find User / Event / Event Vendor</response>
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Subscribed Vendor Information", typeof(EventSubscriptionViewModel))]
        [SwaggerOperation(
            Summary = "Subscribe An Event Guest To An Event Vendor",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpPost("{eventVendorId}/subscribe")]
        public IActionResult SubscribeToEventVendor(int eventId, int eventVendorId) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
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

        /// <response code="400">User Not A Guest Of Event / Already Subscribed</response>   
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find User / Event / Event Vendor</response>
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Unsubscribed Vendor Information", typeof(EventSubscriptionViewModel))]
        [SwaggerOperation(
            Summary = "Unsubscribe An Event Guest From An Event Vendor",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpPost("{eventVendorId}/unsubscribe")]
        public IActionResult UnsubscribeToEventVendor(int eventId, int eventVendorId) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
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