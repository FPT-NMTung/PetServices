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
    public class OrderController : ControllerBase   
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public OrderController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        #region Get
        [HttpGet("getOrder")]
        public IActionResult GetOrder()
        {
            try
            {
                List<Order> orders = _context.Orders
                    .Include(o => o.UserInfo)
                        .ThenInclude(u => u.Accounts)
                    .Include(b => b.OrderProductDetails)
                        .ThenInclude(o => o.Product)
                    .Include(b => b.BookingServicesDetails)
                        .ThenInclude(bs => bs.Service)
                     .Include(b => b.BookingServicesDetails)
                            .ThenInclude(s => s.PartnerInfo)
                    .Include(b => b.BookingRoomDetails)
                        .ThenInclude(br => br.Room)
                    .Include(b => b.BookingRoomServices)
                        .ThenInclude(br => br.Service)
                        .Include(o => o.ReasonOrders)
                    .Where(o => o.BookingRoomDetails.Count() == 0 && (o.BookingServicesDetails.Count() > 0 || o.OrderProductDetails.Count() > 0))
                    .OrderByDescending(o => o.OrderDate)
                .ToList();
                return Ok(_mapper.Map<List<OrdersDTO>>(orders));
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getOrderRoom")]
        public IActionResult GetOrderRoom()
        {
            try
            {
                List<Order> orders = _context.Orders
                    .Include(o => o.UserInfo)
                        .ThenInclude(u => u.Accounts)
                    .Include(b => b.OrderProductDetails)
                        .ThenInclude(o => o.Product)
                    .Include(b => b.BookingServicesDetails)
                        .ThenInclude(bs => bs.Service)
                     .Include(b => b.BookingServicesDetails)
                            .ThenInclude(s => s.PartnerInfo)
                    .Include(b => b.BookingRoomDetails)
                        .ThenInclude(br => br.Room)
                    .Include(b => b.BookingRoomServices)
                        .ThenInclude(br => br.Service)
                        .Include(o => o.ReasonOrders)
                    .Where(o => o.BookingRoomDetails.Count() > 0 && o.BookingServicesDetails.Count() == 0 && o.OrderProductDetails.Count() == 0)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();
                return Ok(_mapper.Map<List<OrdersDTO>>(orders));
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("latest")]
        public IActionResult GetLatestOrder(string email)
        {
            try
            {
                // Lấy đơn đặt hàng mới nhất cho người dùng có email tương ứng
                Order latestOrder = _context.Orders
                     .Include(o => o.UserInfo)
                        .ThenInclude(u => u.Accounts)
                    .Include(b => b.OrderProductDetails)
                        .ThenInclude(o => o.Product)
                    .Include(b => b.BookingServicesDetails)
                        .ThenInclude(bs => bs.Service)
                     .Include(b => b.BookingServicesDetails)
                            .ThenInclude(s => s.PartnerInfo)
                    .Include(b => b.BookingRoomDetails)
                        .ThenInclude(br => br.Room)
                    .Include(b => b.BookingRoomServices)
                        .ThenInclude(br => br.Service)
                    .Where(o => o.UserInfo.Accounts.Any(a => a.Email == email))
                    .OrderByDescending(o => o.OrderId)
                    .FirstOrDefault();

                if (latestOrder != null)
                {
                    // Trả về toàn bộ thông tin đơn đặt hàng
                    return Ok(_mapper.Map<OrdersDTO>(latestOrder));
                }
                else
                {
                    return NotFound(new { Message = "No orders found for the specified email" });
                }
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getOrderUser/{email}")]
        public IActionResult GetOrderUser(string email, string orderstatus, int page = 1, int pageSize = 5)
        {
            try
            {
                IQueryable<Order> query = _context.Orders
                    .Include(o => o.UserInfo)
                        .ThenInclude(u => u.Accounts)
                    .Include(b => b.OrderProductDetails)
                        .ThenInclude(o => o.Product)
                    .Include(b => b.BookingServicesDetails)
                        .ThenInclude(bs => bs.Service)
                     .Include(b => b.BookingServicesDetails)
                            .ThenInclude(s => s.PartnerInfo)
                    .Include(b => b.BookingRoomDetails)
                        .ThenInclude(br => br.Room)
                    .Include(b => b.BookingRoomServices)
                        .ThenInclude(br => br.Service)
                    .Include(o => o.ReasonOrders)
                    .Where(o => o.UserInfo.Accounts.Any(a => a.Email == email) &&
                    o.BookingRoomDetails.Count() == 0 && (o.BookingServicesDetails.Count() > 0 || o.OrderProductDetails.Count() > 0)
                    );

                if (!string.IsNullOrEmpty(orderstatus) && orderstatus.ToLower() != "all")
                {
                    query = query.Where(o => o.OrderStatus == orderstatus);
                }

                query = query.OrderByDescending(o => o.OrderDate);

                var paginatedOrders = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                bool hasOrders = paginatedOrders.Any();

                if (hasOrders)
                {
                    List<OrdersDTO> ordersDTOList = _mapper.Map<List<OrdersDTO>>(paginatedOrders);
                    return Ok(ordersDTOList);
                }
                else
                {
                    return NotFound("No orders found with the specified status");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetOrderUserNoneFeedback/{email}")]
        public IActionResult GetOrderUserNoneFeedback(string email, int page = 1, int pageSize = 5)
        {
            try
            {
                IQueryable<Order> query = _context.Orders
                    .Include(o => o.UserInfo)
                        .ThenInclude(u => u.Accounts)
                    .Include(b => b.OrderProductDetails)
                        .ThenInclude(o => o.Product)
                    .Include(b => b.BookingServicesDetails)
                        .ThenInclude(bs => bs.Service)
                     .Include(b => b.BookingServicesDetails)
                            .ThenInclude(s => s.PartnerInfo)
                    .Include(b => b.BookingRoomDetails)
                        .ThenInclude(br => br.Room)
                    .Include(b => b.BookingRoomServices)
                        .ThenInclude(br => br.Service)
                    .Include(o => o.ReasonOrders)
                    .Where(o => o.UserInfo.Accounts.Any(a => a.Email == email) &&
                    o.BookingRoomDetails.Count() == 0 && (o.BookingServicesDetails.Count() > 0 || o.OrderProductDetails.Count() > 0)
                    && (o.BookingServicesDetails.Any(bs => bs.FeedbackStatus == null) || o.OrderProductDetails.Any(bs => bs.FeedbackStatus == null)
                    || o.BookingServicesDetails.Any(bs => bs.FeedbackPartnerStatus == null))
                    );

                query = query.Where(o => o.OrderStatus == "Completed");

                query = query.OrderByDescending(o => o.OrderDate);

                var paginatedOrders = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                bool hasOrders = paginatedOrders.Any();

                if (hasOrders)
                {
                    List<OrdersDTO> ordersDTOList = _mapper.Map<List<OrdersDTO>>(paginatedOrders);
                    return Ok(ordersDTOList);
                }
                else
                {
                    return NotFound("No orders found with the specified status");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getRoomOrderUser/{email}")]
        public IActionResult GetRoomOrderUser(string email, string orderstatus, int page = 1, int pageSize = 5)
        {
            try
            {
                IQueryable<Order> query = _context.Orders
                   .Include(o => o.UserInfo)
                        .ThenInclude(u => u.Accounts)
                    .Include(b => b.OrderProductDetails)
                        .ThenInclude(o => o.Product)
                    .Include(b => b.BookingServicesDetails)
                        .ThenInclude(bs => bs.Service)
                     .Include(b => b.BookingServicesDetails)
                            .ThenInclude(s => s.PartnerInfo)
                    .Include(b => b.BookingRoomDetails)
                        .ThenInclude(br => br.Room)
                    .Include(b => b.BookingRoomServices)
                        .ThenInclude(br => br.Service)
                        .Include(o => o.ReasonOrders)
                    .Where(o => o.UserInfo.Accounts.Any(a => a.Email == email) &&
                    o.BookingRoomDetails.Count() > 0 && o.BookingServicesDetails.Count() == 0 && o.OrderProductDetails.Count() == 0 
                    );

                if (!string.IsNullOrEmpty(orderstatus) && orderstatus.ToLower() != "all")
                {
                    query = query.Where(o => o.OrderStatus == orderstatus);
                }

                query = query.OrderByDescending(o => o.OrderDate);

                var paginatedOrders = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                bool hasOrders = paginatedOrders.Any();

                if (hasOrders)
                {
                    List<OrdersDTO> ordersDTOList = _mapper.Map<List<OrdersDTO>>(paginatedOrders);
                    return Ok(ordersDTOList);
                }
                else
                {
                    return NotFound("No orders found with the specified status");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getRoomOrderUserNoneFeedback/{email}")]
        public IActionResult getRoomOrderUserNoneFeedback(string email, int page = 1, int pageSize = 5)
        {
            try
            {
                IQueryable<Order> query = _context.Orders
                   .Include(o => o.UserInfo)
                        .ThenInclude(u => u.Accounts)
                    .Include(b => b.OrderProductDetails)
                        .ThenInclude(o => o.Product)
                    .Include(b => b.BookingServicesDetails)
                        .ThenInclude(bs => bs.Service)
                     .Include(b => b.BookingServicesDetails)
                            .ThenInclude(s => s.PartnerInfo)
                    .Include(b => b.BookingRoomDetails)
                        .ThenInclude(br => br.Room)
                    .Include(b => b.BookingRoomServices)
                        .ThenInclude(br => br.Service)
                        .Include(o => o.ReasonOrders)
                    .Where(o => o.UserInfo.Accounts.Any(a => a.Email == email) &&
                    o.BookingRoomDetails.Count() > 0 && o.BookingServicesDetails.Count() == 0 && o.OrderProductDetails.Count() == 0 &&
                    o.BookingRoomDetails.Any(bs => bs.FeedbackStatus == null)
                    );

                query = query.Where(o => o.OrderStatus == "Confirmed");

                query = query.OrderByDescending(o => o.OrderDate);

                var paginatedOrders = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                bool hasOrders = paginatedOrders.Any();

                if (hasOrders)
                {
                    List<OrdersDTO> ordersDTOList = _mapper.Map<List<OrdersDTO>>(paginatedOrders);
                    return Ok(ordersDTOList);
                }
                else
                {
                    return NotFound("No orders found with the specified status");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("orderstatus/{orderstatus}")]
        public IActionResult CheckStatusOrder(string email, string orderstatus)
        {
            try
            {
                if (orderstatus != "All")
                {
                    List<Order> orders = _context.Orders
                                        .Include(o => o.UserInfo)
                                            .ThenInclude(u => u.Accounts)
                                        .Include(b => b.OrderProductDetails)
                                            .ThenInclude(o => o.Product)
                                        .Include(b => b.BookingServicesDetails)
                                            .ThenInclude(bs => bs.Service)
                                         .Include(b => b.BookingServicesDetails)
                                                .ThenInclude(s => s.PartnerInfo)
                                        .Include(b => b.BookingRoomDetails)
                                            .ThenInclude(br => br.Room)
                                        .Include(b => b.BookingRoomServices)
                                            .ThenInclude(br => br.Service)
                                         .Where(o => o.UserInfo.Accounts.Any(a => a.Email == email) 
                                         && o.OrderStatus == orderstatus 
                                         && o.BookingRoomDetails.Count() == 0 
                                         && (o.BookingServicesDetails.Count() > 0 
                                         || o.OrderProductDetails.Count() > 0
                                         )).ToList();

                    if (orders.Count == 0 )
                    {
                        return NotFound("No orders found with the specified status");
                    }
                    else 
                    {
                        return Ok();
                    }
                }
                else
                {
                    List<Order> orders = _context.Orders
                                         .Where(o => o.UserInfo.Accounts.Any(a => a.Email == email) 
                                         && o.BookingRoomDetails.Count() == 0
                                         && (o.BookingServicesDetails.Count() > 0
                                         || o.OrderProductDetails.Count() > 0
                                         )).ToList();
                    if (orders.Count == 0)
                    {
                        return NotFound("No orders found with the specified status");
                    }
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("orderroomstatus/{orderstatus}")]
        public IActionResult CheckRoomStatusOrder(string email, string orderstatus)
        {
            try
            {
                if (orderstatus != "All")
                {
                    List<Order> ordersRoom = _context.Orders
                                        .Include(o => o.UserInfo)
                                            .ThenInclude(u => u.Accounts)
                                        .Include(b => b.OrderProductDetails)
                                            .ThenInclude(o => o.Product)
                                        .Include(b => b.BookingServicesDetails)
                                            .ThenInclude(bs => bs.Service)
                                         .Include(b => b.BookingServicesDetails)
                                                .ThenInclude(s => s.PartnerInfo)
                                        .Include(b => b.BookingRoomDetails)
                                            .ThenInclude(br => br.Room)
                                        .Include(b => b.BookingRoomServices)
                                            .ThenInclude(br => br.Service)
                                         .Where(o => o.UserInfo.Accounts.Any(a => a.Email == email)
                                         && o.OrderStatus == orderstatus
                                         && o.BookingRoomDetails.Count() > 0
                                         && (o.BookingServicesDetails.Count() == 0
                                         && o.OrderProductDetails.Count() == 0
                                         )).ToList();
                    if (ordersRoom.Count == 0)
                    {
                        return NotFound("No orders found with the specified status");
                    }
                    else
                    {
                        return Ok();
                    }
                }
                else
                {
                    List<Order> orders = _context.Orders
                                         .Where(o => o.UserInfo.Accounts.Any(a => a.Email == email)
                                          && o.BookingRoomDetails.Count() > 0
                                         && (o.BookingServicesDetails.Count() == 0
                                         && o.OrderProductDetails.Count() == 0
                                         )).ToList();
                    if (orders.Count == 0)
                    {
                        return NotFound("No orders found with the specified status");
                    }
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetOrder(int Id)
        {
            try
            {
                Order order = await _context.Orders
                    .Include(o => o.UserInfo)
                        .ThenInclude(u => u.Accounts)
                    .Include(b => b.OrderProductDetails)
                        .ThenInclude(o => o.Product)
                    .Include(b => b.BookingServicesDetails)
                        .ThenInclude(bs => bs.Service)
                     .Include(b => b.BookingServicesDetails)
                            .ThenInclude(s => s.PartnerInfo)
                    .Include(b => b.BookingRoomDetails)
                        .ThenInclude(br => br.Room)
                    .Include(b => b.BookingRoomServices)
                        .ThenInclude(br => br.Service)
                        .Include(o => o.ReasonOrders)
                    .SingleOrDefaultAsync(b => b.OrderId == Id);

                return Ok(_mapper.Map<OrdersDTO>(order));
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Put
        [HttpPut("changeStatus")]
        public async Task<IActionResult> ChangeStatus(int Id, [FromBody] Status status)
        {
            try
            {
                Order order = await _context.Orders
                    .Include(o => o.OrderProductDetails)
                    .Include(o => o.BookingServicesDetails)
                    .Include(o => o.BookingRoomDetails)
                    .Include(o => o.BookingServicesDetails)
                    .SingleOrDefaultAsync(b => b.OrderId == Id);

                // Kiểm tra booking có tồn tại hay không
                if (order == null)
                {
                    return NotFound("Order không tồn tại");
                }

                // Kiểm tra xem trạng thái cũ có chính xác hay không
                if (order.OrderStatus.Trim() != status.oldStatus)
                {
                    return BadRequest("Trạng thái cũ không hợp lệ");
                }

                order.OrderStatus = status.newStatus;

                UpdateProductDetailsStatus(order, status.newStatusProduct);
                UpdateServiceDetailsStatus(order, status.newStatusService);

                if (order.OrderProductDetails.Count() > 0 && 
                    order.BookingServicesDetails.Count() == 0)
                {
                    foreach (var dto in order.OrderProductDetails)
                    {
                        if (status.newStatusProduct == "Delivered")
                        {
                            if (order.StatusPayment == false)
                            {
                                order.StatusPayment = !order.StatusPayment;
                            }
                            order.OrderStatus = "Completed";
                        }
                    }
                }

                if (order.OrderProductDetails.Count() == 0
                    && order.BookingServicesDetails.Count() > 0)
                {
                    foreach (var dto in order.BookingServicesDetails)
                    {
                        if (status.newStatusService == "Completed")
                        {
                            if (order.StatusPayment == false)
                            {
                                order.StatusPayment = !order.StatusPayment;
                            }
                            order.OrderStatus = "Completed";
                        }
                    }
                }

                if (order.OrderProductDetails.Count() > 0
                    && order.BookingServicesDetails.Count() > 0)
                {
                    int checkProduct = -1;
                    int checkService = -1;
                    foreach (var dto in order.OrderProductDetails)
                    {
                        if (status.newStatusProduct == "Delivered" || dto.StatusOrderProduct == "Delivered")
                        {
                            checkProduct = 0;
                        }
                        else if (status.newStatusProduct == "Cancelled" || dto.StatusOrderProduct == "Cancelled")
                        {
                            checkProduct = 4;
                        }
                        else
                        {
                            checkProduct = 1;
                        }
                    }

                    foreach (var dto in order.BookingServicesDetails)
                    {
                        if (status.newStatusService == "Completed" || dto.StatusOrderService == "Completed")
                        {
                            checkService = 0;
                        }
                        else if (status.newStatusService == "Cancelled" || dto.StatusOrderService == "Cancelled")
                        {
                            checkService = 4;
                        }
                        else
                        {
                            checkService = 1;
                        }

                    }

                    if (checkProduct == 0 && checkService == 0)
                    {
                        if (order.StatusPayment == false)
                        {
                            order.StatusPayment = !order.StatusPayment;
                        }
                        order.OrderStatus = "Completed";
                    }

                    if (checkProduct == 0 && checkService == 4)
                    {
                        if (order.StatusPayment == false)
                        {
                            order.StatusPayment = !order.StatusPayment;
                        }
                        order.OrderStatus = "Completed";
                    }

                    if (checkProduct == 4 && checkService == 0)
                    {
                        if (order.StatusPayment == false)
                        {
                            order.StatusPayment = !order.StatusPayment;
                        }
                        order.OrderStatus = "Completed";
                    }

                    if (checkProduct == 4 && checkService == 4)
                    {
                        order.OrderStatus = "Cancelled";
                    }

                    if (checkProduct == 0 && checkService == 1)
                    {
                        order.OrderStatus = order.OrderStatus;
                    }

                    if (checkProduct == 1 && checkService == 0)
                    {
                        order.OrderStatus = order.OrderStatus;
                    }
                }

                if (order.OrderProductDetails.Count() == 0
                    && order.BookingServicesDetails.Count() == 0    
                    && order.BookingRoomDetails.Count() > 0)
                {
                    foreach (var dto in order.BookingRoomDetails)
                    {
                        if (status.newStatus == "Confirmed")
                        {
                            if (order.StatusPayment == false)
                            {
                                order.StatusPayment = !order.StatusPayment;
                            }
                        }
                    }
                }

                _context.Orders.Update(order);
                _context.OrderProductDetails.UpdateRange(order.OrderProductDetails);
                _context.BookingServicesDetails.UpdateRange(order.BookingServicesDetails);
                _context.BookingRoomDetails.UpdateRange(order.BookingRoomDetails);
                _context.BookingRoomServices.UpdateRange(order.BookingRoomServices);
                await _context.SaveChangesAsync();
                return Ok("Đổi trạng thái thành công");
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }

        private void UpdateProductDetailsStatus(Order order, string newStatusProduct)
        {
            if (order.OrderProductDetails != null && !string.IsNullOrEmpty(newStatusProduct))
            {
                foreach (var dto in order.OrderProductDetails)
                {
                    dto.StatusOrderProduct = newStatusProduct;
                }
            }
        }

        private void UpdateServiceDetailsStatus(Order order, string newStatusService)
        {
            if (order.BookingServicesDetails != null && !string.IsNullOrEmpty(newStatusService))
            {
                foreach (var dto in order.BookingServicesDetails)
                {
                    dto.StatusOrderService = newStatusService;
                }
            }
        }

        [HttpPut("changeStatusPayment")]
        public async Task<IActionResult> ChangeStatusPayment(int Id)
        {
            try
            {
                Order order = await _context.Orders.SingleOrDefaultAsync(b => b.OrderId == Id);
                // Kiểm tra booking có tồn tại hay không
                if (order == null)
                {
                    return NotFound("Booking không tồn tài");
                }
                // Kiểm tra xem trạng thái cũ có chính xác hay không

                order.StatusPayment = !order.StatusPayment;
                _context.Orders.Update(order);

                await _context.SaveChangesAsync();
                return Ok("Đổi trạng thái thành công");
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("updateAllTotalPrice")]
        public async Task<IActionResult> UpdateAllTotalPrice(int Id, double allTotalPrice)
        {
            try
            {
                Order order = await _context.Orders.SingleOrDefaultAsync(b => b.OrderId == Id);
                // Kiểm tra booking có tồn tại hay không
                if (order == null)
                {
                    return NotFound("Booking không tồn tài");
                }
                // Kiểm tra xem trạng thái cũ có chính xác hay không

                order.TotalPrice = allTotalPrice;
                _context.Orders.Update(order);

                await _context.SaveChangesAsync();
                return Ok("Thay đổi giá tiền thành công");
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }
        #endregion  

        #region Post
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrdersDTO orderDTO)
        {
            if (orderDTO == null)
            {
                return BadRequest("Invalid order data");
            }
            // check tỉnh
            if (string.IsNullOrWhiteSpace(orderDTO.Province))
            {
                string errorMessage = "Tỉnh không được để trống!";
                return BadRequest(errorMessage);
            }
            if (!Regex.IsMatch(orderDTO.Province, "^[a-zA-ZÀ-Ỹà-ỹ ]+$"))
            {
                string errorMessage = "Tỉnh phải là ký tự chữ, không chấp nhận số hay ký tự đặc biệt!";
                return BadRequest(errorMessage);
            }
            if (orderDTO.Province.Length > 50)
            {
                string errorMessage = "Tỉnh vượt quá số ký tự. Tối đa 50 ký tự!";
                return BadRequest(errorMessage);
            }
            // check huyện
            if (string.IsNullOrWhiteSpace(orderDTO.District))
            {
                string errorMessage = "Huyện/Thành Phố không được để trống!";
                return BadRequest(errorMessage);
            }
            if (!Regex.IsMatch(orderDTO.District, "^[a-zA-ZÀ-Ỹà-ỹ ]+$"))
            {
                string errorMessage = "Huyện/Thành phố phải là ký tự chữ, không chấp nhận số hay ký tự đặc biệt!";
                return BadRequest(errorMessage);
            }
            if (orderDTO.District.Length > 50)
            {
                string errorMessage = "Huyện/Thành Phố vượt quá số ký tự. Tối đa 50 ký tự!";
                return BadRequest(errorMessage);
            }
            // check xã
            if (string.IsNullOrWhiteSpace(orderDTO.Commune))
            {
                string errorMessage = "Phường/Xã không được để trống!";
                return BadRequest(errorMessage);
            }
            if (!Regex.IsMatch(orderDTO.Commune, "^[a-zA-ZÀ-Ỹà-ỹ ]+$"))
            {
                string errorMessage = "Phường/Xã phải là ký tự chữ, không chấp nhận số hay ký tự đặc biệt!";
                return BadRequest(errorMessage);
            }
            if (orderDTO.Commune.Length > 50)
            {
                string errorMessage = "Phường/Xã vượt quá số ký tự. Tối đa 50 ký tự!";
                return BadRequest(errorMessage);
            }
            // check địa chỉ
            if (string.IsNullOrWhiteSpace(orderDTO.Address))
            {
                string errorMessage = "Địa chỉ không được để trống!";
                return BadRequest(errorMessage);
            }
            if (orderDTO.Address.Length > 500)
            {
                string errorMessage = "Địa chỉ vượt quá số ký tự. Tối đa 500 ký tự!";
                return BadRequest(errorMessage);
            }
            // chech sđt
            if (string.IsNullOrWhiteSpace(orderDTO.Phone))
            {
                string errorMessage = "Số điện thoại không được để trống!";
                return BadRequest(errorMessage);
            }
            if (orderDTO.Phone.Length != 10)
            {
                string errorMessage = "Số điện thoại phải có 10 ký tự!";
                return BadRequest(errorMessage);
            }
            if (orderDTO.Phone.Contains(" "))
            {
                string errorMessage = "Số điện thoại không được chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            if (!orderDTO.Phone.StartsWith("0"))
            {
                string errorMessage = "Số điện thoại phải bắt đầu bằng số 0!";
                return BadRequest(errorMessage);
            }
            if (!int.TryParse(orderDTO.Phone, out int phoneNumber))
            {
                string errorMessage = "Số điện thoại không phải là số! Bạn cần nhập số điện thoại ở dạng số!";
                return BadRequest(errorMessage);
            }
            // check FullName
            if (string.IsNullOrWhiteSpace(orderDTO.FullName))
            {
                string errorMessage = "Tên liên hệ không được để trống!";
                return BadRequest(errorMessage);
            }
            string fullName = orderDTO.FullName;
            if (!Regex.IsMatch(fullName, "^[a-zA-ZÀ-Ỹà-ỹ ]+$"))
            {
                string errorMessage = "Tên liên hệ chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.";
                return BadRequest(errorMessage);
            }
            // check quantity và product
            if (orderDTO.OrderProductDetails != null)
            {
                var invalidProducts = orderDTO.OrderProductDetails
                    .Where(dto => dto.Quantity <= 0 || dto.ProductId == null || !_context.Products.Any(p => p.ProductId == dto.ProductId))
                    .ToList();

                if (invalidProducts.Any())
                {
                    var errorMessage = "Sản phẩm không hợp lệ. ";

                    // check quantity = 0
                    var quantityErrors = invalidProducts
                        .Where(dto => dto.Quantity <= 0)
                        .Select(dto => $"Số lượng không hợp lệ cho sản phẩm với ID {dto.ProductId}");

                    // Check product k tồn tại
                    var productIdErrors = invalidProducts
                        .Where(dto => dto.ProductId == null || !_context.Products.Any(p => p.ProductId == dto.ProductId))
                        .Select(dto => $"Sản phẩm với ID {dto.ProductId} không tồn tại");

                    errorMessage += string.Join(". ", quantityErrors.Concat(productIdErrors));

                    return BadRequest(errorMessage);
                }

                // check quantity mua > quantity có sẵn
                var quantityExceedsAvailable = orderDTO.OrderProductDetails
                    .Any(dto => dto.Quantity > _context.Products.FirstOrDefault(p => p.ProductId == dto.ProductId)?.Quantity);

                if (quantityExceedsAvailable)
                {
                    return BadRequest("Số lượng sản phẩm không đủ");
                }
            }

            double priceProduct = 0;
            double priceRoom = 0;
            double allTotalPrice = 0;

            if (orderDTO.OrderProductDetails != null)
            {
                var productIds = orderDTO.OrderProductDetails.Where(dto => dto != null).Select(dto => dto.ProductId).ToList();
                var takeProduct = _context.Products.FirstOrDefault(p => productIds.Contains(p.ProductId));
                if (takeProduct != null)
                {
                    priceProduct = (double)takeProduct.Price;
                }

                allTotalPrice += (double)orderDTO.OrderProductDetails.Sum(dto => (dto.Quantity * priceProduct));
            }

            if (orderDTO.BookingRoomDetails != null)
            {
                var roomIds = orderDTO.BookingRoomDetails.Where(dto => dto != null).Select(dto => dto.RoomId).ToList();
                var takeRoom = _context.Rooms.FirstOrDefault(r => roomIds.Contains(r.RoomId));
                if (takeRoom != null)
                {
                    priceRoom = (double)takeRoom.Price;
                }

                allTotalPrice += (double)orderDTO.BookingRoomDetails.Sum(dto => (dto.TotalPrice));
            }

            if (orderDTO.BookingServicesDetails != null)
            {
                allTotalPrice += (double)orderDTO.BookingServicesDetails.Sum(dto => (dto.PriceService));
            }

            var order = new Order
            {
                OrderDate = orderDTO.OrderDate,
                OrderStatus = orderDTO.OrderStatus,
                Province = orderDTO.Province,
                District = orderDTO.District,
                Commune = orderDTO.Commune,
                Address = orderDTO.Address,
                UserInfoId = orderDTO.UserInfoId,
                Phone = orderDTO.Phone,
                FullName = orderDTO.FullName,
                TypePay = orderDTO.TypePay,
                StatusPayment = orderDTO.StatusPayment,
                TotalPrice = allTotalPrice,

                // Sản phẩm
                OrderProductDetails = orderDTO.OrderProductDetails != null
                  ? orderDTO.OrderProductDetails.Select(dto => new OrderProductDetail
                  {
                      Quantity = dto.Quantity,
                      Price = priceProduct,
                      ProductId = dto.ProductId,
                      StatusOrderProduct = dto.StatusOrderProduct,
                  }).ToList()
                : new List<OrderProductDetail>(),

                // Phòng
                BookingRoomDetails = orderDTO.BookingRoomDetails != null
                    ? orderDTO.BookingRoomDetails.Select(dto => new BookingRoomDetail
                    {
                        RoomId = dto.RoomId,
                        Price = priceRoom,
                        StartDate = dto.StartDate,
                        EndDate = dto.EndDate,
                        TotalPrice = dto.TotalPrice,
                        Note = dto.Note,
                    }).ToList()
                    : new List<BookingRoomDetail>(),

                BookingRoomServices = orderDTO.BookingRoomServices != null
                    ? orderDTO.BookingRoomServices.Select(dto => new BookingRoomService
                    {
                        RoomId = dto.RoomId,
                        ServiceId = dto.ServiceId,
                        PriceService = dto.PriceService,
                    }).ToList()
                    : new List<BookingRoomService>(),

                // Dịch vụ
                BookingServicesDetails = orderDTO.BookingServicesDetails != null
                    ? orderDTO.BookingServicesDetails.Select(dto => new BookingServicesDetail
                    {
                        ServiceId = dto.ServiceId,
                        Price = dto.Price,
                        Weight = dto.Weight,
                        PriceService = dto.PriceService,
                        PetInfoId = dto.PetInfoId,
                        StartTime = dto.StartTime,
                        EndTime = dto.EndTime,
                        PartnerInfoId = dto.PartnerInfoId,
                        StatusOrderService = dto.StatusOrderService,
                    }).ToList()
                    : new List<BookingServicesDetail>()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok("Order thành công!");
        }
        #endregion

        #region Delete
        [HttpDelete("delete/{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            try
            {
                Order order = await _context.Orders
                    .Include(o => o.OrderProductDetails)
                    .Include(o => o.BookingServicesDetails)
                    .Include(o => o.BookingRoomDetails)
                    .Include(o => o.BookingRoomServices)
                    .SingleOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    return NotFound(new { Message = $"Không tìm thấy order : {orderId}" });
                }

                _context.OrderProductDetails.RemoveRange(order.OrderProductDetails);
                _context.BookingServicesDetails.RemoveRange(order.BookingServicesDetails);
                _context.BookingRoomDetails.RemoveRange(order.BookingRoomDetails);
                _context.BookingRoomServices.RemoveRange(order.BookingRoomServices);

                _context.Orders.Remove(order);

                await _context.SaveChangesAsync();

                return Ok(new { Message = $"Xoá order thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

    }
}
