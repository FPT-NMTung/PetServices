using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Controllers
{
    [Authorize(Policy = "CusOnly")]
    public class CheckoutController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlUserInfo = "";

        public CheckoutController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "https://localhost:7255/api/UserInfo";
            DefaultApiUrlUserInfo = "https://localhost:7255/api/UserInfo";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrlUserInfo + "/" + email);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                AccountInfo userInfo = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);

                return View(userInfo);
            }

            return View();
        }


    }
}
