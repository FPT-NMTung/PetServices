﻿using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualBasic;
using PetServices.Form;
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

        [HttpGet("getallorder")]
        public async Task<ActionResult> getallorder()
        {
            var customerNumber = await _context.Orders.Where(o => o.OrderStatus == "Completed").ToListAsync();

            return Ok(customerNumber);
        }

        // số khách hàng mới trong tháng 
        [HttpGet("GetNumberCustomerInMonth")]
        public async Task<ActionResult> GetNumberCustomerInMonth()
        {
            int currentMonth = DateTime.Now.Month;

            var customerNumber = await _context.Accounts.Where(a => a.CreateDate.Value.Month == currentMonth).ToListAsync();

            return Ok(customerNumber.Count);
        }

        // % khách hàng mới trong tháng so với tháng trước 
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

        // số đơn hàng trong tháng
        [HttpGet("GetNumberOrderInMonth")]
        public async Task<ActionResult> GetNumberOrderInMonth()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            var numberOrder = await _context.Orders.Where(o => o.OrderDate.Value.Month == currentMonth && o.OrderDate.Value.Year == currentYear && o.OrderStatus == "Completed").ToListAsync();

            return Ok(numberOrder.Count);
        }

        // % số đơn hàng trong tháng so với tháng trước
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
            var numberOrder = await _context.Orders.Where(o => o.OrderDate.Value.Month == currentMonth && o.OrderDate.Value.Year == currentYear && o.OrderStatus == "Completed").ToListAsync();
            var numberOrderPreviousMonth = await _context.Orders.Where(o => o.OrderDate.Value.Month == previousMonth && o.OrderDate.Value.Year == newYear && o.OrderStatus == "Completed").ToListAsync();

            if (numberOrderPreviousMonth.Count == 0)
            {
                return Ok(numberOrder.Count / 1 * 100);
            }

            double percent = (double)(numberOrder.Count - numberOrderPreviousMonth.Count) / numberOrderPreviousMonth.Count * 100;

            return Ok(percent.ToString("F2"));
        }

        // tổng thu nhập trong tháng
        [HttpGet("GetIncomeInMonth")]
        public async Task<ActionResult> GetIncomeInMonth()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            double totalIncome = 0;

            var ordersInMonth = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == currentMonth && o.OrderDate.Value.Year == currentYear && o.OrderStatus == "Completed")
                .ToListAsync();

            foreach (var order in ordersInMonth)
            {
                totalIncome += order.TotalPrice ?? 0;
            }

            return Ok(totalIncome);
        }

        // % tổng thu nhập trong tháng so với tháng trước
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
                .Where(o => o.OrderDate.Value.Month == currentMonth && o.OrderDate.Value.Year == currentYear && o.OrderStatus == "Completed")
                .ToListAsync();

            var ordersPreviousMonth = await _context.Orders
                .Where(o => o.OrderDate.Value.Month == previousMonth && o.OrderDate.Value.Year == newYear && o.OrderStatus == "Completed")
                .ToListAsync();

            foreach (var order in ordersInMonth)
            {
                totalIncome += order.TotalPrice ?? 0;
            }

            foreach (var order in ordersPreviousMonth)
            {
                totalIncomepreviousMonth += order.TotalPrice ?? 0;
            }

            if (totalIncomepreviousMonth == 0)
            {
                return Ok(totalIncome / 1 * 100);
            }

            double percent = (double)(totalIncome - totalIncomepreviousMonth) / totalIncomepreviousMonth * 100;

            return Ok(percent.ToString("F2"));
        }

        // số lượng sản phẩm bán trong tháng 
        [HttpGet("GetNumberProductInMonth")]
        public async Task<ActionResult> GetNumberProductInMonth()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            int sell​​NumberProduct = 0;

            var numberProduct = await _context.Orders.Where(o => o.OrderDate.Value.Month == currentMonth && o.OrderDate.Value.Year == currentYear && o.OrderStatus == "Completed").ToListAsync();

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

        // % số lượng sản phẩm bán trong tháng so với tháng trước
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

            var numberProduct = await _context.Orders.Where(o => o.OrderDate.Value.Month == currentMonth && o.OrderDate.Value.Year == currentYear && o.OrderStatus == "Completed").ToListAsync();
            var numberProductPreviousMonth = await _context.Orders.Where(o => o.OrderDate.Value.Month == previousMonth && o.OrderDate.Value.Year == newYear && o.OrderStatus == "Completed").ToListAsync();

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

        // Doanh số service theo ngày
        [HttpGet("GetTotalPriceServiceIn7Day")]
        public async Task<ActionResult> GetTotalPriceServiceIn7Day()
        {
            DateTime now = DateTime.Now;

            var ReceiveData = new List<ReceiveInDayForm>();

            for (int i=1; i <= 7; i++)
            {
                DateTime date = now.AddDays(-i);
                double total = 0;

                var orders = await _context.Orders.Where(o => o.OrderDate == date && o.OrderStatus == "Completed").ToListAsync();

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

        // Doanh số product theo ngày
        [HttpGet("GetTotalPriceProductIn7Day")]
        public async Task<ActionResult> GetTotalPriceProductIn7Day()
        {
            DateTime now = DateTime.Now;

            var ReceiveData = new List<ReceiveInDayForm>();

            for (int i = 1; i <= 7; i++)
            {
                DateTime date = now.AddDays(-i);
                double total = 0;

                var orders = await _context.Orders.Where(o => o.OrderDate == date && o.OrderStatus == "Completed").ToListAsync();

                foreach (var order in orders)
                {
                    var products = await _context.OrderProductDetails.Where(b => b.OrderId == order.OrderId).ToListAsync();


                    foreach (var product in products)
                    {
                        total += product.Price ?? 0;
                    }
                }

                ReceiveData.Add(new ReceiveInDayForm { Date = date.ToShortDateString(), Receive = total });
            }

            return Ok(ReceiveData);
        }

        // Doanh số room theo ngày
        [HttpGet("GetTotalPriceRoomIn7Day")]
        public async Task<ActionResult> GetTotalPriceRoomIn7Day()
        {
            DateTime now = DateTime.Now;

            var ReceiveData = new List<ReceiveInDayForm>();

            for (int i = 1; i <= 7; i++)
            {
                DateTime date = now.AddDays(-i);
                double total = 0;

                var orders = await _context.Orders.Where(o => o.OrderDate == date && o.OrderStatus == "Completed").ToListAsync();

                foreach (var order in orders)
                {
                    var rooms = await _context.BookingRoomDetails.Where(b => b.OrderId == order.OrderId).ToListAsync();


                    foreach (var room in rooms)
                    {
                        total += room.Price ?? 0;
                    }
                }

                ReceiveData.Add(new ReceiveInDayForm { Date = date.ToShortDateString(), Receive = total });
            }

            return Ok(ReceiveData);
        }

        // Số đơn hàng hoàn thành trong tháng 
        [HttpGet("GetNumberOrderCompleteInMonth")]
        public async Task<ActionResult> GetNumberOrderCompleteInMonth()
        {
            DateTime now = DateTime.Now;

            var NumberOrderComplete = new List<Quantity_RatioForm>();

            for (int i = 0; i < 3; i++)
            {
                var currentMonth = now.AddMonths(-i);
                var previousMonth = currentMonth.AddMonths(-1);

                var orders = await _context.Orders
                    .Where(o => o.OrderStatus == "Completed" &&
                                o.OrderDate.Value.Month == currentMonth.Month &&
                                o.OrderDate.Value.Year == currentMonth.Year)
                    .ToListAsync();

                var ordersPrevious = await _context.Orders
                    .Where(o => o.OrderStatus == "Completed" &&
                                o.OrderDate.Value.Month == previousMonth.Month &&
                                o.OrderDate.Value.Year == previousMonth.Year)
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
                    date = currentMonth.ToString("yyyy-MM"),
                    quantity = orders.Count,
                    Ratio = percent.ToString("F2")
                });
            }

            return Ok(NumberOrderComplete);
        }

        // Số đơn hàng đã nhận trong tháng 
        [HttpGet("GetNumberOrderReceivedInMonth")]
        public async Task<ActionResult> GetNumberOrderReceivedInMonth()
        {
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            int previousMonth;
            int newYear;

            var NumberOrderComplete = new List<Quantity_RatioForm>();

            if (month == 1)
            {
                previousMonth = 12;
                newYear = year - 1;
            }
            else
            {
                previousMonth = month - 1;
                newYear = year;
            }

            var orders = await _context.Orders.Where(o => o.OrderStatus == "Received" && o.OrderStatus == "Delivery" && o.OrderDate.Value.Month == month && o.OrderDate.Value.Year == year).ToListAsync();

            return Ok(orders.Count);
        }

        // Số đơn hàng bị hủy trong tháng 
        [HttpGet("GetNumberOrderRejectedInMonth")]
        public async Task<ActionResult> GetNumberOrderRejectedInMonth()
        {
            DateTime now = DateTime.Now;

            var NumberOrderComplete = new List<Quantity_RatioForm>();

            for (int i = 0; i < 3; i++)
            {
                var currentMonth = now.AddMonths(-i);
                var previousMonth = currentMonth.AddMonths(-1);

                var orders = await _context.Orders
                    .Where(o => o.OrderStatus == "Rejected" &&
                                o.OrderDate.Value.Month == currentMonth.Month &&
                                o.OrderDate.Value.Year == currentMonth.Year)
                    .ToListAsync();

                var ordersPrevious = await _context.Orders
                    .Where(o => o.OrderStatus == "Rejected" &&
                                o.OrderDate.Value.Month == previousMonth.Month &&
                                o.OrderDate.Value.Year == previousMonth.Year)
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
                    date = currentMonth.ToString("MM-yyyy"),
                    quantity = orders.Count,
                    Ratio = percent.ToString("F2")
                });
            }

            return Ok(NumberOrderComplete);
        }

        [HttpGet("GetTop5RoomBooking")]
        public async Task<ActionResult> GetTop5RoomBooking()
        {
            DateTime now = DateTime.Now;

            var NumberOrderComplete = new List<Quantity_RatioForm>();

            // Lấy ra danh sách các đơn đặt hàng trong tháng hiện tại đã hoàn thành
            var orders = await _context.Orders
                .Include(o => o.BookingRoomDetails)
                .Where(o => o.OrderStatus == "Completed" && o.OrderDate.Value.Month == now.Month && o.OrderDate.Value.Year == now.Year)
                .ToListAsync();

            // Lấy ra danh sách các phòng theo các đơn đặt hàng đó
            var roomIds = orders
                .SelectMany(order => order.BookingRoomDetails.Select(o => o.RoomId))
                .ToList();

            // Tính toán số lượng đơn đặt hàng cho mỗi phòng
            var roomOrderCounts = roomIds
                .GroupBy(roomId => roomId)
                .ToDictionary(r => r.Key, r => r.Count());

            // Sắp xếp các phòng theo số lượng đơn đặt hàng giảm dần
            var topRooms = roomOrderCounts
                .OrderByDescending(o => o.Value)
                .Take(5)
                .ToList();

            // Tổng số đơn đặt hàng trong tháng hiện tại
            int totalOrders = orders.Count;

            NumberOrderComplete.Insert(0, new Quantity_RatioForm
            {
                date = "Phòng khác",
                quantity = totalOrders - topRooms.Count
            });

            foreach (var room in topRooms)
            {
                var Room = await _context.Rooms.FirstOrDefaultAsync(o => o.RoomId == room.Key);

                NumberOrderComplete.Add(new Quantity_RatioForm
                {
                    date = Room.RoomName,
                    quantity = room.Value
                });
            }

            return Ok(NumberOrderComplete);
        }

        [HttpGet("GetTop5ServiceBooking")]
        public async Task<ActionResult> GetTop5ServiceBooking()
        {
            DateTime now = DateTime.Now;

            var NumberOrderComplete = new List<Quantity_RatioForm>();

            // Lấy ra danh sách các đơn đặt hàng trong tháng hiện tại đã hoàn thành
            var orders = await _context.Orders
                .Include(o => o.BookingServicesDetails)
                .Where(o => o.OrderStatus == "Completed" && o.OrderDate.Value.Month == now.Month && o.OrderDate.Value.Year == now.Year)
                .ToListAsync();

            // Lấy ra danh sách các phòng theo các đơn đặt hàng đó
            var serviceIds = orders
                .SelectMany(order => order.BookingServicesDetails.Select(o => o.ServiceId))
                .ToList();

            // Tính toán số lượng đơn đặt hàng cho mỗi dịch vụ
            var serviceOrderCounts = serviceIds
                .GroupBy(serviceId => serviceId)
                .ToDictionary(r => r.Key, r => r.Count());

            // Sắp xếp các dịch vụ theo số lượng đơn đặt hàng giảm dần
            var topServices = serviceOrderCounts
                .OrderByDescending(o => o.Value)
                .Take(5)
                .ToList();

            // Tổng số đơn đặt hàng trong tháng hiện tại
            int totalOrders = orders.Count;

            NumberOrderComplete.Insert(0, new Quantity_RatioForm
            {
                date = "Dịch vụ khác",
                quantity = totalOrders - topServices.Count
            });

            foreach (var service in topServices)
            {
                var Service = await _context.Services.FirstOrDefaultAsync(o => o.ServiceId == service.Key);

                NumberOrderComplete.Add(new Quantity_RatioForm
                {
                    date = Service.ServiceName,
                    quantity = service.Value
                });
            }

            return Ok(NumberOrderComplete);
        }

        [HttpGet("GetTop5ProductOrder")]
        public async Task<ActionResult> GetTop5ProductOrder()
        {
            DateTime now = DateTime.Now;

            var NumberOrderComplete = new List<Quantity_RatioForm>();

            // Lấy ra danh sách các đơn đặt hàng trong tháng hiện tại đã hoàn thành
            var orders = await _context.Orders
                .Include(o => o.OrderProductDetails)
                .Where(o => o.OrderStatus == "Completed" && o.OrderDate.Value.Month == now.Month && o.OrderDate.Value.Year == now.Year)
                .ToListAsync();

            // Lấy ra danh sách các sản phẩm theo các đơn đặt hàng đó
            var ProductIds = orders
                .SelectMany(order => order.OrderProductDetails.Select(o => o.ProductId))
                .ToList();

            // Tính toán số lượng đơn đặt hàng cho mỗi sản phẩm
            var productOrderCounts = ProductIds
                .GroupBy(productId => productId)
                .ToDictionary(r => r.Key, r => r.Count());

            // Sắp xếp các sản phẩm theo số lượng đơn đặt hàng giảm dần
            var topProduct = productOrderCounts
                .OrderByDescending(o => o.Value)
                .Take(5)
                .ToList();

            // Tổng số đơn đặt hàng trong tháng hiện tại
            int totalOrders = orders.Count;

            NumberOrderComplete.Insert(0, new Quantity_RatioForm
            {
                date = "Sản phẩm khác",
                quantity = totalOrders - topProduct.Count
            });

            foreach (var product in topProduct)
            {
                var Product = await _context.Products.FirstOrDefaultAsync(o => o.ProductId == product.Key);

                NumberOrderComplete.Add(new Quantity_RatioForm
                {
                    date = Product.ProductName,
                    quantity = product.Value
                });
            }

            return Ok(NumberOrderComplete);
        }

    }
}
