using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
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

        public MenuCustomerController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "https://localhost:7255/api/UserInfo";
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
        public async Task<IActionResult> Information([FromForm] UserInfo userInfo)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            // Sử dụng HttpClient để gửi dữ liệu cập nhật lên API
            if (userInfo.Address == null || userInfo.FirstName == null ||
                userInfo.LastName == null)
            {
                TempData["ErrorToast"] = "Vui lòng điền đầy đủ thông tin";
                return RedirectToAction("Information");
            }

            if (userInfo.Province == null ||
                userInfo.District == null || userInfo.Commune == null)
            {
                TempData["ErrorToast"] = "Vui lòng cung cấp lại địa chỉ";
                return RedirectToAction("Information");
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

        [HttpPost]
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
                TempData["ErrorToast"] = "Mật khẩu mới và xác nhận lại mật khẩu không trùng khớp";
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
    }
}
