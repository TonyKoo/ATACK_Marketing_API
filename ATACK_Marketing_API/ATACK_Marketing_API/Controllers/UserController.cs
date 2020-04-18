using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ATACK_Marketing_API.Data;
using ATACK_Marketing_API.Models;
using ATACK_Marketing_API.Repositories;
using ATACK_Marketing_API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        /// <response code="404">Cannot Find Users Account</response>   
        //[SwaggerResponse(200, "Users Email and Admin Privileges", typeof(UserViewModel))]
        //[SwaggerOperation(
        //    Summary = "Get the user account from the database",
        //    Description = "Requires Authentication"
        //)]
        [Produces("application/json")]
        [HttpGet(Name = "getuser")]
        public IActionResult GetUserAccount() {
            var currentUser = HttpContext.User;
            string uid = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            User theUser = _context.Users.FirstOrDefault(u => u.Uid == uid);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found" });
            }

            return Ok(new UserViewModel {
                Email = theUser.Email,
                IsAdmin = theUser.IsAdmin
            });
        }

        /// <response code="400">User Already Exists</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="500">Database/Server Error</response>   
        //[SwaggerResponse(201, "Users Account Created Successfully", typeof(UserViewModel))]
        //[SwaggerOperation(
        //    Summary = "Creates a new user account in the database",
        //    Description = "Requires Authentication"
        //)]
        [Produces("application/json")]
        [HttpPost("create")]
        public IActionResult CreateUserAccount() {
            var currentUser = HttpContext.User;
            string userEmail = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            string uid = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            UserRepository userRepo = new UserRepository(_context);

            //Check if User Exists
            User theUser = _context.Users.FirstOrDefault(u => u.Uid == uid);

            if (theUser != null) {
                return BadRequest(new { Message = "User Already Exists" });
            }

            //Add User
            if (!userRepo.CreateUser(uid, userEmail)) {
                return StatusCode(500);
            }

            return CreatedAtRoute("getuser",
                                  new UserViewModel {
                                      Email = userEmail,
                                      IsAdmin = false
                                  });
        }
    }
}