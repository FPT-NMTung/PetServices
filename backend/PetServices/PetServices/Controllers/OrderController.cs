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
                List<Booking> bookings = _context.Bookings.Include(b => b.UserInfo).ToList();
                return Ok(_mapper.Map<List<BookingDTO>>(bookings));
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
                Booking bookings = await _context.Bookings.Include(b => b.UserInfo).Include(b => b.OrderProductDetails)
                .ThenInclude(o => o.Product).SingleOrDefaultAsync(b => b.BookingId == Id);
                return Ok(_mapper.Map<BookingDTO>(bookings));
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
                Booking booking = await _context.Bookings.SingleOrDefaultAsync(b => b.BookingId == Id);
                // Kiểm tra booking có tồn tại hay không
                if(booking == null)
                {
                    return NotFound("Booking không tồn tài");
                }
                // Kiểm tra xem trạng thái cũ có chính xác hay không
                if(booking.BookingStatus.Trim() != status.oldStatus)
                {
                    return BadRequest("Trạng thái cũ không hợp lệ");
                }
                booking.BookingStatus = status.newStatus;
                _context.Bookings.Update(booking);

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
