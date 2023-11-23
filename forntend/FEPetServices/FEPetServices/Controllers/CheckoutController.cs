﻿using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using FEPetServices.Models.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;
using System.Globalization;
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

            DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/UserInfo";
            DefaultApiUrlUserInfo = "https://pet-service-api.azurewebsites.net/api/UserInfo";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "/" + email);

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

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] OrderForm orderform, string payment)
        {

            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            double totalPrice = 0;
            DateTime dateOrder = DateTime.Now;
            try
            {
                if (orderform.Province == null ||
                    orderform.District == null || orderform.Commune == null)
                {
                    HttpResponseMessage responseUser = await _client.GetAsync(DefaultApiUrl + "/" + email);
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
                    OrderDate = dateOrder,
                    OrderStatus = "Waiting",
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
                            ProductId = cartItem.product.ProductId
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
                        };
                        order.BookingServicesDetails.Add(bookingServicesDetail);
                        totalPrice = totalPrice + (double)cartItem.PriceService;
                    }
                }

                // Chuyển về chuỗi json
                var jsonOrder = System.Text.Json.JsonSerializer.Serialize(order);

                var content = new StringContent(jsonOrder, Encoding.UTF8, "application/json");
                var responseOrder = await _client.PostAsync("https://pet-service-api.azurewebsites.net/api/Order", content);

                if (responseOrder.IsSuccessStatusCode)
                {
                    if (payment == "vnpay")
                    {
                        int orderLatestID = 0;
                        HttpResponseMessage response = await _client.GetAsync("https://localhost:7255/api/Order/latest?email="+ email);
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

                        vnpay.AddRequestData("vnp_CreateDate", dateOrder.ToString("yyyyMMddHHmmss"));

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
                                HttpResponseMessage response = await _client.PutAsync("https://pet-service-api.azurewebsites.net/api/Product/ChangeProduct"
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

      /*  public void Payment(double price, DateTime orderDate, int orderId)
        {
            string vnp_Returnurl = _vnpConfiguration.ReturnUrl;  // Use the configured value
            string vnp_Url = _vnpConfiguration.Url;  // Use the configured value
            string vnp_TmnCode = _vnpConfiguration.TmnCode;  // Use the configured value
            string vnp_HashSecret = _vnpConfiguration.HashSecret;  // Use the configured value

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (price * 100).ToString());

            vnpay.AddRequestData("vnp_BankCode", "VNBANK");

            vnpay.AddRequestData("vnp_CreateDate", orderDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", _utils.GetIpAddress());

            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderId);
            vnpay.AddRequestData("vnp_OrderType", "other");

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", orderId.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return(paymentUrl);
        }*/
    }
}
