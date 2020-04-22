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
        [SwaggerResponse(200, "Users Email and Admin Privileges", typeof(UserViewModel))]
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
                IsAdmin = theUser.IsAdmin
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
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Check if User Exists
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser != null) {
                return BadRequest(new { Message = "User Already Exists" });
            }

            //Add User
            UserRepository userRepo = new UserRepository(_context);

            var currentUser = HttpContext.User;
            string userEmail = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            string uid = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (!userRepo.CreateUser(uid, userEmail)) {
                return StatusCode(500, new { Message = "DB Error" });
            }

            return CreatedAtRoute("getuser",
                                  new UserViewModel {
                                      Email = userEmail,
                                      IsAdmin = false
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