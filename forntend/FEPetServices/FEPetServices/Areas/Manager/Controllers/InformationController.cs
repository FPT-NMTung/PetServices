using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "ManaOnly")]
    public class InformationController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public InformationController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            //DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/UserInfo";
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Thông tin người dùng";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "UserInfo/" + email);

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
        public async Task<IActionResult> Index([FromForm] UserInfo userInfo, IFormFile image)
        {

            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            if (userInfo.Phone == null)
            {
                TempData["ErrorToast"] = "Số điện thoại không được để trống";
                return RedirectToAction("Index");
            }
            if (userInfo.Phone.Length != 10 && !userInfo.Phone.StartsWith("0"))
            {
                TempData["ErrorToast"] = "Số điện thoại phải bắt đầu bằng số 0 và có 10 chữ số";
                return RedirectToAction("Index");
            }

            if (userInfo.Address == null)
            {
                TempData["ErrorToast"] = "Địa chỉ cụ thể không được để trống";
                return RedirectToAction("Index");
            }
            if (userInfo.Address.Length <= 10)
            {
                TempData["ErrorToast"] = "Địa chỉ cụ thể phải lớn hơn 10 ký tự";
                return RedirectToAction("Index");
            }
            //dob check 
            if (userInfo.Dob.HasValue && userInfo.Dob.Value > DateTime.Now)
            {
                TempData["ErrorToast"] = "Ngày sinh không thể lớn hơn ngày hiện tại";
                return RedirectToAction("Index");
            }
            // Handle the uploaded image
            if (image != null)
            {
                string filename = GenerateRandomNumber(5) + image.FileName;
                filename = Path.GetFileName(filename);
                string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Profile/", filename);
                using (var stream = new FileStream(uploadfile, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                userInfo.ImageUser = "/img/Profile/" + filename;
            }
            else
            {
                HttpResponseMessage responseUser = await _client.GetAsync(DefaultApiUrl + "UserInfo/" + email);
                if (responseUser.IsSuccessStatusCode)
                {
                    string responseContent = await responseUser.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    AccountInfo managerInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                    userInfo.ImageUser = managerInfos.UserInfo.ImageUser;
                }
            }

            if (userInfo.Address == null || userInfo.FirstName == null || userInfo.LastName == null)
            {
                TempData["ErrorToast"] = "Vui lòng điền đầy đủ thông tin";
                return RedirectToAction("Index");
            }

            if (userInfo.Province == null || userInfo.District == null || userInfo.Commune == null)
            {
                HttpResponseMessage responseUser = await _client.GetAsync(DefaultApiUrl + "UserInfo/" + email);
                if (responseUser.IsSuccessStatusCode)
                {
                    string responseContent = await responseUser.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    AccountInfo managerInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                    userInfo.Province = managerInfos.UserInfo.Province;
                    userInfo.District = managerInfos.UserInfo.District;
                    userInfo.Commune = managerInfos.UserInfo.Province;
                }
            }

            // Update the user information, including the image URL
            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrl + "UserInfo/updateInfo?email=" + email, userInfo);
            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.SetString("UserImage", userInfo.ImageUser);
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
