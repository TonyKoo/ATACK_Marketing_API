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
    public class VendorManagement : ControllerBase {
        private readonly MarketingDbContext _context;

        public VendorManagement(MarketingDbContext context) {
            _context = context;
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find Users Account</response>   
        [SwaggerResponse(200, "List Of Vendors", typeof(VendorManagementViewModel))]
        [SwaggerOperation(
            Summary = "Gets List Of All Vendors In DB",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Event Organizer"
        )]
        [Produces("application/json")]
        [HttpGet(Name = "getvendors")]
        public IActionResult GetVendors() {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            //Permission Check
            if (!theUser.IsAdmin) {
                EventOrganizer eventOrganizer = _context.EventOrganizers.FirstOrDefault(eo => eo.User == theUser);

                if(eventOrganizer == null) {
                    return StatusCode(403, new { Message = "Insufficient Permissions (Admin or Event Organizer)" });
                }
            }

            VendorsRepository vendorRepo = new VendorsRepository(_context);

            return Ok(vendorRepo.GetAllVendors());
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find Users Account / Vendor</response>   
        [SwaggerResponse(200, "The Vendor", typeof(Vendor))]
        [SwaggerOperation(
            Summary = "Retrieve A Vendor From The Database",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Event Organizer"
        )]
        [Produces("application/json")]
        [HttpGet("{vendorId}")]
        public IActionResult GetVendor(int vendorId) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            //Permission Check
            if (!theUser.IsAdmin) {
                EventOrganizer eventOrganizer = _context.EventOrganizers.FirstOrDefault(eo => eo.User == theUser);

                if (eventOrganizer == null) {
                    return StatusCode(403, new { Message = "Insufficient Permissions (Admin or Event Organizer)" });
                }
            }

            //Check If Vendor Valid
            VendorsRepository vendorRepo = new VendorsRepository(_context);
            Vendor theVendor = vendorRepo.GetVendor(vendorId);

            if (theVendor == null) {
                return NotFound(new { Message = "Vendor Not Found" });
            }

            //Return Vendor
            return Ok(new Vendor { 
                VendorId = theVendor.VendorId,
                Name = theVendor.Name,
                Description = theVendor.Description,
                Email = theVendor.Email,
                Website = theVendor.Website
            });
        }

        /// <response code="400">Vendor Already In DB</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find Users Account</response>   
        /// <response code="500">Database/Server Error</response>    
        [SwaggerResponse(201, "Added Vendor", typeof(VendorAddModifyViewModel))]
        [SwaggerOperation(
            Summary = "Adds A Vendor To The Database",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Event Organizer<br>" + 
                          "**Audited Function**" 
        )]
        [Produces("application/json")]
        [HttpPost ("AddVendor")]
        public IActionResult CreateVendor([FromBody] VendorAddModifyViewModel newVendor) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User requestingUser = Retrieve.User(HttpContext.User, _context);

            if (requestingUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            //Permission Check
            if (!requestingUser.IsAdmin) {
                EventOrganizer eventOrganizer = _context.EventOrganizers.FirstOrDefault(eo => eo.User == requestingUser);

                if (eventOrganizer == null) {
                    return StatusCode(403, new { Message = "Insufficient Permissions (Admin or Event Organizer)" });
                }
            }

            //Verify Vendor Not Already In DB
            string trimmedVendorName = newVendor.Name.Trim();
            Vendor theVendor = _context.Vendors.FirstOrDefault(v => v.Name.ToLower() == trimmedVendorName.ToLower());

            if (theVendor != null) {
                return BadRequest(new { 
                    Message = $"Vendor {theVendor.Name} Appears To Already Be In DB",
                    Vendor = theVendor
                });
            }

            //Add Vendor
            VendorsRepository vendorRepo = new VendorsRepository(_context);
            bool isCreated;
            (isCreated, theVendor) = vendorRepo.AddVendor(requestingUser, newVendor);
            if (!isCreated) {
                return StatusCode(500, new { Message = "Vendor Add Error (DB Issue)" });
            }

            return CreatedAtRoute("getvendors", theVendor);
        }

        /// <response code="400">Vendor Not Found In DB</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find Users Account</response>  
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Updated Vendor", typeof(VendorAddModifyViewModel))]
        [SwaggerOperation(
            Summary = "Updates An Existing Vendor In The Database",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Event Organizer<br>" +
                          "**Audited Function**"
        )]
        [Produces("application/json")]
        [HttpPut ("{vendorId}")]
        public IActionResult UpdateVendor(int vendorId, [FromBody] VendorAddModifyViewModel updatedVendor) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User requestingUser = Retrieve.User(HttpContext.User, _context);

            if (requestingUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            //Verify Vendor Is In DB
            Vendor theVendor = _context.Vendors.FirstOrDefault(v => v.VendorId == vendorId);

            if (theVendor == null) {
                return BadRequest(new { Message = $"Vendor Not Found" });
            }

            //Permission Check
            if (!requestingUser.IsAdmin) {
                EventOrganizer eventOrganizer = _context.EventOrganizers.FirstOrDefault(eo => eo.User == requestingUser);

                if (eventOrganizer == null) {
                    return StatusCode(403, new { Message = "Insufficient Permissions (Admin or Event Organizer)" });
                }
            }

            //Update Vendor
            VendorsRepository vendorRepo = new VendorsRepository(_context);
            bool isUpdated;
            (isUpdated, theVendor) = vendorRepo.UpdateVendor(requestingUser, theVendor, updatedVendor);
            if (!isUpdated) {
                return StatusCode(500, new { Message = "Vendor Update Error (DB Issue)" });
            }

            return Ok(new Vendor {
                VendorId = theVendor.VendorId,
                Name = theVendor.Name,
                Description = theVendor.Description,
                Email = theVendor.Email,
                Website = theVendor.Website
            });
        }

        /// <response code="400">Delete Confirmation String Is Not Correct</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find Users Account / Vendor</response>   
        /// <response code="500">Database/Server Error</response>
        [SwaggerResponse(200, "Vendor Deleted", typeof(Vendor))]
        [SwaggerOperation(
            Summary = "Deletes Vendor From Database",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin<br>" +
                          "**Audited Function**<br>" + 
                          "**WARNING:** Will Remove Vendor Completely From DB, Even If They Are Attached To An Event"
        )]
        [Produces("application/json")]
        [HttpDelete("{vendorId}")]
        public IActionResult DeleteVendor(int vendorId, [FromBody] VendorDeleteViewModel deleteConfirm) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify Requesting User Valid and Has Admin Rights
            User requestingUser = Retrieve.User(HttpContext.User, _context);

            if (requestingUser == null) {
                return NotFound(new { Message = "Requesting User Not Found In DB" });
            } else if (!requestingUser.IsAdmin) {
                return StatusCode(403, new { Message = "Insufficient Permissions (Admin)" });
            }

            //Check If Vendor Valid
            VendorsRepository vendorRepo = new VendorsRepository(_context);
            Vendor theVendor = vendorRepo.GetVendor(vendorId);

            if (theVendor == null) {
                return NotFound(new { Message = "Vendor Not Found" });
            }

            //Verify Confirm Delete
            if (!(deleteConfirm.ConfirmDeleteName == $"ConfirmDELETE - {theVendor.Name}")) {
                return BadRequest(new { Message = "Vendor Confirm Delete String Invalid" });
            }

            //Delete Vendor
            if (!vendorRepo.DeleteVendor(requestingUser, theVendor)) {
                return StatusCode(500, new { Message = "Vendor Delete Error (DB Issue)" });
            }

            //Return Vendor
            return Ok(new Vendor {
                VendorId = theVendor.VendorId,
                Name = theVendor.Name,
                Description = theVendor.Description,
                Email = theVendor.Email,
                Website = theVendor.Website
            });
        }
    }
}