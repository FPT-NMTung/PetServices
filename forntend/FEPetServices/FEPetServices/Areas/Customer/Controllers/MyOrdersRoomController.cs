using FEPetServices.Form.OrdersForm;
using FEPetServices.Models.ErrorResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

    
namespace FEPetServices.Areas.Customer.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "CusOnly")]
    public class MyOrdersRoomController : Controller
    {
        private readonly HttpClient _client = null;
        private readonly string DefaultApiUrl = "";
        private readonly string DefaultApiUrlOrders = "";
        private readonly IConfiguration configuration;

        public MyOrdersRoomController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrl = "https://localhost:7255/api/";
        }

        private async Task<IActionResult> GetOrdersRoom(string orderStatus, int page, int pageSize)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage responsecheck = await _client.GetAsync($"{DefaultApiUrl}Order/orderroomstatus/{orderStatus}?email={email}");
            if (responsecheck.StatusCode == HttpStatusCode.NotFound)
            {
                return View();
            }
            else
            {
                HttpResponseMessage response = await _client.GetAsync($"{DefaultApiUrl}Order/getRoomOrderUser/{email}?orderstatus={orderStatus}&page={page}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    if (!string.IsNullOrEmpty(responseContent) && responseContent.Contains("404 Not Found"))
                    {
                        return View("Error404");
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
                        return View("Error404");
                    }
                    else
                    {
                        return View();
                    }
                }
            }
        }

        [HttpGet]
        public Task<IActionResult> Index(string orderStatus, int page, int pageSize) => GetOrdersRoom(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> CheckIn(string orderStatus, int page, int pageSize) => GetOrdersRoom(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> CheckOut(string orderStatus, int page, int pageSize) => GetOrdersRoom(orderStatus, page, pageSize);

        [HttpGet] 
        public Task<IActionResult> Canceled(string orderStatus, int page, int pageSize) => GetOrdersRoom(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> Processing(string orderStatus, int page, int pageSize) => GetOrdersRoom(orderStatus, page, pageSize);

        [HttpGet]
        public async Task<IActionResult> OrderRoomDetail(int id)
        {
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
    }
}
