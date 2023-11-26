using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
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
            //DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Order";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Order");
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
            HttpResponseMessage response = await _client.GetAsync("https://localhost:7255/api/" + "Order/" + id);
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
        public async Task<IActionResult> OrderDetail(int id, [FromForm] Status status)
        {
            //https://pet-service-api.azurewebsites.net/api/Order/changeStatus?Id=1
            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrl + "Order/changeStatus?Id=" + id, status);
            if (response.IsSuccessStatusCode)
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
