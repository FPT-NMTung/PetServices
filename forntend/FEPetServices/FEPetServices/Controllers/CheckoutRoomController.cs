using FEPetServices.Areas.DTO;
using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using FEPetServices.Models.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Controllers
{
    [Authorize(Policy = "CusOnly")]
    public class CheckoutRoomController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlUserInfo = "";
        private readonly IConfiguration _configuration;
        private readonly VnpConfiguration _vnpConfiguration;

        private readonly Utils _utils;
        public CheckoutRoomController(HttpClient client, IConfiguration configuration, Utils utils, VnpConfiguration vnpConfiguration)
        {
            _client = client;
            _configuration = configuration;
            _utils = utils;
            _vnpConfiguration = vnpConfiguration;

            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);

            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            /*DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Product";*/
            //DefaultApiUrl = "https://localhost:7255/api/";
        }
        public class CartItem
        {
            // Room
            public List<BookingRoomServiceForm> BookingRoomServices { get; set; }
            //
            public int RoomId { get; set; }
            public double? Price { get; set; }
            public string? Note { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public double? TotalPrice { get; set; }
            public virtual RoomDTO? Room { get; set; } = null!;
        }

        public const string CARTKEY = "cartRoom";
        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            }
            return new List<CartItem>();
        }
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }
        // push to test
        public async Task<IActionResult> Index(int RoomId, List<int> ServiceId, DateTime StartTime, DateTime EndTime, double totalPrice, string note)
        {
            ViewBag.Title = "Thanh toán";
            // Thônng tin người đặt hàng
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            AccountInfo userInfo = null;
            //HttpResponseMessage responseInfo = await _client.GetAsync(DefaultApiUrlUserInfo + "/" + email);
            HttpResponseMessage responseInfo = await _client.GetAsync(DefaultApiUrl + "UserInfo/" + email);
            if (responseInfo.IsSuccessStatusCode)
            {
                string responseContent = await responseInfo.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                 userInfo = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
            }

            // Cart
            ServiceDTO service = null;
            RoomDTO room = null;
            //Room
            //https://localhost:7255/api/Room/GetRoom/1
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Room/GetRoom/" + RoomId);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var option = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                room = System.Text.Json.JsonSerializer.Deserialize<RoomDTO>(responseContent, option);
            }

            if (room != null && RoomId > 0)
            {
                var cart = GetCartItems();
                var cartitem = cart.Find(s => s.Room != null && s.Room.RoomId == RoomId);

                if (cartitem != null)
                {
                    TempData["WatingToast"] = "Phòng đã có trong giỏ hàng.";
                    return RedirectToAction("Index", "CheckoutRoom");
                }
                else
                {
                    var bookingRoomServices = new List<BookingRoomServiceForm>();
                    foreach (var serviceId in ServiceId)
                    {
                        int a = serviceId;
                        //https://localhost:7255/api/Service/ServiceID/5
                        HttpResponseMessage responseService = await _client.GetAsync(DefaultApiUrl + "Service/ServiceID/" + serviceId);
                        if (responseService.IsSuccessStatusCode)
                        {
                            string responseContent = await responseService.Content.ReadAsStringAsync();
                            var option = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            };
                            service = System.Text.Json.JsonSerializer.Deserialize<ServiceDTO>(responseContent, option);
                        }
                        bookingRoomServices.Add(new BookingRoomServiceForm
                        {
                            ServiceId = serviceId,
                            PriceService = service.Price,
                            RoomId = RoomId
                        });
                    }
                    // Thêm mới
                    cart.Add(new CartItem()
                    {
                        Room = room,
                        RoomId = RoomId,
                        StartDate = StartTime,
                        EndDate = EndTime,
                        Note = note,
                        Price = room.Price,
                        TotalPrice = totalPrice,
                        BookingRoomServices = bookingRoomServices
                    });

                    SaveCartSession(cart);

                    return View(userInfo);
                }
            }
            return View(userInfo);
        }

        public async Task<IActionResult> ChekoutRoomDB([FromForm] OrderForm orderform, string payment)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            double totalPrice = 0;
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Lấy thời gian hiện tại theo múi giờ +7
            DateTime currentTimeInVietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            try
            {
                ViewBag.Title = "Thanh toán";
                if (orderform.Province == null ||
                    orderform.District == null || orderform.Commune == null)
                {
                    HttpResponseMessage responseUser = await _client.GetAsync(DefaultApiUrl + "UserInfo" + "/" + email);
                    if (responseUser.IsSuccessStatusCode)
                    {
                        string responseContent = await responseUser.Content.ReadAsStringAsync();

                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        AccountInfo Infos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                        orderform.Province = Infos.UserInfo.Province;
                        orderform.District = Infos.UserInfo.District;
                        orderform.Commune = Infos.UserInfo.Province;
                    }
                }

                // Lấy thông tin CartItems từ Session
                List<CartItem> cartItems = GetCartItems();

                // Tạo đối tượng OrderForm từ thông tin CartItems và orderform
                OrderForm order = new OrderForm
                {
                    OrderDate = currentTimeInVietnam,
                    OrderStatus = "Placed",
                    Province = orderform.Province,
                    District = orderform.District,
                    Commune = orderform.Commune,
                    Address = orderform.Address,
                    UserInfoId = orderform.UserInfoId,
                    FullName = orderform.FullName,
                    Phone = orderform.Phone,
                    TypePay = payment,
                    StatusPayment = false,
                    OrderProductDetails = new List<OrderProductDetailForm>(),
                    BookingServicesDetails = new List<BookingServicesDetailForm>(),
                    BookingRoomDetails = new List<BookingRoomDetailForm>(),
                    BookingRoomServices = new List<BookingRoomServiceForm>()
                };

                foreach (var cartItem in cartItems)
                {
                    if (cartItem.Room != null)
                    {
                        totalPrice = (double)cartItem.TotalPrice;
                        if (cartItem.BookingRoomServices != null && cartItem.BookingRoomServices.Any())
                        {
                            foreach (var roomService in cartItem.BookingRoomServices)
                            {
                                var bookingRoomService = new BookingRoomServiceForm
                                {
                                    RoomId = roomService.RoomId,
                                    ServiceId = roomService.ServiceId,
                                    PriceService = roomService.PriceService
                                };
                                order.BookingRoomServices.Add(bookingRoomService);
                            }
                        }

                        var bookingRoomDetail = new BookingRoomDetailForm
                        {
                            RoomId = cartItem.RoomId,
                            Price = cartItem.Price,
                            TotalPrice = cartItem.TotalPrice,
                            StartDate = cartItem.StartDate,
                            EndDate = cartItem.EndDate,
                        };
                        order.BookingRoomDetails.Add(bookingRoomDetail);
                    }
                }

                var jsonOrder = System.Text.Json.JsonSerializer.Serialize(order);

                var content = new StringContent(jsonOrder, Encoding.UTF8, "application/json");
                var responseOrder = await _client.PostAsync(DefaultApiUrl + "Order", content);

                if (responseOrder.IsSuccessStatusCode)
                {
                    if (payment == "vnpay")
                    {
                        int orderLatestID = 0;
                        HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Order/latest?email=" + email);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();

                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            };

                            OrderForm orderLatest = System.Text.Json.JsonSerializer.Deserialize<OrderForm>(responseContent, options);
                            orderLatestID = orderLatest.OrderId;
                        }

                        string vnp_Returnurl = _vnpConfiguration.ReturnUrl;  // Use the configured value
                        string vnp_Url = _vnpConfiguration.Url;  // Use the configured value
                        string vnp_TmnCode = _vnpConfiguration.TmnCode;  // Use the configured value
                        string vnp_HashSecret = _vnpConfiguration.HashSecret;  // Use the configured value

                        VnPayLibrary vnpay = new VnPayLibrary();

                        vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                        vnpay.AddRequestData("vnp_Command", "pay");
                        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                        vnpay.AddRequestData("vnp_Amount", (totalPrice * 100).ToString());

                        vnpay.AddRequestData("vnp_BankCode", "VNBANK");

                        vnpay.AddRequestData("vnp_CreateDate", currentTimeInVietnam.ToString("yyyyMMddHHmmss"));

                        vnpay.AddRequestData("vnp_CurrCode", "VND");
                        vnpay.AddRequestData("vnp_IpAddr", _utils.GetIpAddress());

                        vnpay.AddRequestData("vnp_Locale", "vn");

                        vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderLatestID);
                        vnpay.AddRequestData("vnp_OrderType", "other");

                        vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                        vnpay.AddRequestData("vnp_TxnRef", orderLatestID.ToString());

                        string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

                        return Redirect(paymentUrl);
                    }
                }
                else
                {
                    var errorContent = await responseOrder.Content.ReadAsStringAsync();
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorToast = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View();
        }

        [HttpPost]
        public IActionResult RemoveRoomCart([FromForm] int RoomId)
        {
            var cart = GetCartItems();

            if (RoomId > 0)
            {
                var serviceCartItem = cart.Find(s => s.Room != null && s.Room.RoomId == RoomId);

                if (serviceCartItem != null)
                {
                    cart.Remove(serviceCartItem);
                    SaveCartSession(cart);
                    ClearCart();
                }
            }

            return RedirectToAction("Index", "CheckoutRoom");
        }
    }
}
