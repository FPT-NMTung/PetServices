using Microsoft.AspNetCore.Mvc;
using System.Text;
using FEPetServices.Form; 
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net;

namespace FEPetServices.Controllers
{
    public class RegisterController : Controller
    {
        private readonly HttpClient _client;
        //private string _defaultApiUrl;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public RegisterController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            //_defaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Account/Register"; 
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Đăng ký tài khoản";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] RegisterDTO registerInfo)
        {
            try
            {
                ViewBag.Title = "Đăng ký tài";
                if (registerInfo.Password.Length < 8)
                {
                    ViewBag.ErrorToast = "Mật khẩu phải trên hoặc bằng 8 ký tự";
                    return View();
                }
                var json = JsonConvert.SerializeObject(registerInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //HttpResponseMessage response = await _client.PostAsync(_defaultApiUrl, content);
                HttpResponseMessage response = await _client.PostAsync(DefaultApiUrl + "Account/Register", content);

                if (response.IsSuccessStatusCode)
                {
                    var sendOtpResult = await CallSendOTP(registerInfo.Email);
                    if (!string.IsNullOrEmpty(sendOtpResult) && sendOtpResult == "Gửi OTP thành công.")
                    {
                        TempData["SuccessToast"] = "Mã OTP đã được gửi đến hòm thư của bạn.";
                    }
                    else
                    {
                        ViewBag.ErrorToast = "Lỗi khi gọi API SendOTP hoặc gửi OTP qua email: " + sendOtpResult;
                    }

                    return RedirectToAction("Index", "VerifyEmail");
                }
                /*else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ViewBag.ErrorToast = response.Content.ReadAsStringAsync();

                }*/
                else
                {
                    ViewBag.ErrorToast = "Đăng ký không thành công. Tài khoản đã tồn tại";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorToast = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View();
        }

        private async Task<string> CallSendOTP(string email)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri("https://pet-service-api.azurewebsites.net/");  
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Account/SendOTP", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultString = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<OTPReturnResponse>(resultString);

                    if (!string.IsNullOrEmpty(result.Email) && result.OTP > 0)
                    {
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
                    client.Credentials = new NetworkCredential("psmsg65@gmail.com", "wztg xjpz szer pvmk");
                    client.EnableSsl = true;

                    var message = new MailMessage();
                    message.From = new MailAddress("psmsg65@gmail.com");
                    message.Subject = "Mã OTP kích hoạt tài khoản đăng ký";
                    /*message.Body = "<h1>Chúc mừng bạn đã đăng ký thành công vào hệ thống của chúng tôi và đây là Mã OTP dùng để xác minh tài khoản của bạn: </h1>" + otp;*/

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
                                                                Chào mừng bạn đã đến với chúng tôi.
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
                                                        <p style=""margin: 0px; padding: 0px;"">Chúc mừng và chào mừng bạn trở lại với cộng đồng toàn những thành viên thân thiện và đáng yêu này nhé ✨✨✨</p>
                                                        <p style=""margin: 0px; padding: 0px;""><br></p>
                                                        <p style=""margin: 0px; padding: 0px;"">Đây là mã OTP để xác nhận tài khoản của bạn !!!</p>
                                                        <p style=""margin: 10px; padding: 0px; font-size: 17px; text-align: center;"">Tài khoản: "+ email + @"</p>
                                                        <p style=""margin: 0px; padding: 0px; font-size: 17px; text-align: center;"">Mã OTP của bạn: "+ otp + @"</p>
                                                        <p class=""text-right"" style=""text-align: right; margin: 0px; padding: 0px;"">
                                                            <a href=""#"" style=""color: #16c2d0; font-size: 14px; font-family: Helvetica, Arial, sans-serif; text-decoration: none;""><span><u>Đăng nhập ngay&gt;&gt;</u></span></a>
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
            catch (Exception ex)
            {
                // Xử lý lỗi khi gửi email, ví dụ: Log lỗi hoặc hiển thị thông báo lỗi
            }
        }
    }
}
