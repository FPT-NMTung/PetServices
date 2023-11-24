using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PetServices.Models;
using System.Globalization;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public DashboardController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("GetNumberCustomerInMonth")]
        public async Task<ActionResult> GetNumberCustomerInMonth()
        {
            int currentMonth = DateTime.Now.Month;

            var customerNumber = await _context.Accounts.Where(a => a.CreateDate.Value.Month == currentMonth).ToListAsync();

            return Ok(customerNumber.Count);
        }

        [HttpGet("GetPercentCustomerPreviousMonth")]
        public async Task<ActionResult> GetPercentCustomerPreviousMonth()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            int previousMonth;
            int newYear;

            if (currentMonth == 1)
            {
                previousMonth = 12;
                newYear = currentYear - 1;
            }
            else
            {
                previousMonth = currentMonth - 1;
                newYear = currentYear;
            }

            var customerNumber = await _context.Accounts.Where(a => a.CreateDate.Value.Month == currentMonth && a.CreateDate.Value.Year == currentYear).ToListAsync();
            var customerNumberPreviousMonth = await _context.Accounts.Where(a => a.CreateDate.Value.Month == previousMonth && a.CreateDate.Value.Year == newYear).ToListAsync();

            if (customerNumberPreviousMonth.Count == 0)
            {
                return Ok(customerNumber.Count / 1 * 100);
            }

            double percent = (double)(customerNumber.Count - customerNumberPreviousMonth.Count) / customerNumberPreviousMonth.Count * 100;

            return Ok(percent.ToString("F2"));
        }

        [HttpGet("GetNumberOrderInMonth")]
        public async Task<ActionResult> GetNumberOrderInMonth()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            var numberOrder = await _context.Orders.Where(o => o.OrderDate.Value.Month == currentMonth && o.OrderDate.Value.Year == currentYear).ToListAsync();

            return Ok(numberOrder.Count);
        }

        [HttpGet("GetPercentOrderPreviousMonth")]
        public async Task<ActionResult> GetPercentOrderPreviousMonth()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            int previousMonth;
            int newYear;

            if (currentMonth == 1)
            {
                previousMonth = 12;
                newYear = currentYear - 1;
            }
            else
            {
                previousMonth = currentMonth - 1;
                newYear = currentYear;
            }
            var numberOrder = await _context.Orders.Where(o => o.OrderDate.Value.Month == currentMonth && o.OrderDate.Value.Year == currentYear).ToListAsync();
            var numberOrderPreviousMonth = await _context.Orders.Where(o => o.OrderDate.Value.Month == previousMonth && o.OrderDate.Value.Year == newYear).ToListAsync();

            if (numberOrderPreviousMonth.Count == 0)
            {
                return Ok(numberOrder.Count / 1 * 100);
            }

            double percent = (double)(numberOrder.Count - numberOrderPreviousMonth.Count) / numberOrderPreviousMonth.Count * 100;

            return Ok(percent.ToString("F2"));
        }

        [HttpGet("GetIncomeInMonth")]
        public async Task<ActionResult> GetIncomeInMonth()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            double totalIncome = 0;

            var ordersInMonth = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == currentMonth && o.OrderDate.Value.Year == currentYear)
                .ToListAsync();

            foreach (var order in ordersInMonth)
            {
                var productIncome = await _context.OrderProductDetails
                    .Where(o => o.OrderId == order.OrderId)
                    .ToListAsync();

                foreach (var product in productIncome)
                {
                    totalIncome += product.Price ?? 0;
                }

                var ServiceIncome = await _context.BookingServicesDetails
                    .Where(o => o.OrderId == order.OrderId)
                    .ToListAsync();

                foreach (var service in ServiceIncome)
                {
                    totalIncome += service.Price ?? 0;
                }

                var RoomIncome = await _context.BookingRoomDetails
                    .Where(o => o.OrderId == order.OrderId)
                    .ToListAsync();

                foreach (var room in RoomIncome)
                {
                    totalIncome += room.Price ?? 0;
                }
            }

            return Ok(totalIncome);
        }


        [HttpGet("GetPercentIncomePreviousMonth")]
        public async Task<ActionResult> GetPercentIncomePreviousMonth()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            int previousMonth;
            int newYear;
            double totalIncome = 0;
            double totalIncomepreviousMonth = 0;

            if (currentMonth == 1)
            {
                previousMonth = 12;
                newYear = currentYear - 1;
            }
            else
            {
                previousMonth = currentMonth - 1;
                newYear = currentYear;
            }

            var ordersInMonth = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == currentMonth && o.OrderDate.Value.Year == currentYear)
                .ToListAsync();

            var ordersPreviousMonth = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == previousMonth && o.OrderDate.Value.Year == newYear)
                .ToListAsync();

            foreach (var order in ordersInMonth)
            {
                var productIncome = await _context.OrderProductDetails
                    .Where(o => o.OrderId == order.OrderId)
                    .ToListAsync();

                foreach (var product in productIncome)
                {
                    totalIncome += product.Price ?? 0;
                }

                var ServiceIncome = await _context.BookingServicesDetails
                    .Where(o => o.OrderId == order.OrderId)
                    .ToListAsync();

                foreach (var service in ServiceIncome)
                {
                    totalIncome += service.Price ?? 0;
                }

                var RoomIncome = await _context.BookingRoomDetails
                    .Where(o => o.OrderId == order.OrderId)
                    .ToListAsync();

                foreach (var room in RoomIncome)
                {
                    totalIncome += room.Price ?? 0;
                }
            }

            foreach (var order in ordersPreviousMonth)
            {
                var productIncome = await _context.OrderProductDetails
                    .Where(o => o.OrderId == order.OrderId)
                    .ToListAsync();

                foreach (var product in productIncome)
                {
                    totalIncomepreviousMonth += product.Price ?? 0;
                }

                var ServiceIncome = await _context.BookingServicesDetails
                    .Where(o => o.OrderId == order.OrderId)
                    .ToListAsync();

                foreach (var service in ServiceIncome)
                {
                    totalIncomepreviousMonth += service.Price ?? 0;
                }

                var RoomIncome = await _context.BookingRoomDetails
                    .Where(o => o.OrderId == order.OrderId)
                    .ToListAsync();

                foreach (var room in RoomIncome)
                {
                    totalIncomepreviousMonth += room.Price ?? 0;
                }
            }

            if (totalIncomepreviousMonth == 0)
            {
                return Ok(totalIncome / 1 * 100);
            }

            double percent = (double)(totalIncome - totalIncomepreviousMonth) / totalIncomepreviousMonth * 100;

            return Ok(percent.ToString("F2"));
        }

        [HttpGet("GetNumberProductInMonth")]
        public async Task<ActionResult> GetNumberProductInMonth()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            int sell​​NumberProduct = 0;

            var numberProduct = await _context.Orders.Where(o => o.OrderDate.Value.Month == currentMonth && o.OrderDate.Value.Year == currentYear).ToListAsync();

            foreach ( var number in numberProduct)
            {
                var productIncome = await _context.OrderProductDetails
                    .Where(o => o.OrderId == number.OrderId)
                    .ToListAsync();

                foreach ( var product in productIncome)
                {
                    sell​​NumberProduct += product.Quantity ?? 0;
                }
            }

            return Ok(sell​​NumberProduct);
        }

        [HttpGet("GetPercentNumberProductPreviousMonth")]
        public async Task<ActionResult> GetPercentNumberProductPreviousMonth()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            int previousMonth;
            int newYear;

            int sell​​NumberProduct = 0;
            int sell​​NumberProductPreviousMonth = 0;


            if (currentMonth == 1)
            {
                previousMonth = 12;
                newYear = currentYear - 1;
            }
            else
            {
                previousMonth = currentMonth - 1;
                newYear = currentYear;
            }

            var numberProduct = await _context.Orders.Where(o => o.OrderDate.Value.Month == currentMonth && o.OrderDate.Value.Year == currentYear).ToListAsync();
            var numberProductPreviousMonth = await _context.Orders.Where(o => o.OrderDate.Value.Month == previousMonth && o.OrderDate.Value.Year == newYear).ToListAsync();

            foreach (var number in numberProduct)
            {
                var productIncome = await _context.OrderProductDetails
                    .Where(o => o.OrderId == number.OrderId)
                    .ToListAsync();

                foreach (var product in productIncome)
                {
                    sell​​NumberProduct += product.Quantity ?? 0;
                }
            }

            foreach (var number in numberProductPreviousMonth)
            {
                var productIncome = await _context.OrderProductDetails
                    .Where(o => o.OrderId == number.OrderId)
                    .ToListAsync();

                foreach (var product in productIncome)
                {
                    sell​​NumberProductPreviousMonth += product.Quantity ?? 0;
                }
            }

            if (sell​​NumberProductPreviousMonth == 0)
            {
                return Ok(sell​​NumberProductPreviousMonth / 1 * 100);
            }

            double percent = (double)(sell​​NumberProduct - sell​​NumberProductPreviousMonth) / sell​​NumberProductPreviousMonth * 100;

            return Ok(percent.ToString("F2"));
        }

        /*[HttpGet("GetAllCustomerByAll")]
        public async Task<ActionResult> GetAllCustomerByAll(int? day, int? month, int? year)
        {
            try
            {
                var query = _context.Accounts.AsQueryable();

                if (year.HasValue)
                {
                    if (day.HasValue && month.HasValue)
                    {
                        query = query.Where(n => n.CreateDate.Value.Day == day && n.CreateDate.Value.Month == month && n.CreateDate.Value.Year == year);
                    }
                    else if (month.HasValue)
                    {
                        query = query.Where(n => n.CreateDate.Value.Month == month && n.CreateDate.Value.Year == year);
                    }
                    else
                    {
                        query = query.Where(n => n.CreateDate.Value.Year == year);
                    }
                }
                else
                {
                    return BadRequest("Hãy cung cấp tham số: year.");
                }

                var newCustomer = await query.ToListAsync();

                return Ok(newCustomer);

            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }*/

        /*[HttpGet("GetAllAccount")]
        public async Task<ActionResult> GetPercentIncreaseCustomerByMonth()
        {
            try
            {
                var accountsInMonth = await _context.Accounts
                    .Where(a => a.CreateDate.Value.Month == month)
                    .ToListAsync();

                return Ok(accountsInMonth);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }*/
    }
}
