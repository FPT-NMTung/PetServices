using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FEPetServices.Areas.Customer.Controllers
{
    public class MyOrdersController : Controller
    {
        private readonly HttpClient _client = null;
        private readonly string DefaultApiUrlOrders = "";

        public MyOrdersController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrlOrders = "https://localhost:7255/api/Order";
        }

        private async Task<IActionResult> GetOrders(string orderStatus, int page, int pageSize)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            //https://localhost:7255/api/Order/orderstatus/Waiting?email=customer%40gmail.com

            HttpResponseMessage responsecheck = await _client.GetAsync($"{DefaultApiUrlOrders}/orderstatus/{orderStatus}?email={email}");
            if (responsecheck.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.NotFound = "Error404";
                return View();
            }
            else
            {
                HttpResponseMessage response = await _client.GetAsync($"{DefaultApiUrlOrders}/email/{email}?orderstatus={orderStatus}&page={page}&pageSize={pageSize}");

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
        public Task<IActionResult> DeliveryOrders(string orderStatus, int page, int pageSize) => GetOrders(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> WaitingOrders(string orderStatus, int page, int pageSize) => GetOrders(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> ReceivedOrders(string orderStatus, int page, int pageSize) => GetOrders(orderStatus, page, pageSize);

        [HttpGet]
        public Task<IActionResult> RejectOrders(string orderStatus, int page, int pageSize) => GetOrders(orderStatus, page, pageSize);
    }
}
