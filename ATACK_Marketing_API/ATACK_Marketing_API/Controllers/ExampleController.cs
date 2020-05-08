using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ATACK_Marketing_API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class ExampleController : ControllerBase {
        /// <summary>
        /// Test Endpoint For Communication (No Auth Token Required)
        /// </summary>
        /// <response code="200">Hello Message</response>
        [HttpGet]
        public IActionResult Get() {
            return Ok(new { Message = "Herro - I'm NOT Secure :(" });
        }


        /// <summary>
        /// Test Endpoint for Communication w/ Auth Token
        /// </summary>
        /// <response code="200">Hello Message With User's Email</response>
        [HttpGet]
        [Authorize]
        [Route("secure")]
        public IActionResult GetSecure() {
            var currentUser = HttpContext.User;
            string userEmail = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            //string uid = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return Ok(new { Message = $"Hiiiii {userEmail}! - This is Same Same But Different (SECURE)" });
        }

        /// <summary>
        /// Test Endpoint To Check For Email Verification With Firebase
        /// </summary>
        /// <response code="200">Email and Verification Status</response>
        [HttpGet]
        [Authorize]
        [Route("secure/emailverified")]
        public IActionResult VerifiedEmailStatus() {
            var currentUser = HttpContext.User;
            string userEmail = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var emailVerified = HttpContext.User.Claims.ElementAt(8).Value;

            return Ok(new { Email = userEmail,
                            EmailVerified = emailVerified 
            });
        }
    }
}