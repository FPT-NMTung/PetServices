using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Xml.Linq;

namespace FEPetServices.Controllers
{
    public class ForgotPassword : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewData["error"] = "Email is required.";
                return View("Index");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri("https://localhost:7255/"); // Đổi URL của API theo cấu hình của bạn
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsJsonAsync("api/Account/ForgotPassword", email);

                if (response.IsSuccessStatusCode)
                {
                    var resultString = await response.Content.ReadAsStringAsync();

                    // Sử dụng JSON.NET để phân tích chuỗi JSON thành đối tượng
                    var result = JsonConvert.DeserializeObject<PasswordResetResponse>(resultString);

                    if (result.NewPass == "NotFound")
                    {
                        ViewData["error"] = "Email không tồn tại.";
                    }
                    else
                    {
                        string pass = result.NewPass;

                        // Gửi mật khẩu mới qua email
                        SendPasswordResetEmail(email, pass);

                        ViewData["messageSuccess"] = "Yêu cầu đặt lại mật khẩu đã được gửi thành công. Vui lòng kiểm tra email của bạn.";
                        ViewData["emailSent"] = true; // Đánh dấu rằng email đã được gửi thành công
                    }
                }
                else
                {
                    ViewData["error"] = "Có lỗi xảy ra khi gửi yêu cầu đặt lại mật khẩu.";
                }
            }

            return View("Index");
        }


        private void SendPasswordResetEmail(string email, string newPassword)
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
                    message.Subject = "Yêu cầu đặt lại mật khẩu";
                    message.Body = "Mật khẩu mới của bạn: " + newPassword;
                    message.IsBodyHtml = true;
                    message.To.Add(email);

                    client.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi gửi email
                // Ví dụ: Log lỗi hoặc hiển thị thông báo lỗi
            }
        }
    }
}
