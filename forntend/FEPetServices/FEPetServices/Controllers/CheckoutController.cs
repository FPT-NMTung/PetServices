using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
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

        public CheckoutController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "https://localhost:7255/api/UserInfo";
            DefaultApiUrlUserInfo = "https://localhost:7255/api/UserInfo";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrlUserInfo + "/" + email);

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
        public async Task<IActionResult> Index([FromForm] OrderForm orderform)
        {
            try
            {
                // Lấy thông tin CartItems từ Session
                List<CartItem> cartItems = GetCartItems();

                // Tạo đối tượng OrderForm từ thông tin CartItems và orderform
                OrderForm order = new OrderForm
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = orderform.Province,
                    District = orderform.District,
                    Commune = orderform.Commune,
                    Address = orderform.Address,
                    UserInfoId = orderform.UserInfoId,
                    FullName = orderform.FullName,
                    Phone = orderform.Phone,
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
                    }
                }

                // Chuyển về chuỗi json
                var jsonOrder = System.Text.Json.JsonSerializer.Serialize(order);

                var content = new StringContent(jsonOrder, Encoding.UTF8, "application/json");
                var responseOrder = await _client.PostAsync("https://localhost:7255/api/Order", content);

                if (responseOrder.IsSuccessStatusCode)
                {
                    // Xoá giỏ hàng
                    ClearCart();
                    return RedirectToAction("Index", "Home");
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
