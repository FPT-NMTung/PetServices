using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Form;
using PetServices.Models;
using System.Numerics;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public OrderController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
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
                    
                .ToList();
                return Ok(_mapper.Map<List<OrdersDTO>>(orders));
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("email/{email}")]
        public IActionResult GetOrderUser(string email)
        {
            try
            {
                Account orders = _context.Accounts
                .Include(a => a.UserInfo)
                .ThenInclude(u => u.Orders)
                .FirstOrDefault(a => a.Email == email);

                return Ok(_mapper.Map<AccountInfo>(orders));
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetOrder(int Id)
        {
            try
            {
                Order order = await _context.Orders.Include(b => b.UserInfo)
                .Include(b => b.OrderProductDetails)
                .ThenInclude(o => o.Product)
                .Include(b => b.BookingServicesDetails)
                .ThenInclude(bs => bs.Service)
                .Include(b =>  b.BookingRoomDetails)
                .ThenInclude(br => br.Room)
                .SingleOrDefaultAsync(b => b.OrderId == Id);
                return Ok(_mapper.Map<OrdersDTO>(order));

            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("changeStatus")]
        public async Task<IActionResult> ChangeStatus(int Id, [FromBody] Status status)
        {
            try
            {
                Order order = await _context.Orders.SingleOrDefaultAsync(b => b.OrderId == Id);
                // Kiểm tra booking có tồn tại hay không
                if(order == null)
                {
                    return NotFound("Booking không tồn tài");
                }
                // Kiểm tra xem trạng thái cũ có chính xác hay không
                if(order.OrderStatus.Trim() != status.oldStatus)
                {
                    return BadRequest("Trạng thái cũ không hợp lệ");
                }
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

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrdersDTO orderDTO)
        {
            if (orderDTO == null)
            {
                return BadRequest("Invalid order data");
            }

            double priceProduct = 0;
            double priceService = 0;
            double priceRoom = 0;

            if (orderDTO.OrderProductDetails != null)
            {
                var productIds = orderDTO.OrderProductDetails.Where(dto => dto != null).Select(dto => dto.ProductId).ToList();
                var takeProduct = _context.Products.FirstOrDefault(p => productIds.Contains(p.ProductId));
                if (takeProduct != null)
                {
                    priceProduct = (double)takeProduct.Price;
                }
            }

            if (orderDTO.BookingServicesDetails != null)
            {
                var serviceIds = orderDTO.BookingServicesDetails.Where(dto => dto != null).Select(dto => dto.ServiceId).ToList();
                var takeService = _context.Services.FirstOrDefault(s => serviceIds.Contains(s.ServiceId));
                if (takeService != null)
                {
                    priceService = (double)takeService.Price;
                }
            }

            if (orderDTO.BookingRoomDetails != null)
            {
                var roomIds = orderDTO.BookingRoomDetails.Where(dto => dto != null).Select(dto => dto.RoomId).ToList();
                var takeRoom = _context.Rooms.FirstOrDefault(r => roomIds.Contains(r.RoomId));
                if (takeRoom != null)
                {
                    priceRoom = (double)takeRoom.Price;
                }
            }

            var order = new Order
            {
                OrderDate = orderDTO.OrderDate,
                OrderStatus = orderDTO.OrderStatus,
                Province = orderDTO.Province,
                District = orderDTO.District,
                Commune = orderDTO.Commune,
                Address = orderDTO.Address,
                UserInfoId = orderDTO.UserInfoId,

                // Sản phẩm
                OrderProductDetails = orderDTO.OrderProductDetails != null 
                  ? orderDTO.OrderProductDetails.Select(dto => new OrderProductDetail
                {
                    Quantity = dto.Quantity,
                    Price = priceProduct,
                    ProductId = dto.ProductId,
                }).ToList() 
                : new List<OrderProductDetail>(),

                // Phòng
                BookingRoomDetails = orderDTO.BookingRoomDetails != null
                    ? orderDTO.BookingRoomDetails.Select(dto => new BookingRoomDetail
                    {
                        RoomId = dto.RoomId,
                        Price = priceRoom,

                    }).ToList()
                    : new List<BookingRoomDetail>(),

                // Dịch vụ
                BookingServicesDetails = orderDTO.BookingServicesDetails != null
                    ? orderDTO.BookingServicesDetails.Select(dto => new BookingServicesDetail
                    {
                        ServiceId = dto.ServiceId,
                        Price = dto.Price,
                        Weight = dto.Weight,
                        PriceService = priceService,
                        PetInfoId = dto.PetInfoId,
                        PartnerInfoId = dto.PartnerInfoId,
                    }).ToList()
                    : new List<BookingServicesDetail>()
                 
                // Loại

            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { Id = order.OrderId }, order);
        }

    }
}
