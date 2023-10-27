using FEPetServices.Form;
using FEPetServices.Form.BookingForm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
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

                List<BookingForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<BookingForm>>(responseContent, options);
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

                BookingForm bookingForm = System.Text.Json.JsonSerializer.Deserialize<BookingForm>(responseContent, options);
                return View(bookingForm);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }


    }
}
