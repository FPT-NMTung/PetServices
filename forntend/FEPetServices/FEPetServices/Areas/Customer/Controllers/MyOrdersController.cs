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
        public async Task<IActionResult> Index(string orderStatus)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            //https://localhost:7255/api/Order/email/customer%40gmail.com?orderstatus=All
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrlOrders + "/email/" + email + "?orderstatus=" + orderStatus);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<OrderForm> orders = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);
                return View(orders);
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeliveryOrder(string orderStatus)
        {
            return View();
        }
    }
}
