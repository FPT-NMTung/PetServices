using FEPetServices.Form.OrdersForm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "ManaOnly")]
    public class ShippedController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public ShippedController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrl = "https://localhost:7255/api/";
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Danh sách đơn hàng vận chuyển";
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Order/getOrder");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<OrderForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options)
                  .Where(order => order.OrderProductDetails.Count() > 0)
                  .ToList();    
                return View(orderLists);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderShippedDetail(int id)
        {
            ViewBag.Title = "Chi tiết đơn hàng vận chuyển";
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
        public async Task<IActionResult> OrderShippedDetail(int id, [FromForm] Status status)
        {
            if (status.newStatus == "Processing")
            {
                status.newStatusProduct = "Shipped";
                status.newStatusService = "";
            }

            else if(status.newStatus == "Delivered")
            {
                status.newStatus = "Processing";
                status.newStatusProduct = "Delivered";
                status.newStatusService = "";
            }

            //HttpResponseMessage response = await _client.PutAsJsonAsync("https://localhost:7255/api/" + "Order/changeStatus?Id=" + id, status);
            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrl + "Order/changeStatus?Id=" + id, status);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessToast"] = "Cập nhật thành công";
                return RedirectToAction("OrderShippedDetail", new { id = id });
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
    }
}
