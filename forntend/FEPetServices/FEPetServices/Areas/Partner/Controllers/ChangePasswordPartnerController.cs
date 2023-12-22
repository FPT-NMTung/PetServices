using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FEPetServices.Areas.Partner.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "PartnerOnly")]
    public class ChangePasswordPartnerController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public ChangePasswordPartnerController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Account";
        }
        public async Task<IActionResult> Index([FromForm] ChangePassword changePassword)
        {
            ViewBag.Title = "Đổi mật khẩu";
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

            string apiUrl = $"{DefaultApiUrl}Account/newpassword?email={email}&oldpassword={changePassword.OldPassword}&newpassword={changePassword.NewPassword}&confirmnewpassword={changePassword.ConfirmNewPassword}";

            HttpResponseMessage response = await _client.PutAsync(apiUrl, null);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessToast = "Đổi mật khẩu thành công";
                return View();
            }
            else
            {
                ViewBag.ErrorToast = "Mật khẩu cũ không chính xác";
                return View();
            }
        }

    }
}