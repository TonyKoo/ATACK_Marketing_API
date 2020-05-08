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
    public class EventVendorUserController : ControllerBase {

        private readonly MarketingDbContext _context;

        public EventVendorUserController(MarketingDbContext context) {
            _context = context;
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find Users Account</response>   
        [SwaggerResponse(200, "Users List Of Event Vendors The Are Apart Of", typeof(EventVendorUserManagedViewModel))]
        [SwaggerOperation(
            Summary = "Gets Event Vendors User Is Assigned To",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult GetUserVendorsManaged() {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            //Retrieve Event Organizers For Event
            EventVendorUserRepository eventVendorUserRepo = new EventVendorUserRepository(_context);

            return Ok(eventVendorUserRepo.GetEventVendorUserList(theUser));
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find Users Account</response>   
        [SwaggerResponse(200, "List Of Event Vendor Users For The Event Vendor", typeof(EventVendorUserListViewModel))]
        [SwaggerOperation(
            Summary = "Gets List Of Users Managing The Event Vendor",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Event Organizer"
        )]
        [Produces("application/json")]
        [HttpGet("{eventVendorId}")]
        public IActionResult GetUserVendorsManaged(int eventVendorId) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify Event Vendor Exists
            EventVendor eventVendor = _context.EventVendors.FirstOrDefault(ev => ev.EventVendorId == eventVendorId);

            if (eventVendor == null) {
                return BadRequest(new { Message = "Event Vendor Doesn't Exist" });
            }

            //Verify Requesting User Valid and Has Rights
            User requestingUser = Retrieve.User(HttpContext.User, _context);
            if (!requestingUser.IsAdmin) {
                EventOrganizer eventOrganizer = _context.EventOrganizers.FirstOrDefault(eo => eo.User == requestingUser &&
                                                                                              eo.Event.EventId == eventVendor.Event.EventId);

                if (eventOrganizer == null) {
                    return StatusCode(403, new { Message = "Insufficient Permissions (Admin or Event Organizer)" });
                }
            }

            //Retrieve Event Organizers For Event
            EventVendorUserRepository eventVendorUserRepo = new EventVendorUserRepository(_context);

            return Ok(eventVendorUserRepo.GetEventVendorUsers(eventVendor));
        }

        /// <response code="400">User Already Has Permissions</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Rights To Modify Users</response>
        /// <response code="404">Cannot Find Users Account OR Event</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Event Vendor User Was Added To", typeof(EventVendorUserResultViewModel))]
        [SwaggerOperation(
            Summary = "Adds A User To Manage An Event Vendor",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Event Organizer<br>" +
                          "**Audited Function**<br>"
        )]
        [Produces("application/json")]
        [HttpPost("add")]
        public IActionResult AddVendorUser([FromBody] EventVendorUserInputViewModel vendorUserToAdd) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Check If Target User Is In DB
            String trimmedEmail = vendorUserToAdd.UserEmailToModify.Trim();
            User targetUser = _context.Users.FirstOrDefault(u => u.Email == trimmedEmail.ToLower());

            if (targetUser == null) {
                return NotFound(new { Message = "Target User Not Found In DB" });
            }

            //Check If Target Event Vendor Is Valid
            EventVendor eventVendor = _context.EventVendors.FirstOrDefault(ev => ev.EventVendorId == vendorUserToAdd.EventVendorId);

            if (eventVendor == null) {
                return NotFound(new { Message = "Target Event Vendor Id Not Found" });
            }

            //Check If Target User Is Already Assigned To The Event Vendor
            EventVendorUser eventVendorUser = _context.EventVendorUsers.FirstOrDefault(eo => eo.EventVendor == eventVendor &&
                                                                                             eo.User == targetUser);
            if (eventVendorUser != null) {
                return BadRequest(new { Message = "Target User Is Already An Event Vendor User For This Event Vendor" });
            }

            //Verify Requesting User Valid and Has Rights
            User requestingUser = Retrieve.User(HttpContext.User, _context);
            if (!requestingUser.IsAdmin) {
                EventOrganizer eventOrganizer = _context.EventOrganizers.FirstOrDefault(eo => eo.User == requestingUser &&
                                                                                              eo.Event.EventId == eventVendor.Event.EventId);

                if (eventOrganizer == null) {
                    return StatusCode(403, new { Message = "Insufficient Permissions (Admin or Event Organizer)" });
                }
            }

            //Grant Access To Event Vendor
            EventVendorUserRepository eventVendorUserRepo = new EventVendorUserRepository(_context);
            if (!eventVendorUserRepo.AddEventVendorUser(eventVendor, requestingUser, targetUser)) {
                return StatusCode(500, new { Message = "DB Error Has Occurred" });
            };

            return Ok(new EventVendorUserResultViewModel {
                EventName = eventVendor.Vendor.Name,
                UserEmailToModify = targetUser.Email,
                EventVendorId = eventVendor.EventVendorId,
                GrantedAccess = true
            });
        }

        /// <response code="400">User Already Has Permissions</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Rights To Modify Users</response>
        /// <response code="404">Cannot Find Users Account OR Event</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Removed Event Organizer And Event Information", typeof(EventVendorUserResultViewModel))]
        [SwaggerOperation(
            Summary = "Removes A User From Managing An Event Vendor",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Event Organizer<br>" +
                          "**Audited Function**<br>"
        )]
        [Produces("application/json")]
        [HttpDelete("remove")]
        public IActionResult RemoveEventVendorUser([FromBody] EventVendorUserInputViewModel vendorUserToRemove) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Check If Target User Is In DB
            String trimmedEmail = vendorUserToRemove.UserEmailToModify.Trim();
            User targetUser = _context.Users.FirstOrDefault(u => u.Email == trimmedEmail.ToLower());

            if (targetUser == null) {
                return NotFound(new { Message = "Target User Not Found In DB" });
            }

            //Check If Target Event Vendor Is Valid
            EventVendor eventVendor = _context.EventVendors.FirstOrDefault(ev => ev.EventVendorId == vendorUserToRemove.EventVendorId);

            if (eventVendor == null) {
                return NotFound(new { Message = "Target Event Vendor Id Not Found" });
            }

            //Check If Target User Is Already Assigned To The Event Vendor
            EventVendorUser eventVendorUser = _context.EventVendorUsers.FirstOrDefault(eo => eo.EventVendor == eventVendor &&
                                                                                             eo.User == targetUser);
            if (eventVendorUser == null) {
                return BadRequest(new { Message = "Target User Is Not An Event Vendor User For This Event Vendor" });
            }

            //Verify Requesting User Valid and Has Rights
            User requestingUser = Retrieve.User(HttpContext.User, _context);
            if (!requestingUser.IsAdmin) {
                EventOrganizer eventOrganizer = _context.EventOrganizers.FirstOrDefault(eo => eo.User == requestingUser &&
                                                                                              eo.Event.EventId == eventVendor.Event.EventId);

                if (eventOrganizer == null) {
                    return StatusCode(403, new { Message = "Insufficient Permissions (Admin or Event Organizer)" });
                }
            }

            //Remove Access To Event Vendor
            EventVendorUserRepository eventVendorUserRepo = new EventVendorUserRepository(_context);
            if (!eventVendorUserRepo.RemoveEventVendorUser(eventVendorUser, requestingUser, targetUser)) {
                return StatusCode(500, new { Message = "DB Error Has Occurred" });
            };

            return Ok(new EventVendorUserResultViewModel {
                EventName = eventVendor.Vendor.Name,
                UserEmailToModify = targetUser.Email,
                EventVendorId = eventVendor.EventVendorId,
                GrantedAccess = false
            });
        }
    }
}