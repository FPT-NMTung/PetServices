using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Form;
using PetServices.Models;

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
                List<Order> orders = _context.Orders.Include(b => b.UserInfo).ToList();
                return Ok(_mapper.Map<List<OrdersDTO>>(orders));
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(int Id)
        {
            try
            {
                Order order = await _context.Orders.Include(b => b.UserInfo).Include(b => b.OrderProductDetails)
                .ThenInclude(o => o.Product).SingleOrDefaultAsync(b => b.OrderId == Id);
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
    }
}
