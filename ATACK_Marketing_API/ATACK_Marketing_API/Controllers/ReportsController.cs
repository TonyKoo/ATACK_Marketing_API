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
    public class ReportsController : ControllerBase {
        private readonly MarketingDbContext _context;

        public ReportsController(MarketingDbContext context) {
            _context = context;
        }

        /// <response code="400">Event Vendor Doesn't Exist</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified/Insufficient Permissions</response>
        /// <response code="404">Cannot Find Users Account / Product Not Found</response>   
        [SwaggerResponse(200, "Subscription Report", typeof(VendorSubscriberReportViewModel))]
        [SwaggerOperation(
            Summary = "Generates A List Of All Event Guests Subscribed To Vendor",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Vendor"
        )]
        [Produces("application/json")]
        [HttpGet("subscribers/{eventVendorId}")]
        public IActionResult GetSubscriberReport(int eventVendorId) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            //Verify Event Vendor Exists
            EventVendor eventVendor = _context.EventVendors.FirstOrDefault(ev => ev.EventVendorId == eventVendorId);

            if (eventVendor == null) {
                return BadRequest(new { Message = "Event Vendor Doesn't Exist" });
            }

            //Permission Check
            if (!theUser.IsAdmin) {
                EventVendorUser eventVendorUser = _context.EventVendorUsers.FirstOrDefault(evu => evu.User == theUser &&
                                                                                                  evu.EventVendor == eventVendor);
                if (eventVendorUser == null) {
                    return StatusCode(403, new { Message = "Insufficient Permissions (Admin or Event Vendor)" });
                }
            }

            //Get The Report
            ReportsRepository reportsRepo = new ReportsRepository(_context);

            return Ok(reportsRepo.GetEventSubscribers(eventVendor));
        }
    }
}