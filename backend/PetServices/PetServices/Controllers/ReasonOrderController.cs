using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Form;
using PetServices.Models;
using System.Text.RegularExpressions;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReasonOrderController : Controller
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ReasonOrderController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("getReasonOrder")]
        public IActionResult GetReasonOrder()
        {
            try
            {
                List<ReasonOrder> orders = _context.ReasonOrders
                .ToList();
                return Ok(_mapper.Map<List<ReasonOrderDTO>>(orders));
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateReasonOrder([FromBody] ReasonOrderDTO reasonOrderDTO)
        {
            try
            {
                if (reasonOrderDTO == null)
                {
                    return BadRequest("Dữ liệu không hợp lệ: ReasonOrderDTO là null.");
                }

                if (string.IsNullOrEmpty(reasonOrderDTO.ReasonOrderTitle))
                {
                    return BadRequest("Dữ liệu không hợp lệ: ReasonTitle là bắt buộc.");
                }

                if (string.IsNullOrEmpty(reasonOrderDTO.ReasonOrderDescription))
                {
                    return BadRequest("Dữ liệu không hợp lệ: Mô tả lý do là bắt buộc.");
                }

                if (reasonOrderDTO.OrderId <= 0)
                {
                    return BadRequest("Dữ liệu không hợp lệ: OrderId phải lớn hơn 0.");
                }

                if (reasonOrderDTO.EmailReject == null)
                {
                    return BadRequest("Dữ liệu không hợp lệ: Email hủy đơn là bắt buộc.");
                }

                var reasonOrder = new ReasonOrder
                {
                    ReasonOrderTitle = reasonOrderDTO.ReasonOrderTitle,
                    ReasonOrderDescription = reasonOrderDTO.ReasonOrderDescription,
                    OrderId = reasonOrderDTO.OrderId,
                    EmailReject = reasonOrderDTO.EmailReject,
                    RejectTime = DateTime.Now,
                };

                _context.ReasonOrders.Add(reasonOrder);
                await _context.SaveChangesAsync();
                return Ok("Thêm lý do thành công!");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
