using FEPetServices.Form;
using FEPetServices.Form.BookingForm;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Areas.Customer.Controllers
{
    public class MenuCustomerController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlPet = "";

        public MenuCustomerController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "https://localhost:7255/api/UserInfo";
            DefaultApiUrlPet = "https://localhost:7255/api/PetInfo";
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Information()
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

                AccountInfo userInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);

                return View(userInfos);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Information([FromForm] UserInfo userInfo, IFormFile image)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            if (userInfo.Phone == null)
            {
                TempData["ErrorToast"] = "Số điện thoại không được để trống";
                return RedirectToAction("Information");
            }
            if (userInfo.Phone.Length == 10 && userInfo.Phone.StartsWith("0"))
            {

            }
            else
            {
                TempData["ErrorToast"] = "Số điện thoại phải bắt đầu bằng số 0 và có 10 chữ số";
                return RedirectToAction("Information");
            }

            if (userInfo.Address == null)
            {
                TempData["ErrorToast"] = "Địa chỉ cụ thể không được để trống";
                return RedirectToAction("Information");
            }
            if (userInfo.Address.Length <= 10)
            {
                TempData["ErrorToast"] = "Địa chỉ cụ thể phải lớn hơn 10 ký tự";
                return RedirectToAction("Information");
            }

            if (image != null)
            {
                string filename = GenerateRandomNumber(5) + image.FileName;
                filename = Path.GetFileName(filename);
                string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/", filename);
                using (var stream = new FileStream(uploadfile, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                userInfo.ImageUser = "/img/" + filename;
            }
            else
            {
                HttpResponseMessage responseUser = await _client.GetAsync(DefaultApiUrl + "/" + email);
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

            if (userInfo.Province == null ||
                userInfo.District == null || userInfo.Commune == null)
            {
                HttpResponseMessage responseUser = await _client.GetAsync(DefaultApiUrl + "/" + email);
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

            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrl + "/updateInfo?email=" + email, userInfo);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessToast"] = "Cập nhật thông tin thành công";
                return RedirectToAction("Information");
            }
            else
            {
                TempData["ErrorToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return RedirectToAction("Information");
            }
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

        public async Task<IActionResult> ChangePassword([FromForm] ChangePassword changePassword)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            if (changePassword.OldPassword == null || changePassword.NewPassword == null)
            {
                return View();
            }

            if (changePassword.NewPassword != changePassword.ConfirmNewPassword)
            {
                ViewBag.ErrorToast = "Mật khẩu mới và xác nhận lại mật khẩu không trùng khớp";
                return View();
            }

            else if (changePassword.NewPassword.Length < 8)
            {
                ViewBag.ErrorToast = "Mật khẩu mới phải có ít nhất 8 ký tự";
                return View();
            }

            string apiUrl = $"https://localhost:7255/api/Account/newpassword?email={email}&oldpassword={changePassword.OldPassword}&newpassword={changePassword.NewPassword}&confirmnewpassword={changePassword.ConfirmNewPassword}";

            HttpResponseMessage response = await _client.PutAsync(apiUrl, null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessToast"] = "Đổi mật khẩu thành công";
                return View();
            }
            else
            {
                TempData["ErrorToast"] = "Mật khẩu cũ không chính xác";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> PetInfo()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrlPet + "/" + email);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                AccountInfo petInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);

                return View(petInfos);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
    }
}
