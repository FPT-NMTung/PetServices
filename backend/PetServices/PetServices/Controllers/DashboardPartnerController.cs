using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}
