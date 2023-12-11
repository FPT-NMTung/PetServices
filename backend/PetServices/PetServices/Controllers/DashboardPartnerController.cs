using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet("GetAllOrderCompleted")]
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
        [HttpGet("OrderInMonth")]
        public async Task<ActionResult> OrderInMonth(int partnerId)
        {
            int curMonth = DateTime.Now.Month;
            int curYear = DateTime.Now.Year;

            var numOrder = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == curMonth 
                && o.OrderDate.Value.Year == curYear 
                && o.BookingServicesDetails.Any(x => x.StatusOrderService == "Completed" && x.PartnerInfoId == partnerId))
                .ToListAsync();

            return Ok(numOrder.Count);
        }

        // % số đơn hàng trong tháng so với tháng trước
        [HttpGet("GetPercentOrderInMonth")]
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
        [HttpGet("GetIncomeInMonth")]
        public async Task<ActionResult> GetIncomeInMonth(int partnerId)
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            double totalIncome = 0;

            var ordersInMonth = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == currentMonth 
                && o.OrderDate.Value.Year == currentYear
                && o.BookingServicesDetails
                .Any(x => x.StatusOrderService == "Completed"
                && x.PartnerInfoId == partnerId))
                .ToListAsync();

            foreach (var order in ordersInMonth)
            {
                totalIncome += order.TotalPrice ?? 0;
            }

            return Ok(totalIncome);
        }

        // % tổng thu nhập trong tháng so với tháng trước
        [HttpGet("GetPercentIncomePreviousMonth")]
        public async Task<ActionResult> GetPercentIncomePreviousMonth(int partnerId)
        {
            int curMonth = DateTime.Now.Month;
            int curYear = DateTime.Now.Year;
            int previousMonth;
            int newYear;
            double totalIncome = 0;
            double totalIncomePreviousMonth = 0;

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
                totalIncome += order.TotalPrice ?? 0;
            }

            foreach (var order in ordersPreviousMonth)
            {
                totalIncomePreviousMonth += order.TotalPrice ?? 0;
            }

            if (totalIncomePreviousMonth == 0)
            {
                return Ok(totalIncome / 1 * 100);
            }

            double percent = (double)(totalIncome - totalIncomePreviousMonth) / totalIncomePreviousMonth * 100;

            return Ok(percent.ToString("F2"));
        }
    }
}
