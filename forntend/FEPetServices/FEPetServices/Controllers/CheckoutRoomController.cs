using FEPetServices.Areas.DTO;
using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using FEPetServices.Models.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Controllers
{
    [Authorize(Policy = "CusOnly")]
    public class CheckoutRoomController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public CheckoutRoomController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            /*DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");*/
            DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Product";
        }
        public class CartItem
        {
            // Room
            public int RoomId { get; set; }
            public int ServiceId { get; set; }
            public double? PriceService { get; set; }
            public virtual OrderForm Order { get; set; } = null!;
            public virtual RoomDTO Room { get; set; } = null!;
            public virtual ServiceDTO Service { get; set; } = null!;

            //
            public double? Price { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public const string CARTKEY = "cart";
        List<CartItem> GetCartItems()
        {

            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            }
            return new List<CartItem>();
        }
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }

        public async Task<IActionResult> Index(int RoomId, List<int> ServiceId, List<int> PriceService, int PartnerId, DateTime StartTime, DateTime EndTime)
        {
            ServiceDTO service = null;
            RoomDTO room = null;

            //Room
            //https://localhost:7255/api/Room/GetRoom/1
            HttpResponseMessage response = await client.GetAsync("https://localhost:7255/api/Room/GetRoom/" + RoomId);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var option = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                room = System.Text.Json.JsonSerializer.Deserialize<RoomDTO>(responseContent, option);
            }

            // Service 


            return View();
        }


    }
}
