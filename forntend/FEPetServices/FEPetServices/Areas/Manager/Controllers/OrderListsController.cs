using FEPetServices.Form;
using FEPetServices.Form.BookingForm;
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

        public OrderListsController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "https://localhost:7255/api/Order";

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<OrdersForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<OrdersForm>>(responseContent, options);
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
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                OrdersForm orderDetail = System.Text.Json.JsonSerializer.Deserialize<OrdersForm>(responseContent, options);
                double totalPrice = 0;
                foreach(var od in orderDetail.OrderProductDetails)
                {
                    totalPrice = (double)(totalPrice + od.Price * od.Quantity); 
                }
                ViewBag.TotalPrice = totalPrice;
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
        //https://localhost:7255/api/Order/changeStatus?Id=1
          https://localhost:7255/api/Order
            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrl + "/changeStatus?Id=" + id, status);
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
