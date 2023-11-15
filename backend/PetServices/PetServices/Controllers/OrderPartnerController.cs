﻿using AutoMapper;
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
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Order> orders = _context.Orders.Include(b => b.UserInfo)
                    .Include(x => x.BookingServicesDetails)
                    .ThenInclude(y => y.Service)
                    .ToList();
                return Ok(_mapper.Map<List<OrdersDTO>>(orders));
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("ListOrderPetTraining")]
        public async Task<IActionResult> ListOrderPetTraining(int serCategoriesId)
        {
            List<Order> orders = await _context.Orders
            .Include(x => x.BookingServicesDetails)
            .ThenInclude(y => y.Service)
            .Include(z => z.UserInfo)
            .Where(o => o.BookingServicesDetails.Any(b => b.Service.SerCategories.SerCategoriesId == serCategoriesId))
            //&& (string.IsNullOrEmpty(orderStatus) || o.OrderStatus == orderStatus))
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
                //&& (string.IsNullOrEmpty(orderStatus) || o.OrderStatus == orderStatus))
                     .ToListAsync();

            return Ok(_mapper.Map<List<OrdersDTO>>(orders));
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            try
            {
                Order order = await _context.Orders
                    .Include(b => b.UserInfo)
                    .Include(b => b.BookingServicesDetails)
                    .ThenInclude(bs => bs.Service)
                    .SingleOrDefaultAsync(b => b.OrderId == orderId);
                return Ok(_mapper.Map<OrdersDTO>(order));

            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }
    }
}
