using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
   /* [Authorize(Policy = "ManaOnly")]*/
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
        public static string GenerateRandomNumber(int length)
        {
            Random random = new Random();
            const string chars = "0123456789"; // Chuỗi chứa các chữ số từ 0 đến 9
            char[] randomChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomChars);
        }
        [HttpPost]
        public async Task<IActionResult> Index([FromForm] UserInfo userInfo, List<IFormFile> image)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            
            foreach (var file in image)
            {
                string filename = GenerateRandomNumber(5) + file.FileName;
                filename = Path.GetFileName(filename);
                string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/", filename);
                var stream = new FileStream(uploadfile, FileMode.Create);
                file.CopyToAsync(stream);
                userInfo.ImageUser = "/img/" + filename;
            }

            // Sử dụng HttpClient để gửi dữ liệu cập nhật lên API
            if (userInfo.Address == null || userInfo.FirstName == null ||
                userInfo.LastName == null)
            {
                TempData["ErrorToast"] = "Vui lòng điền đầy đủ thông tin";
                return RedirectToAction("Index");
            }

            if (userInfo.Province == null ||
                userInfo.District == null || userInfo.Commune == null)
            {
                TempData["ErrorToast"] = "Vui lòng cung cấp lại địa chỉ";
                return RedirectToAction("Index");
            }

            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrl + "/updateInfo?email=" + email, userInfo);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessToast"] = "Cập nhật thông tin thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return RedirectToAction("Index");
            }
        }
    }
}
