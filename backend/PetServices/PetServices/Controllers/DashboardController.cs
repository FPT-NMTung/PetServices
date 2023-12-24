using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Authorization;
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
            int currentYear = DateTime.Now.Year;
            var customerNumber = await _context.Accounts.Where(a => a.CreateDate.Value.Month == currentMonth
            && a.CreateDate.Value.Year == currentYear && a.RoleId == 2
            ).ToListAsync();

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

            var customerNumber = await _context.Accounts.Where(a => a.CreateDate.Value.Month == currentMonth && 
            a.CreateDate.Value.Year == currentYear && a.RoleId == 2).ToListAsync();
            var customerNumberPreviousMonth = await _context.Accounts.Where(a => a.CreateDate.Value.Month == previousMonth
            && a.CreateDate.Value.Year == newYear && a.RoleId == 2).ToListAsync();

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

            var numberOrder = await _context.Orders.Where(o => o.OrderDate.Value.Month == currentMonth && 
            o.OrderDate.Value.Year == currentYear && o.OrderStatus == "Completed").ToListAsync();

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

            for (int i = 6; i >= 0; i--)
            {
                DateTime date = now.AddDays(-i);
                double total = 0;

                var orders = await _context.Orders.Where(o => o.OrderDate.Value.Date == date.Date && o.OrderStatus == "Completed").ToListAsync();

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

            for (int i = 6; i >= 0; i--)
            {
                DateTime date = now.AddDays(-i);
                double total = 0;

                var orders = await _context.Orders.Where(o => o.OrderDate.Value.Date == date.Date && o.OrderStatus == "Completed").ToListAsync();

                foreach (var order in orders)
                {
                    var products = await _context.OrderProductDetails.Where(b => b.OrderId == order.OrderId).ToListAsync();

                    foreach (var product in products)
                    {
                        total += (product.Price ?? 0) * (product.Quantity ?? 0);
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

            for (int i = 6; i >= 0; i--)
            {
                DateTime date = now.AddDays(-i);
                double total = 0;

                var orders = await _context.Orders.Where(o => o.OrderDate.Value.Date == date.Date && o.OrderStatus == "Confirmed").ToListAsync();

                foreach (var order in orders)
                {
                    var rooms = await _context.BookingRoomDetails.Where(b => b.OrderId == order.OrderId).ToListAsync();


                    foreach (var room in rooms)
                    {
                        total += room.TotalPrice ?? 0;
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
        [HttpGet("GetNumberOrderCancelledInMonth")]
        public async Task<ActionResult> GetNumberOrderCancelledInMonth()
        {
            DateTime now = DateTime.Now;

            var NumberOrderComplete = new List<Quantity_RatioForm>();

            for (int i = 0; i < 3; i++)
            {
                var currentMonth = now.AddMonths(-i);
                var previousMonth = currentMonth.AddMonths(-1);

                var orders = await _context.Orders
                    .Where(o => o.OrderStatus == "Cancelled" &&
                                o.OrderDate.Value.Month == currentMonth.Month &&
                                o.OrderDate.Value.Year == currentMonth.Year)
                    .ToListAsync();

                var ordersPrevious = await _context.Orders
                    .Where(o => o.OrderStatus == "Cancelled" &&
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


        // Top 5 phòng được book nhiều nhất trong tháng 
        [HttpGet("GetTop5RoomBooking")]
        public async Task<ActionResult> GetTop5RoomBooking()
        {
            DateTime now = DateTime.Now;

            var NumberOrderComplete = new List<Quantity_RatioForm>();

            // Lấy ra danh sách các đơn đặt hàng trong tháng hiện tại đã hoàn thành
            var orders = await _context.Orders
                .Include(o => o.BookingRoomDetails)
                .Where(o => o.OrderStatus == "Confirmed" && o.OrderDate.Value.Month == now.Month && o.OrderDate.Value.Year == now.Year)
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

            if (roomIds.Count - topRooms.Count > 0)
            {
                NumberOrderComplete.Insert(0, new Quantity_RatioForm
                {
                    date = "Phòng khác",
                    quantity = roomIds.Count - topRooms.Count
                });
            }
            else
            {
                NumberOrderComplete.Insert(0, new Quantity_RatioForm
                {
                    date = "Tất cả phòng",
                    quantity = roomIds.Count
                });
            }

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

        // Top 5 dịch vụ được book nhiều nhất trong tháng 
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

            if (serviceIds.Count - topServices.Count > 0)
            {
                NumberOrderComplete.Insert(0, new Quantity_RatioForm
                {
                    date = "Dịch vụ khác",
                    quantity = serviceIds.Count - topServices.Count
                });
            }
            else
            {
                NumberOrderComplete.Insert(0, new Quantity_RatioForm
                {
                    date = "Tổng các dịch vụ",
                    quantity = serviceIds.Count
                });
            }

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

        // Top 5 sản phẩm bán chạy nhất trong tháng 
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

            var productQuantities = new Dictionary<int, int>();
            int totalQuantity = 0;

            foreach (var order in orders)
            {
                foreach (var orderProductDetail in order.OrderProductDetails)
                {
                    if (productQuantities.ContainsKey(orderProductDetail.ProductId))
                    {
                        productQuantities[orderProductDetail.ProductId] += orderProductDetail.Quantity ?? 0;
                    }
                    else
                    {
                        productQuantities[orderProductDetail.ProductId] = orderProductDetail.Quantity ?? 0;
                    }

                    totalQuantity += orderProductDetail.Quantity ?? 0;
                }
            }

            // Sắp xếp các sản phẩm theo số lượng đơn đặt hàng giảm dần
            var topProduct = productQuantities
                .OrderByDescending(o => o.Value)
                .Take(5)
                .ToList();

            foreach (var product in topProduct)
            {
                var Product = await _context.Products.FirstOrDefaultAsync(o => o.ProductId == product.Key);

                NumberOrderComplete.Add(new Quantity_RatioForm
                {
                    date = Product.ProductName,
                    quantity = (int)Math.Floor(((double)product.Value / totalQuantity) * 100),
                });
            }

            NumberOrderComplete.Insert(0, new Quantity_RatioForm
            {
                quantity = totalQuantity
            });

            return Ok(NumberOrderComplete);
        }


        // Top 5 khu vực đông khách hàng nhất 
        [HttpGet("GetTop5CustomerArea")]
        public async Task<ActionResult> GetTop5CustomerArea()
        {
            // Lấy danh sách khách hàng
            var listCustomer = await _context.Accounts
                .Include(a => a.UserInfo)
                .Where(account => account.RoleId == 2)
                .ToListAsync();

            // Lấy danh sách thông tin khách hàng 
            var userInfoIds = listCustomer
                .Select(account => account.UserInfoId)
                .ToList();

            // Lấy danh sách khách hàng có UserInfoId nằm trong danh sách userInfoIds
            var userInfos = _context.UserInfos
                .Where(userInfo => userInfoIds.Contains(userInfo.UserInfoId) && userInfo.Province != null)
                .ToList();

            // Đếm số lượng khách hàng trong 1 thành phố
            var provinceCounts = userInfos
                .GroupBy(userInfo => userInfo.Province)
                .Select(group => new
                {
                    Province = group.Key,
                    Quantity = group.Count()
                })
                .OrderByDescending(item => item.Quantity)
                .Take(5)
                .ToList();


            var NumberOrderComplete = provinceCounts.Select(item => new Quantity_RatioForm
            {
                date = item.Province,
                quantity = item.Quantity,
            }).ToList();

            return Ok(NumberOrderComplete);
        }

        // đánh giá của khách hàng về các sản phẩm
        [HttpGet("GetFeedbackOfProduct")]
        public async Task<ActionResult> GetFeedbackOfProduct()
        {
            //lấy danh sách đánh giá các sản phẩm 
            var listFeedback = await _context.Feedbacks.Where(o => o.ProductId != null).ToListAsync();
            var stt = 1;

            var feedback = new List<FeedbackForm>();

            foreach (var Feedback in listFeedback)
            {
                var product = await _context.Products.FirstOrDefaultAsync(o => o.ProductId == Feedback.ProductId);
                var customer = await _context.UserInfos.FirstOrDefaultAsync(o => o.UserInfoId == Feedback.UserId);
                var account = await _context.Accounts.FirstOrDefaultAsync(o => o.UserInfoId == Feedback.UserId);


                feedback.Add(new FeedbackForm
                {
                    stt = stt,
                    name = product.ProductName,
                    gmail = account.Email,
                    picture = product.Picture,
                    customerName = customer.FirstName + customer.LastName,
                    NumberStart = Feedback.NumberStart,
                    Content = Feedback.Content,
                });

                stt++;
            }

            return Ok(feedback);
        }

        // đánh giá của khách hàng về các phòng
        [HttpGet("GetFeedbackOfRoom")]
        public async Task<ActionResult> GetFeedbackOfRoom()
        {
            //lấy danh sách đánh giá các phòng 
            var listFeedback = await _context.Feedbacks.Where(o => o.RoomId != null).ToListAsync();
            var stt = 1;

            var feedback = new List<FeedbackForm>();

            foreach (var Feedback in listFeedback)
            {
                var room = await _context.Rooms.FirstOrDefaultAsync(o => o.RoomId == Feedback.RoomId);
                var customer = await _context.UserInfos.FirstOrDefaultAsync(o => o.UserInfoId == Feedback.UserId);
                var account = await _context.Accounts.FirstOrDefaultAsync(o => o.UserInfoId == Feedback.UserId);

                feedback.Add(new FeedbackForm
                {
                    stt = stt,
                    name = room.RoomName,
                    picture = room.Picture,
                    gmail = account.Email,
                    customerName = customer.FirstName + customer.LastName,
                    NumberStart = Feedback.NumberStart,
                    Content = Feedback.Content,
                });

                stt++;
            }

            return Ok(feedback);
        }

        // đánh giá của khách hàng về các dịch vụ
        [HttpGet("GetFeedbackOfService")]
        public async Task<ActionResult> GetFeedbackOfService()
        {
            //lấy danh sách đánh giá các dịch vụ
            var listFeedback = await _context.Feedbacks.Where(o => o.ServiceId != null && o.UserId != null).ToListAsync();
            var stt = 1;

            var feedback = new List<FeedbackForm>();

            foreach (var Feedback in listFeedback)
            {
                var service = await _context.Services.FirstOrDefaultAsync(o => o.ServiceId == Feedback.ServiceId);
                var customer = await _context.UserInfos.FirstOrDefaultAsync(o => o.UserInfoId == Feedback.UserId);
                var account = await _context.Accounts.FirstOrDefaultAsync(o => o.UserInfoId == Feedback.UserId);

                feedback.Add(new FeedbackForm
                {
                    stt = stt,
                    name = service.ServiceName ?? null,
                    picture = service.Picture ?? null,
                    gmail = account.Email,
                    customerName = customer.FirstName + customer.LastName ?? null,
                    NumberStart = Feedback.NumberStart,
                    Content = Feedback.Content,
                });

                stt++;
            }

            return Ok(feedback);
        }
    }
}
