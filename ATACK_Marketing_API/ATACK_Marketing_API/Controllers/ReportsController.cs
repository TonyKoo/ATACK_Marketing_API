using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ATACK_Marketing_API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ATACK_Marketing_API.Controllers
{
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
        //[SwaggerResponse(200, "Event Product Information", typeof(ProductRetrieveViewModel))]
        //[SwaggerOperation(
        //    Summary = "Gets An Event Vendor's Product",
        //    Description = "Requires Authentication<br>" +
        //                  "**Privileges:** Admin, Vendor"
        //)]
        [Produces("application/json")]
        [HttpGet("{eventVendorId}/products/{productId}", Name = "geteventproduct")]
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
                ProductDetails = theProduct.ProductDetails,
                EventId = eventVendor.Event.EventId,
                EventName = eventVendor.Event.EventName,
                EventVendorId = eventVendor.EventVendorId,
                EventVendorName = eventVendor.Vendor.Name
            });
        }
    }
}