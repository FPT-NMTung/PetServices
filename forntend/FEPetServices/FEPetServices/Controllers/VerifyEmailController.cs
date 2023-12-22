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
        /*private readonly string _defaultApiUrl;*/
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public VerifyEmailController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //_defaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Account/VerifyOTPAndActivateAccount";
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Xác minh tài khoản";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Verify(VerifyOTPModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.OTP_1) || string.IsNullOrEmpty(model.OTP_2)
                    || string.IsNullOrEmpty(model.OTP_3) || string.IsNullOrEmpty(model.OTP_4) || string.IsNullOrEmpty(model.OTP_5)
                    || string.IsNullOrEmpty(model.OTP_6))
                {
                    return View("Index", model);
                }
                string graftOTP = model.OTP_1.Trim() + model.OTP_2.Trim() + model.OTP_3.Trim() + model.OTP_4.Trim() + model.OTP_5.Trim() + model.OTP_6.Trim();
                model.OTP = graftOTP;

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //HttpResponseMessage response = await _client.PostAsync(_defaultApiUrl, content);
                HttpResponseMessage response = await _client.PostAsync(DefaultApiUrl + "Account/VerifyOTPAndActivateAccount", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Tài khoản đã được kích hoạt thành công.";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    ViewBag.ErrorToast = "Xác minh không thành công. Vui lòng kiểm tra lại email và mã OTP.";
                    return View("Index", model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorToast = "Đã xảy ra lỗi: " + ex.Message;
            }

            return View("Index", model);
        }
    }
}
