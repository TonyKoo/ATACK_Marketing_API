using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ATACK_Marketing_API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ATACK_Marketing_API.Controllers
{
    [Route("api/{eventId}/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase {
        private readonly MarketingDbContext _context;

        public VendorsController(MarketingDbContext context) {
            _context = context;
        }


    }
}