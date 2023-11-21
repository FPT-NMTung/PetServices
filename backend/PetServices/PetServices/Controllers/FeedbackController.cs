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

        [HttpGet("GetRoomStar")]
        public async Task<ActionResult> GetRoomStar(int roomID)
        {
            var averageStars = _context.Feedbacks.Where(f => f.RoomId == roomID).Average(f => f.NumberStart);

            return Ok(Convert.ToInt32(averageStars));
        }

        [HttpGet("GetRoomVoteNumber")]
        public async Task<ActionResult> GetRoomVoteNumber(int roomID)
        {
            var feedbacks = await _context.Feedbacks.Where(f => f.RoomId == roomID).ToListAsync();

            var feedback = new VoteNumberDTO
            {
                number5s = feedbacks.Count(f => f.NumberStart == 5),
                number4s = feedbacks.Count(f => f.NumberStart == 4),
                number3s = feedbacks.Count(f => f.NumberStart == 3),
                number2s = feedbacks.Count(f => f.NumberStart == 2),
                number1s = feedbacks.Count(f => f.NumberStart == 1),
            };

            return Ok(feedback);
        }

        [HttpGet("PaginationInRoom")]
        public async Task<ActionResult> PaginationInRoom(int roomID, string starnumber, int pagenumber)
        {
            var pageSize = 5;
            var starNumber = 0;

            var feedbacks = await _context.Feedbacks.Where(f => f.RoomId == roomID).ToListAsync();
                
            if (starnumber == "5star")
            {
                starNumber = 5;
            }
            if (starnumber == "4star")
            {
                starNumber = 4;
            }
            if (starnumber == "3star")
            {
                starNumber = 3;
            }
            if (starnumber == "2star")
            {
                starNumber = 2;
            }
            if (starnumber == "1star")
            {
                starNumber = 1;
            }

            if (starnumber != "0")
            {
                feedbacks = feedbacks.Where(f => f.NumberStart == starNumber).ToList();
            }

            var startIndex = (pagenumber - 1) * pageSize;

            feedbacks = feedbacks.Skip(startIndex).Take(pageSize).ToList();

            var feedbacks1 = _mapper.Map<List<FeedbackDTO>>(feedbacks);

            foreach (var feedback in feedbacks1)
            {
                var user = await _context.UserInfos.FirstOrDefaultAsync(u => u.UserInfoId == feedback.UserId);

                feedback.UserName = user?.LastName + user?.FirstName;
                feedback.UserImage = user?.ImageUser;
            }

            return Ok(feedbacks1);
        }

        [HttpGet("GetAllFeedbackInProduct")]
        public async Task<ActionResult> GetAllFeedbackInProduct(int productID)
        {
            var feedbacks = await _context.Feedbacks.Where(f => f.ProductId == productID).ToListAsync();

            var listFeedback = _mapper.Map<List<FeedbackDTO>>(feedbacks);

            foreach (var feedback in listFeedback)
            {
                var user = await _context.UserInfos.FirstOrDefaultAsync(u => u.UserInfoId == feedback.UserId);

                feedback.UserName = user?.LastName + user?.FirstName;
                feedback.UserImage = user?.ImageUser;
            }

            return Ok(listFeedback);
        }

        [HttpGet("GetProductStar")]
        public async Task<ActionResult> GetProductStar(int productID)
        {
            var averageStars = _context.Feedbacks.Where(f => f.ProductId == productID).Average(f => f.NumberStart);

            return Ok(Convert.ToInt32(averageStars));
        }

        [HttpGet("GetAllFeedbackInService")]
        public async Task<ActionResult> GetAllFeedbackInService(int serviceID)
        {
            var feedbacks = await _context.Feedbacks.Where(f => f.ServiceId == serviceID).ToListAsync();

            var listFeedback = _mapper.Map<List<FeedbackDTO>>(feedbacks);

            foreach (var feedback in listFeedback)
            {
                var user = await _context.UserInfos.FirstOrDefaultAsync(u => u.UserInfoId == feedback.UserId);

                feedback.UserName = user?.FirstName + " " + user?.LastName;
                feedback.UserImage = user?.ImageUser;
            }

            return Ok(listFeedback);
        }

        [HttpGet("GetServiceStar")]
        public async Task<ActionResult> GetServiceStar(int serviceID)
        {
            var averageStars = _context.Feedbacks.Where(f => f.ServiceId == serviceID).Average(f => f.NumberStart);

            return Ok(Convert.ToInt32(averageStars));
        }

        [HttpDelete("del")]
        public async Task<ActionResult> del(int feedbackId)
        {
            var feedback = _context.Feedbacks.Where(f => f.FeedbackId == feedbackId).FirstOrDefault();

            _context.Remove(feedback);
            _context.SaveChanges();

            return Ok();
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
