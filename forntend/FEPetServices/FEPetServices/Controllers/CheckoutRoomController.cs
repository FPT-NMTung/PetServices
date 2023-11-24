using FEPetServices.Areas.DTO;
using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;
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
            public List<BookingRoomServiceForm> BookingRoomServices { get; set; }
            //
            public int RoomId { get; set; }
            public double? Price { get; set; }
            public string? Note { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public double? TotalPrice { get; set; }
            public virtual RoomDTO? Room { get; set; } = null!;
        }

        public const string CARTKEY = "cartRoom";
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

        public async Task<IActionResult> Index(int RoomId, List<int> ServiceId, DateTime StartTime, DateTime EndTime, double totalPrice, string note)
        {
            // Thônng tin người đặt hàng
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            AccountInfo userInfo = null;
            HttpResponseMessage responseInfo = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/UserInfo" + "/" + email);
            if (responseInfo.IsSuccessStatusCode)
            {
                string responseContent = await responseInfo.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                 userInfo = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
            }

            // Cart
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
            if (RoomId != null)
            {
                var cart = GetCartItems();
                var cartitem = cart.Find(s => s.Room != null && s.Room.RoomId == RoomId);

                if (cartitem != null)
                {

                }
                else
                {
                    var bookingRoomServices = new List<BookingRoomServiceForm>();
                    foreach (var serviceId in ServiceId)
                    {
                        int a = serviceId;
                        //https://localhost:7255/api/Service/ServiceID/5
                        HttpResponseMessage responseService = await client.GetAsync("https://localhost:7255/api/Service/ServiceID/" + serviceId);
                        if (responseService.IsSuccessStatusCode)
                        {
                            string responseContent = await responseService.Content.ReadAsStringAsync();
                            var option = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            };
                            service = System.Text.Json.JsonSerializer.Deserialize<ServiceDTO>(responseContent, option);
                        }
                        bookingRoomServices.Add(new BookingRoomServiceForm
                        {
                            ServiceId = serviceId,
                            PriceService = service.Price,
                            RoomId = RoomId
                        });
                    }
                    // Thêm mới
                    cart.Add(new CartItem()
                    {
                        Room = room,
                        RoomId = RoomId,
                        StartDate = StartTime,
                        EndDate = EndTime,
                        Note = note,
                        TotalPrice = totalPrice,
                        BookingRoomServices = bookingRoomServices
                    });

                    SaveCartSession(cart);

                    return View(userInfo);
                }
            }
            return View();
        }


    }
}
