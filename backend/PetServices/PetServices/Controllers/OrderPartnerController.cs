using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PetServices.DTO;
using PetServices.Models;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderPartnerController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public OrderPartnerController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpGet("ListOrderPetTraining")]
        public async Task<IActionResult> ListOrderPetTraining(int serCategoriesId)
        {
            List<Order> orders = await _context.Orders
                .Include(x => x.BookingServicesDetails)
                .ThenInclude(y => y.Service)
                .Where(o => o.BookingServicesDetails.Any(b =>
                    b.Service != null &&
                    b.Service.Status == true &&
                    b.Service.SerCategoriesId == serCategoriesId))
                .ToListAsync();

            return Ok(_mapper.Map<List<OrdersDTO>>(orders));
        }

    }
}
