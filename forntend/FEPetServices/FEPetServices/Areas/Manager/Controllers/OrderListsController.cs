using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "ManaOnly")]
    public class OrderListsController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public OrderListsController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrl = "https://localhost:7255/api/";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Danh sách đơn hàng";
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Order/getOrder");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<OrderForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);
                return View(orderLists);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int id)
        {
            ViewBag.Title = "Chi tiết đơn hàng";
            //HttpResponseMessage reasonResponse = await _client.GetAsync(DefaultApiUrl + "Reason/GetAll");
            //if (reasonResponse.IsSuccessStatusCode)
            //{
            //    var reaCategories = await reasonResponse.Content.ReadFromJsonAsync<List<ReasonDTO>>();
            //    ViewBag.Reasons = new SelectList(reaCategories, "ReasonId", "ReasonTitle");
            //}
            //HttpResponseMessage response = await _client.GetAsync("https://localhost:7255/api/" + "Order/" + id);
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
            if (status.newStatus == "Confirmed")
            {
                status.newStatusProduct = "Packaging";
                status.newStatusService = "Waiting";
            }
            if (status.newStatus == "Cancelled")
            {
                if (string.IsNullOrWhiteSpace(reasonOrders.ReasonOrderTitle) || string.IsNullOrWhiteSpace(reasonOrders.ReasonOrderDescription))
                {
                    TempData["ErrorToast"] = "Vui lòng nhập tiêu đề và mô tả trước khi cập nhật.";
                    return RedirectToAction("OrderDetail", new { id = id });
                }
                status.newStatusProduct = "Cancelled";
                status.newStatusService = "Cancelled";
                HttpResponseMessage responseOrder = await _client.GetAsync(DefaultApiUrl + "Order/" + id);
                if (responseOrder.IsSuccessStatusCode)
                {
                    string responseContent = await responseOrder.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    OrderForm orderDetail = System.Text.Json.JsonSerializer.Deserialize<OrderForm>(responseContent, options);

                    if (orderDetail.OrderProductDetails.Count() > 0)
                    {
                        foreach (var orderProductDetail in orderDetail.OrderProductDetails)
                        {
                            HttpResponseMessage responseProduct = await _client.PutAsync(DefaultApiUrl + "Product/InChangeProduct"
                              + "?ProductId=" + orderProductDetail.ProductId +
                              "&Quantity=" + orderProductDetail.Quantity, null);
                        }
                    }
                }
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

    }
}
