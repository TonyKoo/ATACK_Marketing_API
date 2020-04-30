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
using static ATACK_Marketing_API.Swagger.Examples;

namespace ATACK_Marketing_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase {
        private readonly MarketingDbContext _context;

        public VenuesController(MarketingDbContext context) {
            _context = context;
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="400">User Not Registered In DB</response>   
        [SwaggerResponse(200, "Listing Of All Venues", typeof(VenueViewModel))]
        [SwaggerOperation(
            Summary = "Gets List Of All Venues",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult GetVenues() {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            VenueRepository venueRepo = new VenueRepository(_context);

            return Ok(venueRepo.getVenues());
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified</response>
        /// <response code="404">Cannot Find Venue</response>
        [SwaggerResponse(200, "The Venue", typeof(VenueViewModel))]
        [SwaggerOperation(
            Summary = "Gets A Venue",
            Description = "Requires Authentication"
        )]
        [Produces("application/json")]
        [HttpGet("{venueId}")]
        public IActionResult GetVenue(int venueId) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            VenueRepository venueRepo = new VenueRepository(_context);
            Venue venue = venueRepo.getVenue(venueId);

            if (venue == null) {
                return NotFound(new { Message = "Venue Not Found" });
            }

            return Ok(new VenueViewModel { 
                VenueId = venue.VenueId,
                VenueName = venue.VenueName,
                Website = venue.Website
            });
        }


        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Permissions</response>
        /// <response code="404">Cannot Find User / Venue</response>
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(201, "New Venue", typeof(VenueViewModel))]
        [SwaggerOperation(
            Summary = "Adds A Venue",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin<br>" +
                          "Audited Function"
        )]
        [Produces("application/json")]
        [HttpPost("add")]
        public IActionResult AddVenue([FromBody]VenueInputViewModel newVenue) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify Requesting User Valid and Has Admin Rights
            User requestingUser = Retrieve.User(HttpContext.User, _context);

            if (requestingUser == null) {
                return NotFound(new { Message = "Requesting User Not Found In DB" });
            } else if (!requestingUser.IsAdmin) {
                return StatusCode(403, new { Message = "Insufficient Permissions To Modify Venues (Admin)" });
            }

            VenueRepository venueRepo = new VenueRepository(_context);
            (bool isSuccessful, Venue venue) = venueRepo.AddVenue(requestingUser, newVenue);

            if (!isSuccessful) {
                return StatusCode(500, new { Message = "Add Venue Error (DB Issue)" });
            }

            return StatusCode(201, new VenueViewModel {
                VenueId = venue.VenueId,
                VenueName = venue.VenueName,
                Website = venue.Website
            });
        }

        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Permissions</response>
        /// <response code="404">Cannot Find User / Venue</response>
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Updated Venue", typeof(VenueViewModel))]
        [SwaggerOperation(
            Summary = "Updates A Venue",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin<br>" +
                          "Audited Function"
        )]
        [Produces("application/json")]
        [HttpPut("update/{venueId}")]
        public IActionResult UpdateVenue(int venueId, [FromBody]VenueInputViewModel updatedVenue) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify Requesting User Valid and Has Admin Rights
            User requestingUser = Retrieve.User(HttpContext.User, _context);

            if (requestingUser == null) {
                return NotFound(new { Message = "Requesting User Not Found In DB" });
            } else if (!requestingUser.IsAdmin) {
                return StatusCode(403, new { Message = "Insufficient Permissions To Modify Venues (Admin)" });
            }

            VenueRepository venueRepo = new VenueRepository(_context);

            //Verify Venue Exists
            Venue theVenue = venueRepo.getVenue(venueId);
            if (theVenue == null) {
                return NotFound(new { Message = "Venue Not Found" });
            }

            //Update Venue
            if (!venueRepo.UpdateVenue(requestingUser, theVenue, updatedVenue)) {
                return StatusCode(500, new { Message = "Add Venue Error (DB Issue)" });
            }

            return Ok(new VenueViewModel {
                VenueId = theVenue.VenueId,
                VenueName = theVenue.VenueName,
                Website = theVenue.Website
            });
        }

        /// <response code="400">Delete Confirmation Doesn't Match / Venue Has Events Attached</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified / Insufficient Permissions</response>
        /// <response code="404">Cannot Find User / Venue</response>
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Removed Venue", typeof(VenueViewModel))]
        [SwaggerOperation(
            Summary = "Removes A Venue",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin<br>" + 
                          "**Audited Function**<br>" +
                          "**Note:** You Cannot Remove Venues If They Have Events" 
        )]
        [Produces("application/json")]
        [HttpDelete("remove/{venueId}")]
        public IActionResult RemoveVenue(int venueId, [FromBody]VenueDeleteInputViewModel deleteConfirmation) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify Requesting User Valid and Has Admin Rights
            User requestingUser = Retrieve.User(HttpContext.User, _context);

            if (requestingUser == null) {
                return NotFound(new { Message = "Requesting User Not Found In DB" });
            } else if (!requestingUser.IsAdmin) {
                return StatusCode(403, new { Message = "Insufficient Permissions To Modify Venues (Admin)" });
            }

            VenueRepository venueRepo = new VenueRepository(_context);

            //Verify Venue Exists
            Venue theVenue = venueRepo.getVenue(venueId);
            if (theVenue == null) {
                return NotFound(new { Message = "Venue Not Found" });
            }

            //Check For Events Attached
            EventsRepository eventsRepo = new EventsRepository(_context);
            if (eventsRepo.GetVenueEventCount(venueId) > 0) {
                return BadRequest(new { Message = "Venues With Events Cannot Be Removed" });
            } 

            //Delete Confirmation
            if (deleteConfirmation.ConfirmDeleteVenue != $"ConfirmDELETE - {theVenue.VenueName}") {
                return BadRequest(new { Message = "Venue Confirm Delete String Invalid" });
            }

            //Remove Venue
            if (!venueRepo.RemoveVenue(requestingUser, theVenue)) {
                return StatusCode(500, new { Message = "Add Venue Error (DB Issue)" });
            }

            return Ok(new VenueViewModel {
                VenueId = theVenue.VenueId,
                VenueName = theVenue.VenueName,
                Website = theVenue.Website
            });
        }
    }
}