using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetServices.Form;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Areas.Partner.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "PartnerOnly")]
    public class DashboardPartnerController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;
        public DashboardPartnerController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            //DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            DefaultApiUrl = "https://localhost:7255/api/";
        }
        public class DashBoardPartner
        {
            public List<FeedbackForm>? FeedbackCustomer { get; set; }
        }
        public async Task<IActionResult> Index()
        {
            DashBoardPartner dashBoardPartner = new DashBoardPartner()
            {
                FeedbackCustomer = new List<FeedbackForm>()
            };
            try
            {
                //lay thong tin cua partner
                ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
                string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
                HttpResponseMessage repId = await client.GetAsync(DefaultApiUrl + "Partner/" + email);
                AccountInfo account = null; // Initialize with null or a default value

                if (repId.IsSuccessStatusCode)
                {
                    string responseAccContent = await repId.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    account = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseAccContent, options);
                }
                int partnerInfoId = account?.PartnerInfoId ?? 0; // Use the null-conditional operator to provide a default value

                //so don hang trong thang
                HttpResponseMessage NumberOrderInMonthResponse = await client.GetAsync(DefaultApiUrl + "OrderInMonth/" + partnerInfoId);
                if (NumberOrderInMonthResponse.IsSuccessStatusCode)
                {
                    var OrderInMonth = await NumberOrderInMonthResponse.Content.ReadFromJsonAsync<int>();
                    ViewBag.OrderInMonth = OrderInMonth;
                }

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View(dashBoardPartner);
        }
    }
}
