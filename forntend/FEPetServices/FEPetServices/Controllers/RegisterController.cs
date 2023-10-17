using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FEPetServices.Form; // Import namespace chứa class RegisterDTO
using Newtonsoft.Json;

namespace FEPetServices.Controllers
{
    public class RegisterController : Controller
    {
        private readonly HttpClient _client;
        private string _defaultApiUrl;

        public RegisterController()
        {
            _client = new HttpClient();
            _defaultApiUrl = "https://localhost:7255/api/Account/Register"; // URL của API Register
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] RegisterDTO registerInfo)
        {
            try
            {
                // Chuyển thông tin đăng ký thành dạng JSON
                var json = JsonConvert.SerializeObject(registerInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi yêu cầu POST đến API Register
                HttpResponseMessage response = await _client.PostAsync(_defaultApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Đăng ký thành công, bạn có thể xử lý kết quả ở đây (ví dụ: hiển thị thông báo thành công)
                    ViewBag.SuccessMessage = "Đăng ký thành công!";
                }
                else
                {
                    // Đăng ký không thành công, bạn có thể xử lý kết quả ở đây (ví dụ: hiển thị thông báo lỗi)
                    ViewBag.ErrorMessage = "Đăng ký không thành công. Mã lỗi HTTP: " + (int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }

            return View();
        }
    }
}
