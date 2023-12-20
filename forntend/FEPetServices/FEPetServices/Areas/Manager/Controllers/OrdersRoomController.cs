using FEPetServices.Form.OrdersForm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;


namespace FEPetServices.Areas.Manager.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "ManaOnly")]
    public class OrdersRoomController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public OrdersRoomController(IConfiguration configuration)
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
            ViewBag.Title = "Danh sách đơn phòng đã đặt";
            //https://localhost:7255/api/Order/getOrderRoom
            //HttpResponseMessage response = await _client.GetAsync("https://localhost:7255/api/" + "Order/getOrderRoom");
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Order/getOrderRoom");
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
        public async Task<IActionResult> OrderRoomDetail(int id)
        {
            ViewBag.Title = "Chi tiết phòng đã đặt";
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
        public async Task<IActionResult> OrderRoomDetail(int id, [FromForm] Status status)
        {
            status.newStatusProduct = "";
            status.newStatusService = "";

            //HttpResponseMessage response = await _client.PutAsJsonAsync("https://localhost:7255/api/" + "Order/changeStatus?Id=" + id, status);
            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrl + "Order/changeStatus?Id=" + id, status);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessToast"] = "Cập nhật thành công";
                return RedirectToAction("OrderRoomDetail", new { id = id });
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
    }
}
