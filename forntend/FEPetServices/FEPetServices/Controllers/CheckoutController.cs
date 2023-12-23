using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using FEPetServices.Models.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using PetServices.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Controllers
{
    [Authorize(Policy = "CusOnly")]
    public class CheckoutController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlUserInfo = "";

        private readonly IConfiguration _configuration;
        private readonly VnpConfiguration _vnpConfiguration;

        private readonly Utils _utils;

        public CheckoutController(HttpClient client, IConfiguration configuration, Utils utils, VnpConfiguration vnpConfiguration)
        {
            _client = client;
            _configuration = configuration;
            _utils = utils;
            _vnpConfiguration = vnpConfiguration;

            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");

            //DefaultApiUrl = "https://localhost:7255/api/";
            //DefaultApiUrlUserInfo = "https://pet-service-api.azurewebsites.net/api/UserInfo";
        }

        public const string CARTKEY = "cart";
        public class CartItem
        {
            // Product
            public int quantityProduct { set; get; }
            public ProductDTO product { set; get; }

            // Service
            public int ServiceId { get; set; }
            public double? Price { get; set; }
            public double? Weight { get; set; }
            public double? PriceService { get; set; }
            public int? PartnerInfoId { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public PartnerInfo? PartnerInfo { get; set; }
            public ServiceDTO service { set; get; }
            // Room
        }
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Thanh toán";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "UserInfo/" + email);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                AccountInfo userInfo = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                return View(userInfo);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] OrderForm orderform, string payment)
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
                    HttpResponseMessage responseUser = await _client.GetAsync(DefaultApiUrl + "UserInfo/" + email);
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

                if(cartItems.Count() == 0)
                {
                    TempData["ErrorToast"] = "Giỏ hàng không tồn tại";
                    return RedirectToAction("Index", "Checkout");
                }

                foreach (var cartItem in cartItems)
                {
                    if (cartItem.product != null)
                    {
                        ProductDTO product = null;
                        HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Product/ProductID/" + cartItem.product.ProductId);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            var option = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            };
                            product = System.Text.Json.JsonSerializer.Deserialize<ProductDTO>(responseContent, option);
                        }

                        if (product.Quantity < cartItem.quantityProduct)
                        {
                            TempData["ErrorToast"] = "Sản phẩm vượt quá số lượng sản phẩm đã có trong kho, vui lòng kiểm tra lại trước khi đặt hàng";
                            return RedirectToAction("Index", "Checkout");
                        }
                    }
                }

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
                    BookingRoomDetails = new List<BookingRoomDetailForm>()
                };

                foreach (var cartItem in cartItems)
                {
                    if (cartItem.product != null)
                    {
                        var orderProductDetail = new OrderProductDetailForm
                        {
                            Quantity = cartItem.quantityProduct,
                            Price = cartItem.product.Price,
                            ProductId = cartItem.product.ProductId,
                            StatusOrderProduct = "Placed",
                        };
                        order.OrderProductDetails.Add(orderProductDetail);
                        totalPrice = totalPrice + (double)(cartItem.quantityProduct * cartItem.product.Price);
                    }

                    if (cartItem.service != null)
                    {
                        var bookingServicesDetail = new BookingServicesDetailForm
                        {
                            ServiceId = cartItem.service.ServiceId,
                            Price = cartItem.Price,
                            PriceService = cartItem.PriceService,
                            Weight = cartItem.Weight,
                            PartnerInfoId = cartItem.PartnerInfoId,
                            StartTime = cartItem.StartTime,
                            EndTime = cartItem.EndTime,
                            StatusOrderService = "Placed"
                        };
                        order.BookingServicesDetails.Add(bookingServicesDetail);
                        totalPrice = totalPrice + (double)cartItem.PriceService;
                    }
                }

                // Chuyển về chuỗi json
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
                    else
                    {
                        foreach (var cartItem in cartItems)
                        {
                            if (cartItem.product != null)
                            {
                                HttpResponseMessage response = await _client.PutAsync(DefaultApiUrl + "Product/ChangeProduct"
                                    + "?ProductId=" + cartItem.product.ProductId + "&Quantity=" + cartItem.quantityProduct, null);
                            }
                        }
                        ClearCart();
                        TempData["SuccessToast"] = "Đặt hàng thành công. Vui lòng kiểm tra lại giỏ hàng.";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    var errorContent = await responseOrder.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorToast = "Đã xảy ra lỗi: " + ex.Message;
            }

            return View();
        }

        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }
    }
}
