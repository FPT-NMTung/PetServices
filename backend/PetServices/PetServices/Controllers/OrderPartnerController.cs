using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PetServices.DTO;
using PetServices.Form;
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
                .Include(q => q.Reason)
                .Where(o =>
                    o.BookingServicesDetails.Any(b => b.Service.SerCategories.SerCategoriesId == serCategoriesId) &&
                    o.BookingServicesDetails.All(b => b.PartnerInfoId == partnerInfoId))
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
                    .Include(a => a.Reason)
                    .SingleOrDefaultAsync(b => b.OrderId == orderId);
                return Ok(_mapper.Map<OrdersDTO>(order));

            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("UpdateOrderStatusReceived")]
        public async Task<IActionResult> UpdateOrderStatusReceived(int orderId, int partnerId)
        {
            try
            {
                Order order = await _context.Orders
                    .Include(b => b.UserInfo)
                    .Include(b => b.BookingServicesDetails)
                    .ThenInclude(bs => bs.Service)
                    .SingleOrDefaultAsync(b => b.OrderId == orderId);
                foreach (var bookingDetail in order.BookingServicesDetails)
                {
                    if (bookingDetail.PartnerInfoId == null)
                    {
                        bookingDetail.PartnerInfoId = partnerId;
                    }
                    if(bookingDetail.StatusOrderService == "Waiting")
                    {
                        bookingDetail.StatusOrderService = "Received";
                    }
                }
                _context.Update(order);
                await _context.SaveChangesAsync();

                return Ok(_mapper.Map<OrdersDTO>(order));

            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("UpdateOrderStatusProcessing")]
        public async Task<IActionResult> UpdateOrderStatusProcessing(int orderId)
        {
            try
            {
                Order order = await _context.Orders
                    .Include(b => b.UserInfo)
                    .Include(b => b.BookingServicesDetails)
                    .ThenInclude(bs => bs.Service)
                    .SingleOrDefaultAsync(b => b.OrderId == orderId);
                foreach (var bookingDetail in order.BookingServicesDetails)
                {
                    if(bookingDetail.StatusOrderService == "Received")
                    {
                        bookingDetail.StatusOrderService = "Processing";
                    }
                }
                _context.Update(order);
                await _context.SaveChangesAsync();

                return Ok(_mapper.Map<OrdersDTO>(order));

            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("UpdateOrderStatusRejected")]
        public async Task<IActionResult> UpdateOrderStatusRejected(int orderId, int reasonId)
        {
            try
            {
                Order order = await _context.Orders
                    .Include(a => a.BookingServicesDetails)
                    .Include(a => a.Reason)
                    .SingleOrDefaultAsync(b => b.OrderId == orderId);
                // Kiểm tra booking có tồn tại hay không
                if (order == null)
                {
                    return NotFound("Booking không tồn tài");
                }

                // Cập nhật PartnerInfoId thành null cho các BookingServicesDetail có PartnerInfoId 
                foreach (var bookingDetail in order.BookingServicesDetails)
                {
                    if (bookingDetail.PartnerInfoId != null)
                    {
                        bookingDetail.PartnerInfoId = null;
                        if(bookingDetail.PriceService != null)
                        {
                            bookingDetail.PriceService -= 50000;
                        }
                    }
                }
                order.TotalPrice -= 50000;
                order.ReasonId = reasonId;
                _context.Orders.Update(order);

                await _context.SaveChangesAsync();
                return Ok("Đổi trạng thái thành công");
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("UpdateOrderStatusCompleted")]
        public async Task<IActionResult> UpdateOrderStatusCompleted(int orderId)
        {
            try
            {
                Order order = await _context.Orders
                    .Include(b => b.UserInfo)
                    .Include(b => b.BookingServicesDetails)
                    .ThenInclude(bs => bs.Service)
                    .SingleOrDefaultAsync(b => b.OrderId == orderId);
                order.OrderStatus = "Completed";
                _context.Update(order);
                await _context.SaveChangesAsync();
                return Ok(_mapper.Map<OrdersDTO>(order));

            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(int orderId, [FromBody] Status status)
        {
            try
            {
                Order order = await _context.Orders
                    .Include(c => c.BookingServicesDetails)
                    .SingleOrDefaultAsync(b => b.OrderId == orderId);
                if(order == null)
                {
                    return NotFound("Booking không tồn tại!");
                }
                if(order.OrderStatus.Trim() != status.oldStatus)
                {
                    return BadRequest("Trạng thái cũ không hợp lệ");
                }
                foreach (var bookingDetail in order.BookingServicesDetails)
                {
                    if (bookingDetail.PartnerInfoId != null)
                    {
                        bookingDetail.PartnerInfoId = null;
                        if(bookingDetail.PriceService != null)
                        {
                            bookingDetail.PriceService -= 50000;
                        }
                    }
                }
                order.TotalPrice -= 50000;
                order.OrderStatus = status.newStatus;
                _context.Orders.Update(order);

                await _context.SaveChangesAsync();
                return Ok("Đổi trạng thái thành công");
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }
    }
}
