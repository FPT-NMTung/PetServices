using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Form;
using PetServices.Models;
using System.Numerics;
using System.Text.RegularExpressions;

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
            if (string.IsNullOrWhiteSpace(orderDTO.Province))
            {
                string errorMessage = "Tỉnh không được để trống!";
                return BadRequest(errorMessage);
            }
            if (!Regex.IsMatch(orderDTO.Province, @"^[\p{L}\s]+$"))
            {
                string errorMessage = "Tỉnh phải là ký tự chữ, không chấp nhận số hay ký tự đặc biệt!";
                return BadRequest(errorMessage);
            }
            if (orderDTO.Province.Length > 50)
            {
                string errorMessage = "Tỉnh vượt quá số ký tự. Tối đa 50 ký tự!";
                return BadRequest(errorMessage);
            }
            if (string.IsNullOrWhiteSpace(orderDTO.District))
            {
                string errorMessage = "Huyện/Thành Phố không được để trống!";
                return BadRequest(errorMessage);
            }
            if (!Regex.IsMatch(orderDTO.District, @"^[\p{L}\s]+$"))
            {
                string errorMessage = "Huyện/Thành phố phải là ký tự chữ, không chấp nhận số hay ký tự đặc biệt!";
                return BadRequest(errorMessage);
            }
            if (orderDTO.District.Length > 50)
            {
                string errorMessage = "Huyện/Thành Phố vượt quá số ký tự. Tối đa 50 ký tự!";
                return BadRequest(errorMessage);
            }
            if (string.IsNullOrWhiteSpace(orderDTO.Commune))
            {
                string errorMessage = "Phường/Xã không được để trống!";
                return BadRequest(errorMessage);
            }
            if (!Regex.IsMatch(orderDTO.Commune, @"^[\p{L}\s]+$"))
            {
                string errorMessage = "Phường/Xã phải là ký tự chữ, không chấp nhận số hay ký tự đặc biệt!";
                return BadRequest(errorMessage);
            }
            if (orderDTO.Commune.Length > 50)
            {
                string errorMessage = "Phường/Xã vượt quá số ký tự. Tối đa 50 ký tự!";
                return BadRequest(errorMessage);
            }

            if (string.IsNullOrWhiteSpace(orderDTO.Address))
            {
                string errorMessage = "Địa chỉ không được để trống!";
                return BadRequest(errorMessage);
            }
            if (orderDTO.Address.Length > 500)
            {
                string errorMessage = "Địa chỉ vượt quá số ký tự. Tối đa 500 ký tự!";
                return BadRequest(errorMessage);
            }

            if (orderDTO.OrderProductDetails != null)
            {
                var invalidProducts = orderDTO.OrderProductDetails
                    .Where(dto => dto.Quantity <= 0 || dto.ProductId == null || !_context.Products.Any(p => p.ProductId == dto.ProductId))
                    .ToList();

                if (invalidProducts.Any())
                {
                    var errorMessage = "Sản phẩm không hợp lệ. ";

                    // Check for Quantity
                    var quantityErrors = invalidProducts
                        .Where(dto => dto.Quantity <= 0)
                        .Select(dto => $"Số lượng không hợp lệ cho sản phẩm với ID {dto.ProductId}");

                    // Check for ProductId
                    var productIdErrors = invalidProducts
                        .Where(dto => dto.ProductId == null || !_context.Products.Any(p => p.ProductId == dto.ProductId))
                        .Select(dto => $"Sản phẩm với ID {dto.ProductId} không tồn tại");

                    errorMessage += string.Join(". ", quantityErrors.Concat(productIdErrors));

                    return BadRequest(errorMessage);
                }

                // Check if the entered quantity is greater than available quantity
                var quantityExceedsAvailable = orderDTO.OrderProductDetails
                    .Any(dto => dto.Quantity > _context.Products.FirstOrDefault(p => p.ProductId == dto.ProductId)?.Quantity);

                if (quantityExceedsAvailable)
                {
                    return BadRequest("Số lượng sản phẩm không đủ");
                }
            }

            double priceProduct = 0;
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
                       /* Price = dto.Price,
                        Weight = dto.Weight,
                        PriceService = dto.PriceService ,
                        PetInfoId = dto.PetInfoId,
                    }).ToList()
                    : new List<BookingServicesDetail>()

                // Loại

            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok("Order thành công!");
        }
    }
}
