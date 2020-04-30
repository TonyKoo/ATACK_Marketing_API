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
    public class UserController : ControllerBase {
        private readonly MarketingDbContext _context;

        public UserController(MarketingDbContext context) {
            _context = context;
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find Users Account</response>   
        [SwaggerResponse(200, "Users Email, Admin Privileges and Event/Vendor Membership Counts", typeof(UserViewModel))]
        [SwaggerOperation(
            Summary = "Gets the user account from the database",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpGet(Name = "getuser")]
        public IActionResult GetUserAccount() {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            return Ok(new UserViewModel {
                Email = theUser.Email,
                IsAdmin = theUser.IsAdmin,
                EventsOrganizing = _context.EventOrganizers.Where(eo => eo.User == theUser).Count(),
                VendorsManaged = _context.EventVendorUsers.Where(evu => evu.User == theUser).Count()
            });
        }

        /// <response code="400">User Already Exists</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="500">Database/Server Error</response>   
        [SwaggerResponse(200, "User Account Created Successfully", typeof(UserViewModel))]
        [SwaggerOperation(
            Summary = "Creates the user account in the database",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpPost("create")]
        public IActionResult CreateUserAccount() {
            var currentUser = HttpContext.User;
            string userEmail = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            string uid = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            //if (!Validate.VerifiedUser(currentUser)) {
            //    return StatusCode(403, new { Message = "Unverified User" });
            //}

            //Check For Duplicate Email First
            User theUser = _context.Users.FirstOrDefault(u => u.Email.ToLower() == userEmail.ToLower());
            if (theUser != null) {
                return BadRequest(new { Message = "Email Address Already Exists" });
            }

            //Check if Uid Already Exists
            theUser = Retrieve.User(currentUser, _context);
            if (theUser != null) {
                return BadRequest(new { Message = "User (Uid) Already Exists" });
            }

            //Add User
            UserRepository userRepo = new UserRepository(_context);

            if (!userRepo.CreateUser(uid, userEmail)) {
                return StatusCode(500, new { Message = "DB Error" });
            }

            return CreatedAtRoute("getuser",
                                  new UserViewModel {
                                      Email = userEmail,
                                      IsAdmin = false,
                                      EventsOrganizing = 0,
                                      VendorsManaged = 0
                                  });
        }

        /// <response code="400">User Already Has Permissions / Cannot Modify Your Own Account</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Rights To Modify Users</response>
        /// <response code="404">Cannot Find Users Account</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Users Email and Admin Privileges", typeof(UserViewModel))]
        [SwaggerOperation(
            Summary = "Grants Admin Rights To A Specified User",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin<br>" +
                          "**Audited Function**<br>"
        )]
        [Produces("application/json")]
        [HttpPut("elevate")]
        public IActionResult ElevateUser([FromBody] UserAdminInputViewModel emailToElevate) {
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


            //Verify User Is Not Themselves
            String trimmedEmail = emailToElevate.UserEmailToModify.Trim();

            if (requestingUser.Email.ToLower() == trimmedEmail.ToLower()) {
                return BadRequest(new { Message = "You Cannot Modify Your Own Account" });
            }

            //Verify User To Elevate Exists
            User userToElevate = _context.Users.FirstOrDefault(u => u.Email == emailToElevate.UserEmailToModify);

            if (userToElevate == null) {
                return NotFound(new { Message = "Target User Not Found In DB" });
            } else if (userToElevate.IsAdmin) {
                return BadRequest(new { Message = "User Is Already Admin" });
            }

            UserRepository userRepo = new UserRepository(_context);

            //Perform Elevation
            if (!userRepo.ElevateAdminUser(requestingUser, userToElevate)) {
                return StatusCode(500, new { Message = "User DB Error" });
            }

            return Ok(new UserViewModel {
                Email = userToElevate.Email,
                IsAdmin = userToElevate.IsAdmin,
                EventsOrganizing = _context.EventOrganizers.Where(eo => eo.User == userToElevate).Count(),
                VendorsManaged = _context.EventVendorUsers.Where(evu => evu.User == userToElevate).Count()
            });
        }

        /// <response code="400">Insufficient Permissions To Modify Users</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Rights To Modify Users</response>
        /// <response code="404">Cannot Find Users Account</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Users Email and Admin Privileges", typeof(UserViewModel))]
        [SwaggerOperation(
            Summary = "Removes Admin Rights From A Specified User",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin<br>" +
                          "**Audited Function**<br>"
        )]
        [Produces("application/json")]
        [HttpPut("demote")]
        public IActionResult DemoteUser([FromBody] UserAdminInputViewModel emailToDemote) {
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

            //Verify User Is Not Themselves
            String trimmedEmail = emailToDemote.UserEmailToModify.Trim();

            if (requestingUser.Email.ToLower() == trimmedEmail.ToLower()) {
                return BadRequest(new { Message = "You Cannot Modify Your Own Account" });
            }

            //Verify User To Elevate Exists
            User userToElevate = _context.Users.FirstOrDefault(u => u.Email == emailToDemote.UserEmailToModify);

            if (userToElevate == null) {
                return NotFound(new { Message = "Target User Not Found In DB" });
            } else if (!userToElevate.IsAdmin) {
                return BadRequest(new { Message = "Target User Has No Admin Rights" });
            }

            UserRepository userRepo = new UserRepository(_context);

            //Perform Elevation
            if (!userRepo.DemoteAdminUser(requestingUser, userToElevate)) {
                return StatusCode(500, new { Message = "User DB Error" });
            }

            return Ok(new UserViewModel {
                Email = userToElevate.Email,
                IsAdmin = userToElevate.IsAdmin,
                EventsOrganizing = 0,
                VendorsManaged = 0
            });
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find Users Account</response>   
        [SwaggerResponse(200, "List Of Events User Has Joined", typeof(UserEventListViewModel))]
        [SwaggerOperation(
            Summary = "Gets Events User Has Joined",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpGet("eventlist")]
        public IActionResult GetUsersEvents() {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            EventGuestRepository eventGuestRepo = new EventGuestRepository(_context);

            return Ok(eventGuestRepo.GetUsersEvents(theUser));
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find Users Account</response>   
        [SwaggerResponse(200, "List Of Users Event Subscriptions", typeof(EventSubscriptionSummaryViewModel))]
        [SwaggerOperation(
            Summary = "Gets Vendors User Has Subscribed To",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpGet("subscriptionlist")]
        public IActionResult GetUsersSubscriptions() {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            EventGuestSubscriptionRepository eventGuestSubRepo = new EventGuestSubscriptionRepository(_context);

            return Ok(eventGuestSubRepo.GetUsersSubscriptions(theUser));
        }
    }
}