using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FEPetServices.Form; // Import namespace chứa class RegisterDTO
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net;

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
                if (registerInfo.Email == null || registerInfo.Password == null)
                {
                    return View();
                }
                // Chuyển thông tin đăng ký thành dạng JSON
                var json = JsonConvert.SerializeObject(registerInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi yêu cầu POST đến API Register
                HttpResponseMessage response = await _client.PostAsync(_defaultApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Đăng ký thành công, bạn có thể xử lý kết quả ở đây (ví dụ: hiển thị thông báo thành công)
                    /*ViewBag.SuccessMessage = "Đăng ký thành công!";*/

                    var sendOtpResult = await CallSendOTP(registerInfo.Email);
                    if (!string.IsNullOrEmpty(sendOtpResult) && sendOtpResult == "Gửi OTP thành công.")
                    {
                        // Thành công khi gửi OTP
                        TempData["SuccessRegisterToast"] = "Mã OTP đã được gửi đến hòm thư của bạn.";
                    }
                    else
                    {
                        // Xử lý lỗi khi gọi API SendOTP hoặc gửi OTP qua email không thành công
                        ViewBag.ErrorToast = "Lỗi khi gọi API SendOTP hoặc gửi OTP qua email: " + sendOtpResult;
                    }

                    return RedirectToAction("Index", "VerifyEmail");
                }
                else
                {
                    // Đăng ký không thành công, bạn có thể xử lý kết quả ở đây (ví dụ: hiển thị thông báo lỗi)
                    ViewBag.ErrorToast = "Đăng ký không thành công. Mã lỗi HTTP: " + (int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                ViewBag.ErrorToast = "Đã xảy ra lỗi: " + ex.Message;
            }

            return View();
        }

        private async Task<string> CallSendOTP(string email)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri("https://localhost:7255/"); // Đổi URL của API SendOTP theo cấu hình của bạn
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Account/SendOTP", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultString = await response.Content.ReadAsStringAsync();

                    // Sử dụng JSON.NET để phân tích chuỗi JSON thành đối tượng
                    var result = JsonConvert.DeserializeObject<OTPReturnResponse>(resultString);

                    if (!string.IsNullOrEmpty(result.Email) && result.OTP > 0)
                    {
                        // Gọi thành công API SendOTP
                        SendOTPByEmail(result.Email, result.OTP);
                        return "Gửi OTP thành công.";
                    }
                }

                return "Gửi OTP không thành công.";
            }
        }

        private void SendOTPByEmail(string email, int otp)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("psmsg65@gmail.com", "pfrn dczf xruz sona");
                    client.EnableSsl = true;

                    var message = new MailMessage();
                    message.From = new MailAddress("psmsg65@gmail.com");
                    message.Subject = "Mã OTP kích hoạt tài khoản đăng ký";
                    message.Body = "Chúc mừng bạn đã đăng ký thành công vào hệ thống của chúng tôi và đây là Mã OTP dùng để xác minh tài khoản của bạn: " + otp;
                    message.IsBodyHtml = true;
                    message.To.Add(email);

                    client.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi gửi email, ví dụ: Log lỗi hoặc hiển thị thông báo lỗi
            }
        }
    }
}
