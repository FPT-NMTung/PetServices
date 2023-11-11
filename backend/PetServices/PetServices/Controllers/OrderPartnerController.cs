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
            .Include(z => z.UserInfo)
            .Where(o => o.BookingServicesDetails.Any(b => b.Service.SerCategories.SerCategoriesId == serCategoriesId))
            .ToListAsync();

            return Ok(_mapper.Map<List<OrdersDTO>>(orders));
        }
        [HttpGet("ListOrderPetTrainingSpecial")]
        public async Task<IActionResult> ListOrderPetTrainingSpecial(int serCategoriesId, int partnerInfoId)
        {
            List<Order> orders = await _context.Orders
                .Include(x => x.BookingServicesDetails)
                .ThenInclude(y => y.Service)
                .Include(z => z.UserInfo)
                .Where(o =>
                    o.BookingServicesDetails.Any(b => b.Service.SerCategories.SerCategoriesId == serCategoriesId) &&
                    o.BookingServicesDetails.All(b => b.PartnerInfoId == partnerInfoId))
                     .ToListAsync();

            return Ok(_mapper.Map<List<OrdersDTO>>(orders));
        }
    }
}
