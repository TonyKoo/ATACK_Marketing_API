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
    public class EventVendorController : ControllerBase {
        private readonly MarketingDbContext _context;

        public EventVendorController(MarketingDbContext context) {
            _context = context;
        }

        /// <response code="400">Event Vendor Already Exists</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified/Insufficient Permissions</response>
        /// <response code="404">Cannot Find Users Account / Vendor / Event</response>   
        /// <response code="500">Database/Server Error</response>
        [SwaggerResponse(200, "Vendor and Event Information", typeof(EventVendorResultViewModel))]
        [SwaggerOperation(
            Summary = "Add An Existing Vendor To An Event",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Event Organizer<br>" +
                          "**Audited Function**"
        )]
        [Produces("application/json")]
        [HttpPost("add")]
        public IActionResult AddEventVendor([FromBody] EventVendorAddRemoveViewModel vendorToAdd) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            //Check If Vendor Valid
            VendorsRepository vendorRepo = new VendorsRepository(_context);
            Vendor theVendor = vendorRepo.GetVendor(vendorToAdd.VendorId);

            if (theVendor == null) {
                return NotFound(new { Message = "Vendor Not Found" });
            }

            //Verify Event Valid
            EventsRepository eventsRepo = new EventsRepository(_context);
            Event theEvent = eventsRepo.GetEvent(vendorToAdd.EventId);

            if (theEvent == null) {
                return NotFound(new { Message = "Event Not Found" });
            }

            //Permission Check
            if (!theUser.IsAdmin) {
                EventOrganizer eventOrganizer = _context.EventOrganizers.FirstOrDefault(eo => eo.User == theUser && eo.Event == theEvent);

                if (eventOrganizer == null) {
                    return StatusCode(403, new { Message = "Insufficient Permissions (Admin or Event Organizer)" });
                }
            }

            //Verify Event Vendor Not Already Vendor
            EventVendor eventVendor = _context.EventVendors.FirstOrDefault(ev => ev.Event == theEvent &&
                                                                                 ev.Vendor == theVendor);

            if (eventVendor != null) {
                return BadRequest(new { Message = "Event Vendor Already Exists" });
            }

            //Add The Event Vendor
            EventVendorRepository eventVendorRepo = new EventVendorRepository(_context);
            bool isSuccessful;
            (isSuccessful, eventVendor) = eventVendorRepo.AddEventVendor(theUser, theEvent, theVendor);

            if (!isSuccessful) {
                return StatusCode(500, new { Message = "EventVendor Add Error (DB Issue)" });
            }

            return StatusCode(201, new EventVendorResultViewModel { 
                EventVendorId = eventVendor.EventVendorId,
                EventId = eventVendor.Event.EventId,
                EventName = eventVendor.Event.EventName,
                EventStartDateTime = eventVendor.Event.EventDateTime,
                VendorId = eventVendor.Vendor.VendorId,
                VendorName = eventVendor.Vendor.Name,
                IsEventVendor = true
            });
        }

        /// <response code="400">Event Vendor Doesn't Exist / Event Vendor Has Vendor User's Attached / Delete Confirmation Incorrect</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified/Insufficient Permissions</response>
        /// <response code="404">Cannot Find Users Account</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Vendor Was Removed")]
        [SwaggerOperation(
            Summary = "Remove Vendor From An Event",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Event Organizer<br>" +
                          "**Audited Function**<br>" +
                          "**Note:** You Cannot Remove An Event Once Users Have Subscribed To That Vendor<br>" + 
                          "**WARNING:** Vendors Products For Event Will Be Removed"
        )]
        [Produces("application/json")]
        [HttpDelete("remove/{eventVendorId}")]
        public IActionResult RemoveEventVendor(int eventVendorId, [FromBody] EventVendorRemoveInputViewModel deleteString) {
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

            //Delete Confirmation
            if (deleteString.DeleteVendorString != $"{eventVendor.Event.EventName} - {eventVendor.Vendor.Name}") {
                return BadRequest(new { Message = "EventVendor Confirm Delete String Invalid" });
            }


            //Vendor User Check
            EventVendorUserRepository eventVendorUserRepo = new EventVendorUserRepository(_context);
            if(eventVendorUserRepo.GetEventVendorUsersCount(eventVendorId) > 0) {
                return BadRequest(new { Message = "Event Vendor Has Vendor Users Attached" });
            }

            //Permission Check
            if (!theUser.IsAdmin) {
                EventOrganizer eventOrganizer = _context.EventOrganizers.FirstOrDefault(eo => eo.User == theUser && eo.Event == eventVendor.Event);

                if (eventOrganizer == null) {
                    return StatusCode(403, new { Message = "Insufficient Permissions (Admin or Event Organizer)" });
                }
            }

            //Subscriber Check
            EventGuestSubscriptionRepository eventGuestSubscriptionRepo = new EventGuestSubscriptionRepository(_context);
            if (eventGuestSubscriptionRepo.GetUsersSubscribedVendorCount(eventVendorId) > 0) {
                return BadRequest(new { Message = "Event Vendor Has User Subscriptions" });
            }

            //Remove The Event Vendor
            EventVendorRepository eventVendorRepo = new EventVendorRepository(_context);
            if (!eventVendorRepo.RemoveEventVendor(theUser, eventVendor)) {
                return StatusCode(500, new { Message = "EventVendor Remove Error (DB Issue)" });
            }

            return Ok();
        }


        /// <response code="400">Event Vendor Doesn't Exist</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified/Insufficient Permissions</response>
        /// <response code="404">Cannot Find Users Account / Product Not Found</response>   
        [SwaggerResponse(200, "Event Product Information", typeof(ProductRetrieveViewModel))]
        [SwaggerOperation(
            Summary = "Gets An Event Vendor's Product",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Vendor"
        )]
        [Produces("application/json")]
        [HttpGet("{eventVendorId}/products/{productId}")]
        public IActionResult GetProduct(int eventVendorId, int productId) {
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

            //Get The Product
            ProductsRepository productsRepo = new ProductsRepository(_context);
            Product theProduct = productsRepo.GetEventProduct(productId);

            if (theProduct == null) {
                return NotFound(new { Message = "No Product Found With That ID" });
            }

            return Ok(new ProductRetrieveViewModel {
                ProductId = theProduct.ProductId,
                ProductName = theProduct.ProductName,
                ProductPrice = theProduct.ProductPrice,
                EventId = eventVendor.Event.EventId,
                EventName = eventVendor.Event.EventName,
                EventVendorId = eventVendor.EventVendorId,
                EventVendorName = eventVendor.Vendor.Name
            });
        }

        /// <response code="400">Event Vendor Doesn't Exist / Product Price Negative</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified/Insufficient Permissions</response>
        /// <response code="404">Cannot Find Users Account</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(201, "Added Event Product Information", typeof(ProductRetrieveViewModel))]
        [SwaggerOperation(
            Summary = "Allows Vendor To Add Products To An Event",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Vendor"
        )]
        [Produces("application/json")]
        [HttpPost("{eventVendorId}/products/add")]
        public IActionResult AddProduct(int eventVendorId, [FromBody]ProductInputViewModel newProduct) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            //Validate Input
            if (newProduct.ProductPrice < 0) {
                return BadRequest(new { Message = "Product Price Cannot Be Negative" });
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

            ProductsRepository productsRepo = new ProductsRepository(_context);

            //Add The Product
            (bool isSuccessful, Product theNewProduct) = productsRepo.AddEventProduct(eventVendor, newProduct);
            if (!isSuccessful) {
                return StatusCode(500, new { Message = "Product Add Error (DB Issue)" });
            }


            return StatusCode(201, new ProductRetrieveViewModel {
                ProductId = theNewProduct.ProductId,
                ProductName = theNewProduct.ProductName,
                ProductPrice = theNewProduct.ProductPrice,
                EventId = eventVendor.Event.EventId,
                EventName = eventVendor.Event.EventName,
                EventVendorId = eventVendor.EventVendorId,
                EventVendorName = eventVendor.Vendor.Name
            });
        }

        /// <response code="400">Event Vendor Doesn't Exist / Product Price Negative</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified/Insufficient Permissions</response>
        /// <response code="404">Cannot Find Users Account</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Updated Event Product Information", typeof(ProductRetrieveViewModel))]
        [SwaggerOperation(
            Summary = "Allows Vendor To Update A Product They Have Added To An Event",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Vendor"
        )]
        [Produces("application/json")]
        [HttpPut("{eventVendorId}/products/{productId}")]
        public IActionResult UpdateProduct(int eventVendorId, int productId, [FromBody]ProductInputViewModel updatedProduct) {
            if (!Validate.VerifiedUser(HttpContext.User)) {
                return StatusCode(403, new { Message = "Unverified User" });
            }

            //Verify User Valid
            User theUser = Retrieve.User(HttpContext.User, _context);

            if (theUser == null) {
                return NotFound(new { Message = "User Not Found In DB" });
            }

            //Validate Input
            if (updatedProduct.ProductPrice < 0) {
                return BadRequest(new { Message = "Product Price Cannot Be Negative" });
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

            //Verify Product Valid
            ProductsRepository productsRepo = new ProductsRepository(_context);
            Product product = _context.Products.FirstOrDefault(p => p.ProductId == productId && 
                                                                    p.EventVendor.EventVendorId == eventVendorId);

            if (product == null) {
                return NotFound(new { Message = "Product Not Found For Event" });
            }

            //Update Product
            if (!productsRepo.UpdateEventProduct(product, updatedProduct)) {
                return StatusCode(500, new { Message = "Product Update Error (DB Issue)" });
            }
            
            return Ok(new ProductRetrieveViewModel {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                EventId = eventVendor.Event.EventId,
                EventName = eventVendor.Event.EventName,
                EventVendorId = eventVendor.EventVendorId,
                EventVendorName = eventVendor.Vendor.Name
            });
        }


        /// <response code="400">Event Vendor Doesn't Exist</response>
        /// <response code="401">Missing Authentication Token</response>
        /// <response code="403">Users Email is Not Verified/Insufficient Permissions</response>
        /// <response code="404">Cannot Find Users Account</response>   
        /// <response code="500">Database/Server Error</response>  
        [SwaggerResponse(200, "Removed Event Product Information", typeof(ProductRetrieveViewModel))]
        [SwaggerOperation(
            Summary = "Allows Vendor To Remove A Product From An Event",
            Description = "Requires Authentication<br>" +
                          "**Privileges:** Admin, Vendor"
        )]
        [Produces("application/json")]
        [HttpDelete("{eventVendorId}/products/{productId}")]
        public IActionResult RemoveProduct(int eventVendorId, int productId) {
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

            //Verify Product Valid
            ProductsRepository productsRepo = new ProductsRepository(_context);
            Product product = _context.Products.FirstOrDefault(p => p.ProductId == productId &&
                                                                    p.EventVendor.EventVendorId == eventVendorId);

            if (product == null) {
                return NotFound(new { Message = "Product Not Found For Event" });
            }

            //Update Product
            if (!productsRepo.RemoveEventProduct(product)) {
                return StatusCode(500, new { Message = "Product Remove Error (DB Issue)" });
            }

            return Ok(new ProductRetrieveViewModel {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                EventId = eventVendor.Event.EventId,
                EventName = eventVendor.Event.EventName,
                EventVendorId = eventVendor.EventVendorId,
                EventVendorName = eventVendor.Vendor.Name
            });
        }
    }
}