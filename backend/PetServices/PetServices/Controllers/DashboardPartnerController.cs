using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.Models;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardPartnerController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;
        public DashboardPartnerController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpGet("GetAllOrderCompleted")]
        public async Task<ActionResult> GetAllOrderCompleted(int partnerId)
        {
            var orderCompleted = await _context.Orders
                .Include(o => o.BookingServicesDetails)
                .Where(x => x.BookingServicesDetails.Any(y => y.StatusOrderService == "Completed" 
                && y.PartnerInfoId == partnerId))
                .ToListAsync();
            return Ok(orderCompleted);
        }
    }
}
