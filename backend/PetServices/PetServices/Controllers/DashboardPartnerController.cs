using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
  
    public class DashboardPartnerController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;
        public DashboardPartnerController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpGet("GetAllOrderCompleted/{partnerId}")]
        public async Task<ActionResult> GetAllOrderCompleted(int partnerId)
        {
            var orderCompleted = await _context.Orders
                .Include(o => o.BookingServicesDetails)
                .Where(x => x.BookingServicesDetails.Any(y => y.StatusOrderService == "Completed" 
                && y.PartnerInfoId == partnerId))
                .ToListAsync();
            return Ok(orderCompleted);
        }

        // số đơn hàng trong tháng
        [HttpGet("OrderInMonth/{partnerId}")]
        public async Task<ActionResult> OrderInMonth(int partnerId)
        {
            int curMonth = DateTime.Now.Month;
            int curYear = DateTime.Now.Year;

            var numOrder = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == curMonth 
                && o.OrderDate.Value.Year == curYear 
                && o.BookingServicesDetails.
                Any(x => x.StatusOrderService == "Completed" 
                && x.PartnerInfoId == partnerId))
                .ToListAsync();

            return Ok(numOrder.Count);
        }

        // % số đơn hàng trong tháng so với tháng trước
        [HttpGet("GetPercentOrderInMonth/{partnerId}")]
        public async Task<ActionResult> GetPercentOrderInMonth(int partnerId)
        {
            int curMonth = DateTime.Now.Month;
            int curYear = DateTime.Now.Year;
            int preMonth;
            int newYear;

            if (curMonth == 1)
            {
                preMonth = 12;
                newYear = curYear - 1;
            }
            else
            {
                preMonth = curMonth - 1;
                newYear = curYear;
            }
            var numberOrderInMonth = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == curMonth 
                && o.OrderDate.Value.Year == curYear 
                && o.BookingServicesDetails
                .Any(x => x.StatusOrderService == "Completed" 
                && x.PartnerInfoId == partnerId))
                .ToListAsync();
            var numberOrderInPreviousMonth = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == preMonth 
                && o.OrderDate.Value.Year == newYear 
                && o.BookingServicesDetails
                .Any(x => x.StatusOrderService == "Completed"
                && x.PartnerInfoId == partnerId))
                .ToListAsync();

            if (numberOrderInPreviousMonth.Count == 0)
            {
                return Ok(numberOrderInMonth.Count / 1 * 100);
            }

            double percent = (double)(numberOrderInMonth.Count - numberOrderInPreviousMonth.Count) / numberOrderInPreviousMonth.Count * 100;

            return Ok(percent.ToString("F2"));
        }

        // tổng thu nhập trong tháng
        [HttpGet("GetTotalPriceInMonth/{partnerId}")]
        public async Task<ActionResult> GetTotalPriceInMonth(int partnerId)
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            double totalPrice = 0;

            var ordersInMonth = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == currentMonth 
                && o.OrderDate.Value.Year == currentYear
                && o.BookingServicesDetails
                .Any(x => x.StatusOrderService == "Completed"
                && x.PartnerInfoId == partnerId))
                .ToListAsync();

            foreach (var order in ordersInMonth)
            {
                totalPrice += order.TotalPrice ?? 0;
            }

            return Ok(totalPrice);
        }

        // % tổng thu nhập trong tháng so với tháng trước
        [HttpGet("GetPercentTotalPriceInMonthAndInPreMonth/{partnerId}")]
        public async Task<ActionResult> GetPercentTotalPriceInMonthAndInPreMonth(int partnerId)
        {
            int curMonth = DateTime.Now.Month;
            int curYear = DateTime.Now.Year;
            int previousMonth;
            int newYear;
            double totalPrice = 0;
            double totalPricePreviousMonth = 0;

            if (curMonth == 1)
            {
                previousMonth = 12;
                newYear = curYear - 1;
            }
            else
            {
                previousMonth = curMonth - 1;
                newYear = curYear;
            }

            var ordersInMonth = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == curMonth 
                && o.OrderDate.Value.Year == curYear
                && o.BookingServicesDetails
                .Any(x => x.StatusOrderService == "Completed"
                && x.PartnerInfoId == partnerId))
                .ToListAsync();

            var ordersPreviousMonth = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == previousMonth 
                && o.OrderDate.Value.Year == newYear 
                && o.BookingServicesDetails
                .Any(x => x.StatusOrderService == "Completed"
                && x.PartnerInfoId == partnerId))
                .ToListAsync();

            foreach (var order in ordersInMonth)
            {
                totalPrice += order.TotalPrice ?? 0;
            }

            foreach (var order in ordersPreviousMonth)
            {
                totalPricePreviousMonth += order.TotalPrice ?? 0;
            }

            if (totalPricePreviousMonth == 0)
            {
                return Ok(totalPrice / 1 * 100);
            }

            double percent = (double)(totalPrice - totalPricePreviousMonth) / totalPricePreviousMonth * 100;

            return Ok(percent.ToString("F2"));
        }

        // đánh giá của khách hàng về các dịch vụ
        [HttpGet("GetFeedbackOfCustomer/{partnerId}")]
        public async Task<ActionResult> GetFeedbackOfCustomer(int partnerId)
        {
            var listFeedback = await _context.Feedbacks
                .Where(o => o.PartnerId == partnerId)
                .ToListAsync();

            var stt = 1;
            var feedback = new List<FeedbackForm>();

            foreach (var feedbackItem in listFeedback)
            {
                var service = await _context.Services
                    .FirstOrDefaultAsync(o => o.ServiceId == feedbackItem.ServiceId);

                var customer = await _context.UserInfos
                    .FirstOrDefaultAsync(o => o.UserInfoId == feedbackItem.UserId);

                var partner = await _context.PartnerInfos
                    .FirstOrDefaultAsync(o => o.PartnerInfoId == feedbackItem.PartnerId);

                var account = await _context.Accounts
                    .FirstOrDefaultAsync(o => o.UserInfoId == feedbackItem.UserId);

                feedback.Add(new FeedbackForm
                {
                    stt = stt,
                    name = (partner.FirstName + " " + partner.LastName) ?? null,
                    picture = partner.ImagePartner ?? null,
                    gmail = account.Email,
                    customerName = customer.FirstName + customer.LastName ?? null,
                    NumberStart = feedbackItem.NumberStart,
                    Content = feedbackItem.Content,
                });

                stt++;
            }

            return Ok(feedback);
        }

        //tổng số đánh giá của partner
        [HttpGet("GetStarInProductPet/{partnerId}")]
        public async Task<ActionResult> GetStarInPartner(int partnerId)
        {
            var partners = await _context.PartnerInfos
                .Where(x => x.PartnerInfoId == partnerId)
                .ToListAsync();

            double totalStars = 0;
            int totalFeedbackCount = 0;
            int count = 0;


            foreach (var partner in partners)
            {
                var feedbacks = _context.Feedbacks
                    .Where(f => f.PartnerId == partner.PartnerInfoId)
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
        [HttpGet("GetPartnerStar/{partnerId}")]
        public async Task<ActionResult> GetPartnerStar(int partnerId)
        {
            var averageStars = _context.Feedbacks.Where(f => f.PartnerId == partnerId).Average(f => f.NumberStart);

            if (averageStars.HasValue)
            {
                averageStars = Math.Round(averageStars.Value, 1);
            }

            return Ok(averageStars);
        }
        [HttpGet("GetPartnerVoteNumber/{partnerId}")]
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

        // Doanh thu theo ngày
        [HttpGet("GetTotalPriceIn7Day/{partnerId}")]
        public async Task<ActionResult> GetTotalPriceIn7Day(int partnerId)
        {
            DateTime now = DateTime.Now;

            var ReceiveData = new List<ReceiveInDayForm>();

            for (int i = 6; i >= 0; i--)
            {
                DateTime date = now.AddDays(-i);
                double total = 0;

                var orders = await _context.Orders
                    .Where(o => o.OrderDate.Value.Date == date.Date 
                    && o.BookingServicesDetails
                    .Any(x => x.StatusOrderService == "Completed"
                    && x.PartnerInfoId == partnerId))
                    .ToListAsync();

                foreach (var order in orders)
                {
                    var services = await _context.BookingServicesDetails.Where(b => b.OrderId == order.OrderId).ToListAsync();

                    foreach (var service in services)
                    {
                        total += service.PriceService ?? 0;
                    }
                }

                ReceiveData.Add(new ReceiveInDayForm { Date = date.ToShortDateString(), Receive = total });
            }

            return Ok(ReceiveData);
        }

        // Số đơn hàng hoàn thành trong tháng 
        [HttpGet("GetNumberOrderCompleteInMonth/{partnerId}")]
        public async Task<ActionResult> GetNumberOrderCompleteInMonth(int partnerId)
        {
            DateTime now = DateTime.Now;

            var NumberOrderComplete = new List<Quantity_RatioForm>();

            for (int i = 0; i < 3; i++)
            {
                var curMonth = now.AddMonths(-i);
                var preMonth = curMonth.AddMonths(-1);

                var orders = await _context.Orders
                    .Where(o => o.BookingServicesDetails
                                .Any(x => x.StatusOrderService == "Completed"
                                && x.PartnerInfoId == partnerId)
                                && o.OrderDate.Value.Month == curMonth.Month
                                && o.OrderDate.Value.Year == curMonth.Year)
                    .ToListAsync();

                var ordersPrevious = await _context.Orders
                    .Where(o => o.BookingServicesDetails
                                .Any(x => x.StatusOrderService == "Completed"
                                && x.PartnerInfoId == partnerId)
                                && o.OrderDate.Value.Month == preMonth.Month
                                && o.OrderDate.Value.Year == preMonth.Year)
                    .ToListAsync();

                double percent = 0;

                if (ordersPrevious.Count == 0)
                {
                    percent = orders.Count / 1 * 100;
                }
                else
                {
                    percent = (double)(orders.Count - ordersPrevious.Count) / ordersPrevious.Count * 100;
                }

                NumberOrderComplete.Add(new Quantity_RatioForm
                {
                    date = curMonth.ToString("yyyy-MM"),
                    quantity = orders.Count,
                    Ratio = percent.ToString("F2")
                });
            }

            return Ok(NumberOrderComplete);
        }
        // đánh giá của khách hàng về nhân viên
        [HttpGet("GetFeedbackOfPartner/{partnerId}")]
        public async Task<ActionResult> GetFeedbackOfPartner(int partnerId)
        {
            //lấy danh sách đánh giá các sản phẩm 
            var listFeedback = await _context.Feedbacks.Where(o => o.PartnerId == partnerId).ToListAsync();
            var stt = 1;

            var feedback = new List<FeedbackForm>();

            foreach (var Feedback in listFeedback)
            {
                var partner = await _context.Orders.FirstOrDefaultAsync(o => o.BookingServicesDetails.Any(x => x.PartnerInfoId == Feedback.PartnerId));
                var customer = await _context.UserInfos.FirstOrDefaultAsync(o => o.UserInfoId == Feedback.UserId);
                var account = await _context.Accounts.FirstOrDefaultAsync(o => o.UserInfoId == Feedback.UserId);


                feedback.Add(new FeedbackForm
                {
                    stt = stt,
                    name = partner.FullName,
                    gmail = account.Email,
                    customerName = customer.FirstName + customer.LastName,
                    NumberStart = Feedback.NumberStart,
                    Content = Feedback.Content,
                });

                stt++;
            }

            return Ok(feedback);
        }
    }
}
