using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Areas.Customer.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrlOrders = "";

        public OrderController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrlOrders = "https://localhost:7255/api/Order";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<OrderForm> orders = TempData["OrdersData"] as List<OrderForm>;
            if (orders != null)
            {
                return View(orders);
            }
            else
            {
                return View();
            }

        }
    }
}
