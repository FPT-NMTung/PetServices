using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{

    public class InformationController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";

        public InformationController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "https://localhost:7255/api/UserInfo";
            //https://localhost:7255/api/UserInfo/manager%40gmail.com
            //https://localhost:7255/api/UserInfo/updateInfo?email=manager%40gmail.com
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "/" + email);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                AccountInfo managerInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);

                return View(managerInfos);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] UserInfo userInfo)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            // Sử dụng HttpClient để gửi dữ liệu cập nhật lên API
            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrl + "/updateInfo?email=" + email, userInfo);

            if (response.IsSuccessStatusCode)
            {
                // Xử lý thành công, ví dụ: chuyển hướng hoặc hiển thị thông báo
                return RedirectToAction("Index");
            }
            else
            {
                // Xử lý thất bại, ví dụ: hiển thị thông báo lỗi
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật thông tin.");
                return View(userInfo);
            }
        }
    }
}
