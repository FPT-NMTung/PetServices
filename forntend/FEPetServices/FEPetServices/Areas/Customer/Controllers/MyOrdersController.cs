using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Areas.Customer.Controllers
{
    public class MyOrdersController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlOrders = "";

        public MyOrdersController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrlOrders = "https://localhost:7255/api/Order";
        }

        [HttpGet]
        public async Task<IActionResult> Index(string orderStatus, int page , int pageSize )
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage response = await _client.GetAsync($"{DefaultApiUrlOrders}/email/{email}?orderstatus={orderStatus}&page={page}&pageSize={pageSize}");

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

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
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeliveryOrders(string orderStatus)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> WaitingOrders(string orderStatus)
        {
            return View();
        }


    }
}
