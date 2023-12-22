using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
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
            ViewBag.Title = "Danh sách nhân viên";
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Partner");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
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
            ViewBag.Title = "Thông tin nhân viên";
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Partner/" + email);
            //HttpResponseMessage response = await _client.GetAsync("https://localhost:7255/api/" + "Partner/" + email);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
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
                client.Credentials = new NetworkCredential("psmsg65@gmail.com", "wztg xjpz szer pvmk");
                client.EnableSsl = true;

                var message = new MailMessage();
                message.From = new MailAddress("psmsg65@gmail.com");
                message.Subject = "Tài khoản của bạn đã được kích hoạt";
                message.Body = @"<body style=""padding: 0; margin: 0; background: #efefef"">
    <table style=""height: 100%; width: 100%; background-color: #efefef;"" align=""center"">
        <tbody>
            <tr>
                <td valign=""top"" id=""dbody"" data-version=""2.31"" style=""width: 100%; height: 100%; padding-top: 30px; padding-bottom: 30px; background-color: #efefef;"">
                    <table class=""layer_1"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""max-width: 600px; box-sizing: border-box; width: 100%; margin: 0px auto;"">
                        <tbody>
                            <tr>
                                <td class=""drow"" valign=""top"" align=""center"" style=""background-color: #ffffff; box-sizing: border-box; font-size: 0px; text-align: center;"">
                                    <div class=""layer_2"" style=""max-width: 600px; display: inline-block; vertical-align: top; width: 100%;"">
                                        <table border=""0"" cellspacing=""0"" cellpadding=""0"" class=""edcontent"" style=""border-collapse: collapse; width: 100%;"">
                                            <tbody>
                                                <tr>
                                                    <td valign=""top"" class=""emptycell"" style=""padding: 10px;"">
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class=""drow"" valign=""top"" align=""center"" style=""background-color: #ffffff; box-sizing: border-box; font-size: 0px; text-align: center;"">
                                    <div class=""layer_2"" style=""max-width: 600px; display: inline-block; vertical-align: top; width: 100%;"">
                                        <table border=""0"" cellspacing=""0"" cellpadding=""0"" class=""edcontent"" style=""border-collapse: collapse; width: 100%;"">
                                            <tbody>
                                                <tr>
                                                    <td valign=""top"" class=""edimg"" style=""padding: 0px; box-sizing: border-box; text-align: center;"">
                                                        <img src=""https://api.elasticemail.com/userfile/a18de9fc-4724-42f2-b203-4992ceddc1de/geometric_divider1.png"" alt=""Image"" width=""576"" style=""border-width: 0px; border-style: none; max-width: 576px; width: 100%;"">
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class=""drow"" valign=""top"" align=""center"" style=""background-color: #ffffff; box-sizing: border-box; font-size: 0px; text-align: center;"">
                                    <div class=""layer_2"" style=""max-width: 600px; display: inline-block; vertical-align: top; width: 100%;"">
                                        <table border=""0"" cellspacing=""0"" class=""edcontent"" style=""border-collapse: collapse; width: 100%;"">
                                            <tbody>
                                                <tr>
                                                    <td valign=""top"" class=""edtext"" style=""padding: 20px; text-align: left; color: #5f5f5f; font-size: 14px; font-family: Helvetica, Arial, sans-serif; word-break: break-word; direction: ltr; box-sizing: border-box;"">
                                                        <p class=""style1 text-center"" style=""text-align: center; margin: 0px; padding: 0px; color: #f24656; font-size: 36px; font-family: Helvetica, Arial, sans-serif;"">
                                                            <strong>
                                                                Chào mừng bạn đến với hệ thống  của chúng tôi
                                                            </strong>
                                                        </p>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class=""drow"" valign=""top"" align=""center"" style=""background-color: #ffffff; box-sizing: border-box; font-size: 0px; text-align: center;"">
                                    <div class=""layer_2"" style=""max-width: 600px; display: inline-block; vertical-align: top; width: 100%;"">
                                        <table border=""0"" cellspacing=""0"" class=""edcontent"" style=""border-collapse: collapse; width: 100%;"">
                                            <tbody>
                                                <tr>
                                                    <td valign=""top"" class=""edtext"" style=""padding: 20px; text-align: left; color: #5f5f5f; font-size: 14px; font-family: Helvetica, Arial, sans-serif; word-break: break-word; direction: ltr; box-sizing: border-box;"">
                                                        <p style=""margin: 0px; padding: 0px;"">Email của bạn đã được Quản lý kích hoạt thành công!! ✨✨✨</p>
                                                        <p style=""margin: 0px; padding: 0px;""><br></p>
                                                        <p style=""margin: 0px; padding: 0px; font-size: 17px; text-align: center;"">Bạn có thể truy cập hệ thống với email đã đăng kỳ:  " + email + @"</p>
                                                        <p class=""text-right"" style=""text-align: right; margin: 0px; padding: 0px;"">
                                                            <a href=""https://pet-service.azurewebsites.net/Login"" style=""color: #16c2d0; font-size: 14px; font-family: Helvetica, Arial, sans-serif; text-decoration: none;""><span><u>Đăng nhập ngay&gt;&gt;</u></span></a>
                                                        </p>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td valign=""top"" class=""edimg"" style=""padding: 0px; box-sizing: border-box; text-align: center;"">
                                                        <img src=""https://api.elasticemail.com/userfile/a18de9fc-4724-42f2-b203-4992ceddc1de/geometric_divider1.png"" alt=""Image"" width=""576"" style=""border-width: 0px; border-style: none; max-width: 576px; width: 100%;"">
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class=""drow"" valign=""top"" align=""center"" style=""background-color: #ffffff; box-sizing: border-box; font-size: 0px; text-align: center;"">
                                    <div class=""layer_2"" style=""max-width: 600px; display: inline-block; vertical-align: top; width: 100%;"">
                                        <table border=""0"" cellspacing=""0"" class=""edcontent"" style=""border-collapse: collapse; width: 100%;"">
                                            <tbody>
                                                <tr>
                                                    <td valign=""top"" class=""emptycell"" style=""padding: 10px;"">
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class=""drow"" valign=""top"" align=""center"" style=""background-color: #ffffff; box-sizing: border-box; font-size: 0px; text-align: center;"">
                                    <div class=""layer_2"" style=""max-width: 600px; display: inline-block; vertical-align: top; width: 100%;"">
                                        <table border=""0"" cellspacing=""0"" class=""edcontent"" style=""border-collapse: collapse; width: 100%;"">
                                            <tbody>
                                                <tr>
                                                    <td valign=""top"" class=""emptycell"" style=""padding: 10px;"">
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
</body>";

                message.IsBodyHtml = true;
                message.To.Add(email);

                client.Send(message);
            }
        }
    }
}
