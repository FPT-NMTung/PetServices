using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Models;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;
        /*[Authorize(Roles = "MANAGER")]*/
        public FeedbackController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpGet("GetAllFeedback")]
        public IActionResult Get()
        {
            List<Feedback> blogs = _context.Feedbacks.ToList();
            return Ok(_mapper.Map<List<FeedbackDTO>>(blogs));
        }

        [HttpGet("GetInfomationForFeedback")]
        public IActionResult GetInfomationForFeedback()
        {
            return Ok();
        }

        [HttpGet("GetAllFeedbackInRoom")]
        public async Task<ActionResult> GetAllFeedbackInRoom(int roomID)
        {
            var feedbacks = await _context.Feedbacks.Where(f => f.RoomId == roomID).ToListAsync();

            var listFeedback = _mapper.Map<List<FeedbackDTO>>(feedbacks);

            foreach (var feedback in listFeedback)
            {
                var user = await _context.UserInfos.FirstOrDefaultAsync(u => u.UserInfoId == feedback.UserId);

                feedback.UserName = user?.LastName + user?.FirstName;
                feedback.UserImage = user?.ImageUser;
            }

            return Ok(listFeedback);
        }

        [HttpPost("AddFeedBack")]
        public async Task<ActionResult> AddFeedBack(FeedbackDTO feedbackDTO)
        {
            if(string.IsNullOrWhiteSpace(feedbackDTO.Content))
            {
                string errorMessage = "Nội dung không được để trống!";
                return BadRequest(errorMessage);
            }
            if (feedbackDTO.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 20)
            {
                string errorMessage = "Nội dung không được viết dưới 20 kí tự!";
                return BadRequest(errorMessage);
            }
            if (string.IsNullOrEmpty(feedbackDTO.NumberStart.ToString()))
            {
                string errorMessage = "Số sao không được để trống!";
                return BadRequest(errorMessage);
            }
            if (feedbackDTO.NumberStart < 1 && feedbackDTO.NumberStart > 5)
            {
                string errorMessage = "Số sao không được vượt quá 0-5!";
                return BadRequest(errorMessage);
            }

            try
            {
                var feedback = new Feedback
                {
                    Content = feedbackDTO.Content,
                    NumberStart = feedbackDTO.NumberStart,
                    ServiceId = feedbackDTO.ServiceId,
                    RoomId = feedbackDTO.RoomId,
                    PartnerId = feedbackDTO.PartnerId,
                    ProductId = feedbackDTO.ProductId,
                    UserId = feedbackDTO.UserId,
                    OrderId = feedbackDTO.OrderId,
                };

                await _context.Feedbacks.AddAsync(feedback);
                await _context.SaveChangesAsync();

                return Ok("Thêm đánh giá thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

    }
}
