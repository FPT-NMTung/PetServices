using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FEPetServices.Controllers
{
    public class VerifyEmailController : Controller
    {
        private readonly HttpClient _client;
        private readonly string _defaultApiUrl;

        public VerifyEmailController()
        {
            _client = new HttpClient();
            _defaultApiUrl = "https://localhost:7255/api/Account/VerifyOTPAndActivateAccount";
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Verify(VerifyOTPModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.OTP))
                {
                    ViewBag.ErrorMessage = "Email và mã OTP cần phải được nhập đúng.";
                    return View("Index", model);
                }

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_defaultApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.SuccessMessage = "Tài khoản đã được kích hoạt thành công.";
                }
                else
                {
                    ViewBag.ErrorMessage = "Xác minh không thành công. Vui lòng kiểm tra lại email và mã OTP.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }

            return View("Index", model);
        }
    }
}
