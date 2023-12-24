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

                feedback.UserName = user?.LastName + " " + user?.FirstName;
                feedback.UserImage = user?.ImageUser;
            }

            return Ok(listFeedback);
        }

        [HttpGet("GetRoomStar")]
        public async Task<ActionResult> GetRoomStar(int roomID)
        {
            var averageStars = _context.Feedbacks.Where(f => f.RoomId == roomID).Average(f => f.NumberStart);

            if (averageStars.HasValue)
            {
                averageStars = Math.Round(averageStars.Value, 1);
            }

            return Ok(averageStars); ;
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

                feedback.UserName = user?.LastName + " " + user?.FirstName;
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

                feedback.UserName = user?.LastName + " " + user?.FirstName;
                feedback.UserImage = user?.ImageUser;
            }

            return Ok(listFeedback);
        }

        [HttpGet("GetProductStar")]
        public async Task<ActionResult> GetProductStar(int productID)
        {
            var averageStars = _context.Feedbacks.Where(f => f.ProductId == productID).Average(f => f.NumberStart);

            if (averageStars.HasValue)
            {
                averageStars = Math.Round(averageStars.Value, 1);
            }

            return Ok(averageStars);
        }

        [HttpGet("GetProductVoteNumber")]
        public async Task<ActionResult> GetProductVoteNumber(int productID)
        {
            var feedbacks = await _context.Feedbacks.Where(f => f.ProductId == productID).ToListAsync();

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

        [HttpGet("PaginationInProduct")]
        public async Task<ActionResult> PaginationInProduct(int productID, string starnumber, int pagenumber)
        {
            var pageSize = 5;
            var starNumber = 0;

            var feedbacks = await _context.Feedbacks.Where(f => f.ProductId == productID).ToListAsync();

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

                feedback.UserName = user?.LastName + " " + user?.FirstName;
                feedback.UserImage = user?.ImageUser;
            }

            return Ok(feedbacks1);
        }

        [HttpGet("GetAllFeedbackInService")]
        public async Task<ActionResult> GetAllFeedbackInService(int serviceID)
        {
            var feedbacks = await _context.Feedbacks.Where(f => f.ServiceId == serviceID).ToListAsync();

            var listFeedback = _mapper.Map<List<FeedbackDTO>>(feedbacks);

            foreach (var feedback in listFeedback)
            {
                var user = await _context.UserInfos.FirstOrDefaultAsync(u => u.UserInfoId == feedback.UserId);

                feedback.UserName = user?.LastName + " " + user?.FirstName;
                feedback.UserImage = user?.ImageUser;
            }

            return Ok(listFeedback);
        }

        [HttpGet("GetServiceStar")]
        public async Task<ActionResult> GetServiceStar(int serviceID)
        {
            var averageStars = _context.Feedbacks.Where(f => f.ServiceId == serviceID).Average(f => f.NumberStart);

            if (averageStars.HasValue)
            {
                averageStars = Math.Round(averageStars.Value, 1);
            }

            return Ok(averageStars); ;
        }

        [HttpGet("GetServiceVoteNumber")]
        public async Task<ActionResult> GetServiceVoteNumber(int serviceID)
        {
            var feedbacks = await _context.Feedbacks.Where(f => f.ServiceId == serviceID).ToListAsync();

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

        [HttpGet("PaginationInService")]
        public async Task<ActionResult> PaginationInService(int serviceID, string starnumber, int pagenumber)
        {
            var pageSize = 5;
            var starNumber = 0;

            var feedbacks = await _context.Feedbacks.Where(f => f.ServiceId == serviceID).ToListAsync();

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

                feedback.UserName = user?.LastName + " " + user?.FirstName;
                feedback.UserImage = user?.ImageUser;
            }

            return Ok(feedbacks1);
        }

        [HttpGet("GetAllFeedbackInallPartner")]
        public async Task<ActionResult> GetAllFeedbackInallPartner()
        {
            var feedbacks = await _context.Feedbacks.Where(f => f.PartnerId != null).ToListAsync();

            var listFeedback = _mapper.Map<List<FeedbackDTO>>(feedbacks);

            foreach (var feedback in listFeedback)
            {
                var user = await _context.UserInfos.FirstOrDefaultAsync(u => u.UserInfoId == feedback.UserId);

                feedback.UserName = user?.FirstName + " " + user?.LastName;
                feedback.UserImage = user?.ImageUser;
            }

            return Ok(listFeedback);
        }

        [HttpGet("GetAllFeedbackInPartner")]
        public async Task<ActionResult> GetAllFeedbackInPartner(int partnerId)
        {
            var feedbacks = await _context.Feedbacks.Where(f => f.PartnerId == partnerId).ToListAsync();

            var listFeedback = _mapper.Map<List<FeedbackDTO>>(feedbacks);

            foreach (var feedback in listFeedback)
            {
                var user = await _context.UserInfos.FirstOrDefaultAsync(u => u.UserInfoId == feedback.UserId);

                feedback.UserName = user?.FirstName + " " + user?.LastName;
                feedback.UserImage = user?.ImageUser;
            }

            return Ok(listFeedback);
        }

        [HttpGet("GetPartnerStar")]
        public async Task<ActionResult> GetPartnerStar(int partnerId)
        {
            var averageStars = _context.Feedbacks.Where(f => f.PartnerId == partnerId).Average(f => f.NumberStart);

            if (averageStars.HasValue)
            {
                averageStars = Math.Round(averageStars.Value, 1);
            }

            return Ok(averageStars); ;
        }

        [HttpGet("GetPartnerVoteNumber")]
        public async Task<ActionResult> GetPartnerVoteNumber(int partnerId)
        {
            var feedbacks = await _context.Feedbacks.Where(f => f.PartnerId == partnerId).ToListAsync();

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

        [HttpGet("PaginationInPartner")]
        public async Task<ActionResult> PaginationInPartner(int partnerId, string starnumber, int pagenumber)
        {
            var pageSize = 5;
            var starNumber = 0;

            var feedbacks = await _context.Feedbacks.Where(f => f.PartnerId == partnerId).ToListAsync();

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

                feedback.UserName = user?.LastName + " " + user?.FirstName;
                feedback.UserImage = user?.ImageUser;
            }

            return Ok(feedbacks1);
        }

        [HttpGet("GetStarInTakeCarePet")]
        public async Task<ActionResult> GetStarInTakeCarePet()
        {
            var takeCareServices = await _context.Services.Where(s => s.SerCategoriesId == 3).ToListAsync();

            double totalStars = 0;
            int totalFeedbackCount = 0;
            int count = 0;

            foreach (var service in takeCareServices)
            {
                var feedbacks = _context.Feedbacks
                    .Where(f => f.ServiceId == service.ServiceId)
                    .ToList();

                if (feedbacks.Any())
                {
                    totalStars += feedbacks.Average(f => f.NumberStart) ?? 0;
                    totalFeedbackCount += feedbacks.Count;
                    count++; 
                }
            }

            if (count > 0)
            {
                double averageStars = totalFeedbackCount > 0 ? Math.Round(totalStars / count, 1) : 0;

                var feedbackData = new FeedbackDataForm
                {
                    AverageStars = averageStars,
                    TotalFeedbackCount = totalFeedbackCount
                };

                return Ok(feedbackData);
            }
            else
            {
                var feedbackData = new FeedbackDataForm
                {
                    AverageStars = 0,
                    TotalFeedbackCount = 0
                };
                return Ok(feedbackData);
            }
        }

        [HttpGet("GetStarInRoomPet")]
        public async Task<ActionResult> GetStarInRoomPet()
        {
            var Rooms = await _context.Rooms.ToListAsync();

            double totalStars = 0;
            int totalFeedbackCount = 0;
            int count = 0;


            foreach (var room in Rooms)
            {
                var feedbacks = _context.Feedbacks
                    .Where(f => f.RoomId == room.RoomId)
                    .ToList();

                if (feedbacks.Any())
                {
                    totalStars += feedbacks.Average(f => f.NumberStart) ?? 0;
                    totalFeedbackCount += feedbacks.Count;
                    count++;
                }
            }

            if (count > 0)
            {
                double averageStars = totalFeedbackCount > 0 ? Math.Round(totalStars / count, 1) : 0;

                var feedbackData = new FeedbackDataForm
                {
                    AverageStars = averageStars,
                    TotalFeedbackCount = totalFeedbackCount
                };

                return Ok(feedbackData);
            }
            else
            {
                var feedbackData = new FeedbackDataForm
                {
                    AverageStars = 0,
                    TotalFeedbackCount = 0
                };
                return Ok(feedbackData);
            }
        }

        [HttpGet("GetStarInPetWalking")]
        public async Task<ActionResult> GetStarInPetWalking()
        {
            var PetWalkingServices = await _context.Services.Where(s => s.SerCategoriesId == 4).ToListAsync();

            double totalStars = 0;
            int totalFeedbackCount = 0;
            int count = 0;

            foreach (var service in PetWalkingServices)
            {
                var feedbacks = _context.Feedbacks
                    .Where(f => f.ServiceId == service.ServiceId)
                    .ToList();

                if (feedbacks.Any())
                {
                    totalStars += feedbacks.Average(f => f.NumberStart) ?? 0;
                    totalFeedbackCount += feedbacks.Count;
                    count++;
                }
            }

            if (count > 0)
            {
                double averageStars = totalFeedbackCount > 0 ? Math.Round(totalStars / count, 1) : 0;

                var feedbackData = new FeedbackDataForm
                {
                    AverageStars = averageStars,
                    TotalFeedbackCount = totalFeedbackCount
                };

                return Ok(feedbackData);
            }
            else
            {
                var feedbackData = new FeedbackDataForm
                {
                    AverageStars = 0,
                    TotalFeedbackCount = 0
                };
                return Ok(feedbackData);
            }
        }

        [HttpGet("GetStarInProductPet")]
        public async Task<ActionResult> GetStarInProductPet()
        {
            var Products = await _context.Products.ToListAsync();

            double totalStars = 0;
            int totalFeedbackCount = 0;
            int count = 0;


            foreach (var Product in Products)
            {
                var feedbacks = _context.Feedbacks
                    .Where(f => f.ProductId == Product.ProductId)
                    .ToList();

                if (feedbacks.Any())
                {
                    totalStars += feedbacks.Average(f => f.NumberStart) ?? 0;
                    totalFeedbackCount += feedbacks.Count;
                    count++;
                }
            }

            if (count > 0)
            {
                double averageStars = totalFeedbackCount > 0 ? Math.Round(totalStars / count, 1) : 0;

                var feedbackData = new FeedbackDataForm
                {
                    AverageStars = averageStars,
                    TotalFeedbackCount = totalFeedbackCount
                };

                return Ok(feedbackData);
            }
            else
            {
                var feedbackData = new FeedbackDataForm
                {
                    AverageStars = 0,
                    TotalFeedbackCount = 0
                };
                return Ok(feedbackData);
            }
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
            if (feedbackDTO.Content.Length <= 5)
            {
                string errorMessage = "Nội dung không được viết dưới 5 kí tự!";
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
                if (feedbackDTO.OrderId != null)
                {
                    if (feedbackDTO.RoomId != null)
                    {
                        var roomOrder = await _context.BookingRoomDetails.FirstOrDefaultAsync(b => b.OrderId == feedbackDTO.OrderId && b.RoomId == feedbackDTO.RoomId);

                        roomOrder.FeedbackStatus = true;
                        await _context.SaveChangesAsync();
                    }

                    if (feedbackDTO.ProductId != null)
                    {
                        var productOrder = await _context.OrderProductDetails.FirstOrDefaultAsync(b => b.OrderId == feedbackDTO.OrderId && b.ProductId == feedbackDTO.ProductId);

                        productOrder.FeedbackStatus = true;
                        await _context.SaveChangesAsync();
                    }

                    if (feedbackDTO.ServiceId != null && feedbackDTO.PartnerId == null)
                    {
                        var serviceOrder = await _context.BookingServicesDetails.FirstOrDefaultAsync(b => b.OrderId == feedbackDTO.OrderId && b.ServiceId == feedbackDTO.ServiceId);

                        serviceOrder.FeedbackStatus = true;
                        await _context.SaveChangesAsync();
                    }

                    if (feedbackDTO.PartnerId != null && feedbackDTO.ServiceId != null)
                    {
                        var partnerOrder = await _context.BookingServicesDetails.FirstOrDefaultAsync(b => b.OrderId == feedbackDTO.OrderId && b.PartnerInfoId == feedbackDTO.PartnerId && b.ServiceId == feedbackDTO.ServiceId);

                        partnerOrder.FeedbackPartnerStatus = true;
                        await _context.SaveChangesAsync();
                    }
                }

                if (feedbackDTO.PartnerId != null && feedbackDTO.ServiceId != null)
                {
                    feedbackDTO.ServiceId = null;
                }

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
