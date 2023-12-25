using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using FEPetServices.Models.ErrorResult;
using FEPetServices.Models.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Areas.Customer.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "CusOnly")]
    public class MyOrdersController : Controller
    {
        private readonly HttpClient _client = null;
        private readonly string DefaultApiUrl = "";
        private readonly string DefaultApiUrlOrders = "";
        private readonly IConfiguration configuration;
        private readonly VnpConfiguration _vnpConfiguration;

        private readonly Utils _utils;

        public MyOrdersController(IConfiguration configuration, Utils utils, VnpConfiguration vnpConfiguration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            _utils = utils;
            _vnpConfiguration = vnpConfiguration;

            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrl = "https://localhost:7255/api/";
        }

        private async Task<IActionResult> GetOrders(string orderStatus, int page, int pageSize)
        {
            ViewBag.Title = "Danh sách đơn hàng";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage responsecheck = await _client.GetAsync($"{DefaultApiUrl}Order/orderstatus/{orderStatus}?email={email}");
            if (responsecheck.StatusCode == HttpStatusCode.NotFound)
            {
                return View();
            }   
            else
            {
                HttpResponseMessage response = await _client.GetAsync($"{DefaultApiUrl}Order/getOrderUser/{email}?orderstatus={orderStatus}&page={page}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    if (!string.IsNullOrEmpty(responseContent) && responseContent.Contains("404 Not Found"))
                    {
                        return new ErrorResult("");
                    }

                    List<OrderForm> orders = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return PartialView("_OrderPartialView", orders);
                    }
                    else
                    {
                        return View(orders);
                    }
                }   
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return new ErrorResult("");
                    }
                    else
                    {
                        return View();
                    }
                }
            }
        }

        private async Task<IActionResult> GetOrdersNoneStatus(string orderStatus, int page, int pageSize)
        {
            
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage responsecheck = await _client.GetAsync($"{DefaultApiUrl}Order/GetOrderUserNoneFeedback/{email}?orderstatus={orderStatus}&page={page}&pageSize={pageSize}");
            if (!responsecheck.IsSuccessStatusCode)
            {
                return View(); 
            }
            else
            {
                //HttpResponseMessage response = await _client.GetAsync($"{DefaultApiUrl}Order/GetOrderUserNoneFeedback/{email}?orderstatus={orderStatus}&page={page}&pageSize={pageSize}");
                HttpResponseMessage response = await _client.GetAsync($"{DefaultApiUrl}Order/GetOrderUserNoneFeedback/{email}?orderstatus={orderStatus}&page={page}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    if (!string.IsNullOrEmpty(responseContent) && responseContent.Contains("404 Not Found"))
                    {
                        return new ErrorResult("");
                    }

                    List<OrderForm> orders = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return PartialView("_OrderPartialView", orders);
                    }
                    else
                    {
                        return View(orders);
                    }
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return new ErrorResult("");
                        //return View();

                    }
                    else
                    {
                        //return new ErrorResult("");
                        return View();
                    }
                }
            }
        }

        [HttpGet]
        public Task<IActionResult> AllOrders(string orderStatus, int page, int pageSize) => GetOrders(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> PlacedOrders(string orderStatus, int page, int pageSize) => GetOrders(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> ConfirmedOrders(string orderStatus, int page, int pageSize) => GetOrders(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> ProcessingOrders(string orderStatus, int page, int pageSize) => GetOrders(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> CompletedOrders(string orderStatus, int page, int pageSize) => GetOrders(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> CancelledOrders(string orderStatus, int page, int pageSize) => GetOrders(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> NoneFeedback(string orderStatus, int page, int pageSize) => GetOrdersNoneStatus(orderStatus, page, pageSize);

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int id)
        {
            ViewBag.Title = "Chi tiết đơn hàng";
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Order/" + id);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                OrderForm orderDetail = System.Text.Json.JsonSerializer.Deserialize<OrderForm>(responseContent, options);
                return View(orderDetail);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> OrderDetail(int id, [FromForm] Status status, [FromForm] ReasonOrdersForm reasonOrders)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);


            if (status.newStatus == "Cancelled")
            {
                if (string.IsNullOrWhiteSpace(reasonOrders.ReasonOrderTitle) || string.IsNullOrWhiteSpace(reasonOrders.ReasonOrderDescription))
                {
                    TempData["ErrorToast"] = "Vui lòng nhập tiêu đề và mô tả trước khi cập nhật.";
                    return RedirectToAction("OrderDetail", new { id = id });
                }
                status.newStatusProduct = "Cancelled";
                status.newStatusService = "Cancelled";
            }
            //HttpResponseMessage response = await _client.PutAsJsonAsync("https://localhost:7255/api/" + "Order/changeStatus?Id=" + id, status);
            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrl + "Order/changeStatus?Id=" + id, status);
            reasonOrders.OrderId = id;

            reasonOrders.EmailReject = email;

            HttpResponseMessage responseReject = await _client.PostAsJsonAsync(DefaultApiUrl + "ReasonOrder", reasonOrders);
            if (response.IsSuccessStatusCode || responseReject.IsSuccessStatusCode)
            {
                TempData["SuccessToast"] = "Cập nhật thành công";
                return RedirectToAction("OrderDetail", new { id = id });
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }

        [HttpPost]
        public IActionResult CheckoutAgain(int orderID, DateTime orderDate, float totalPrice)
        {
            string vnp_Returnurl = _vnpConfiguration.ReturnUrl;  
            string vnp_Url = _vnpConfiguration.Url;  
            string vnp_TmnCode = _vnpConfiguration.TmnCode;  
            string vnp_HashSecret = _vnpConfiguration.HashSecret; 

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (totalPrice * 100).ToString());

            vnpay.AddRequestData("vnp_BankCode", "VNBANK");

            vnpay.AddRequestData("vnp_CreateDate", orderDate.ToString("yyyyMMddHHmmss"));

            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", _utils.GetIpAddress());

            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderID);
            vnpay.AddRequestData("vnp_OrderType", "other");

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", orderID.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return Redirect(paymentUrl);
        }

    }
}
