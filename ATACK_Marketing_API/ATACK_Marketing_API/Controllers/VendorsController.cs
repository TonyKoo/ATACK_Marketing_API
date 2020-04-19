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

    }
}