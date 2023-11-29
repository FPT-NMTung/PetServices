using FEPetServices.Form.OrdersForm;
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
    public class MyOrdersController : Controller
    {
        private readonly HttpClient _client = null;
        private readonly string DefaultApiUrl = "";
        private readonly string DefaultApiUrlOrders = "";
        private readonly IConfiguration configuration;

        public MyOrdersController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            //DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            DefaultApiUrl = "https://localhost:7255/api/";
            DefaultApiUrlOrders = "https://localhost:7255/api/";
        }

        private async Task<IActionResult> GetOrders(string orderStatus, int page, int pageSize)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            //https://localhost:7255/api/Order/orderstatus/Waiting?email=customer%40gmail.com

            //HttpResponseMessage responsecheck = await _client.GetAsync($"{DefaultApiUrlOrders}Order/orderstatus/{orderStatus}?email={email}");
            HttpResponseMessage responsecheck = await _client.GetAsync($"{DefaultApiUrl}Order/orderstatus/{orderStatus}?email={email}");
            if (responsecheck.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.NotFound = "Error404";
                return View();
            }   
            else
            {
                //HttpResponseMessage response = await _client.GetAsync($"{DefaultApiUrlOrders}Order/email/{email}?orderstatus={orderStatus}&page={page}&pageSize={pageSize}");
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
        public Task<IActionResult> AllOrders(string orderStatus, int page, int pageSize) => GetOrders(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> CompletedOrders(string orderStatus, int page, int pageSize) => GetOrders(orderStatus, page, pageSize);
    }
}
