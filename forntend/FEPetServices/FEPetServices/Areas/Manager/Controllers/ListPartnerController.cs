using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
    [Authorize(Policy = "ManaOnly")]
    public class ListPartnerController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public ListPartnerController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            //DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Partner";
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Partner");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                List<AccountInfo> listAccounts = System.Text.Json.JsonSerializer.Deserialize<List<AccountInfo>>(responseContent, options);
                return View(listAccounts);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartner(string email)
        {

            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Partner/" + email);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                AccountInfo accountInfo = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                return View(accountInfo);
            }
            else {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DetailPartner(string email, string password)
        {
            HttpResponseMessage response = await _client.PutAsync(DefaultApiUrl + "Partner/updateAccount?email=" + email, null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessToast"] = "Cấp tài khoản thành công";
                SendEmail(email, password);
                return RedirectToAction("DetailPartner", new { email = email });
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return RedirectToAction("DetailPartner", new { email = email });
            }
        }

        private void SendEmail(string email, string password)
        {
            using (var client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("psmsg65@gmail.com", "pfrn dczf xruz sona");
                client.EnableSsl = true;

                var message = new MailMessage();
                message.From = new MailAddress("psmsg65@gmail.com");
                message.Subject = "Tài khoản của bạn đã được kích hoạt";
                message.Body = @"
<!DOCTYPE html>
<html>
<head>
<style>
    .commonx {
        font-family: Arial, sans-serif;
        background-image: url('background.jpg');
        background-size: cover;
        margin: 0;
        padding: 0;
        display: flex;
        justify-content: center;
        align-items: center;
        text-align:center;
        height: 100vh;
    }

    .container {
        background-color: #f9f9f9;
        border-radius: 10px;
        box-shadow: 0 0 20px rgba(0, 0, 0, 0.2);
        padding: 20px;
        max-width: 400px;
        width: 100%;
    }

    .header {
        text-align: center;
        padding: 20px;
    }

    h1 {
        font-size: 32px;
        color: #007bff;
    }

    h4 {
        font-size: 20px;
        color: #555;
    }

    .content {
        margin-top: 20px;
    }

    p {
        font-size: 18px;
        color: #333;
        line-height: 1.5;
    }

    .otp {
        background-color: #fff;
        border: 1px solid #ccc;
        border-radius: 5px;
        padding: 10px;
        margin: 20px 0;
    }

    .otp p {
        margin: 5px 0;
        font-size: 16px;
        color: #555;
    }

    .login-link {
        text-align: center;
        margin-top: 20px;
    }

    a {
        text-decoration: none;
        background-color: #007bff;
        color: #fff;
        padding: 15px 30px;
        border-radius: 5px;
        transition: background-color 0.3s;
        font-size: 20px;
        display: inline-block;
    }

    a:hover {
        background-color: #0056b3;
    }
</style>
</head>
<body>
    <div class=""commonx"">
        <div class=""container"">
            <div class=""header"">
                <h1>Chào mừng bạn đã đến với chúng tôi.</h1>
                <h4>Những người yêu động vật.</h4>
            </div>
            <div class=""content"">
                <p>Chúc mừng và chào mừng bạn đã trở thành một thành viên của chúng tôi✨✨✨</p>
                <p>Đây là tài khoản và mật khẩu của bạn:</p>
                <div class=""otp"">
                    <p>Tài khoản: " + email + @"</p>
                    <p>Mật khẩu: " + password + @"</p>
                </div>
                <div class=""login-link"" style=""color:#ffffff;"">
                    <a style=""color:#ffffff;"" href=""#"">Đăng nhập ngay</a>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
";

                message.IsBodyHtml = true;
                message.To.Add(email);

                client.Send(message);
            }
        }
    }
}
