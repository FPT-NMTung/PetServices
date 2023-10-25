using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
    [Authorize(Policy = "ManaOnly")]
    public class ListPartnerController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";

        public ListPartnerController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "https://localhost:7255/api/Partner";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<AccountInfo> listAccounts = System.Text.Json.JsonSerializer.Deserialize<List<AccountInfo>>(responseContent, options);
                return View(listAccounts);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartner(string email)
        {

            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "/" + email);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                AccountInfo accountInfo = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                return View(accountInfo);
            }
            else {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DetailPartner()
        {
            /* HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "/" + email);
             if (response.IsSuccessStatusCode)
             {
                 string responseContent = await response.Content.ReadAsStringAsync();

                 var options = new JsonSerializerOptions
                 {
                     PropertyNameCaseInsensitive = true
                 };

                 AccountInfo accountInfo = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                 return View(accountInfo);
             }
             else
             {
                 TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                 return View();
             }*/
            return View();
        }
    }
}
