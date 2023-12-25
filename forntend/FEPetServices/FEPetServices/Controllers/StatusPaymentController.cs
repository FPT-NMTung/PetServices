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
    public class StatusPaymentController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        //private string DefaultApiUrlUserInfo = "";

        private readonly IConfiguration _configuration;
        private readonly VnpConfiguration _vnpConfiguration;

        private readonly Utils _utils;

        public StatusPaymentController(HttpClient client, IConfiguration configuration, Utils utils, VnpConfiguration vnpConfiguration)
        {
            _client = client;
            _configuration = configuration;
            _utils = utils;
            _vnpConfiguration = vnpConfiguration;

            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);

            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");

            //DefaultApiUrl = "https://localhost:7255/api/";
        }

        public const string CARTKEY = "cart";
        public const string CARTKEYROOM = "cartRoom";
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
        private List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsonCart = session.GetString(CARTKEY);
            return jsonCart != null ? JsonConvert.DeserializeObject<List<CartItem>>(jsonCart) : new List<CartItem>();
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Thanh toán";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            string vnp_HashSecret = _vnpConfiguration.HashSecret;
            var vnpayData = Request.Query;
            VnPayLibrary vnpay = new VnPayLibrary();

            foreach (var queryParameter in vnpayData)
            {
                if (!string.IsNullOrEmpty(queryParameter.Key) && queryParameter.Key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(queryParameter.Key, queryParameter.Value);
                }
            }

            long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            String vnp_SecureHash = Request.Query["vnp_SecureHash"];
            String TerminalID = Request.Query["vnp_TmnCode"];
            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            String bankCode = Request.Query["vnp_BankCode"];
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

            if (checkSignature)
            {
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    List<CartItem> cartItems = GetCartItems();
                    await UpdateProductQuantitiesAsync(cartItems);
                    HttpResponseMessage responseStatusPayment = await _client.PutAsync(DefaultApiUrl + "Order/changeStatusPayment"
                               + "?Id=" + orderId, null);
                    ClearCart();
                    ClearCartRoom();
                    TempData["SuccessToast"] = "Thanh toán thành công.";
                    ViewBag.SuccessOrderID = orderId;
                    ViewBag.VNPAY = vnpayTranId;    
                    ViewBag.CheckSucess = "Success";
                    return View();
                }
                else
                {
                    List<CartItem> cartItems = GetCartItems();
                    await UpdateProductQuantitiesAsync(cartItems);
                    int orderLatestID = 0;
                    bool checkRoom = false;
                    HttpResponseMessage responseLastOrder = await _client.GetAsync(DefaultApiUrl + "Order/latest?email=" + email);
                    if (responseLastOrder.IsSuccessStatusCode)
                    {
                        string responseContent = await responseLastOrder.Content.ReadAsStringAsync();

                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        OrderForm orderLatest = System.Text.Json.JsonSerializer.Deserialize<OrderForm>(responseContent, options);
                        orderLatestID = orderLatest.OrderId;

                        if(orderLatest.BookingRoomDetails.Count() > 0 && orderLatest.BookingServicesDetails.Count() == 0 
                            && orderLatest.OrderProductDetails.Count() == 0)
                        {
                            HttpResponseMessage responseDeleteOrder = await _client.DeleteAsync(DefaultApiUrl + "Order/delete/" + orderLatestID);
                            if (responseDeleteOrder.IsSuccessStatusCode)
                            {
                                checkRoom = true;
                            }
                        }
                    }

                    ClearCart();
                    ClearCartRoom();
                    if (!checkRoom)
                    {
                        TempData["ErrorToast"] = "Thanh toán thất bại. Vui lòng kiểm tra lại đơn hàng trong giỏ hàng.";
                    }
                    else
                    {
                        TempData["ErrorToast"] = "Thanh toán thất bại. Vui lòng kiểm tra thanh toán.";
                    }
                    ViewBag.ErrorOrderID = orderId;
                    ViewBag.VNPAY = vnpayTranId;
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        private void ClearCart()
        {
            HttpContext.Session.Remove(CARTKEY);
        }

        private void ClearCartRoom()
        {
            HttpContext.Session.Remove(CARTKEYROOM);
        }

        private async Task UpdateProductQuantitiesAsync(List<CartItem> cartItems)
        {
            foreach (var cartItem in cartItems)
            {
                if (cartItem.product != null)
                {
                    HttpResponseMessage response = await _client.PutAsync(
                        $"{DefaultApiUrl}Product/ChangeProduct" +
                        $"?ProductId={cartItem.product.ProductId}&Quantity={cartItem.quantityProduct}", null);
                }
            }
        }

    }
}
