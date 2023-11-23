using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using FEPetServices.Models.Payments;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;
using System.Net.Http.Headers;
using System.Text;

namespace FEPetServices.Controllers
{
    public class StatusPaymentController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlUserInfo = "";

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

            DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/UserInfo";
            DefaultApiUrlUserInfo = "https://pet-service-api.azurewebsites.net/api/UserInfo";
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

        public async Task<IActionResult> Index()
        {
            string vnp_HashSecret = _vnpConfiguration.HashSecret;
            var vnpayData = Request.Query;
            VnPayLibrary vnpay = new VnPayLibrary();

            foreach (var queryParameter in vnpayData)
            {
                //get all query string data
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
                    foreach (var cartItem in cartItems)
                    {
                        if (cartItem.product != null)
                        {
                            HttpResponseMessage response = await _client.PutAsync("https://pet-service-api.azurewebsites.net/api/Product/ChangeProduct"
                                + "?ProductId=" + cartItem.product.ProductId + "&Quantity=" + cartItem.quantityProduct, null);
                        }
                    }
                    //https://localhost:7255/api/Order/changeStatusPayment?Id=45
                    HttpResponseMessage responseStatusPayment = await _client.PutAsync("https://localhost:7255/api/Order/changeStatusPayment"
                               + "?Id=" + orderId, null);

                    ClearCart();

                    TempData["SuccessToast"] = "Thanh toán thành công.";
                    ViewBag.SuccessOrderID = orderId;
                    ViewBag.VNPAY = vnpayTranId;
                    ViewBag.CheckSucess = "Sucess";
                    return View();
                }
                else
                {
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
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }
    }
}
