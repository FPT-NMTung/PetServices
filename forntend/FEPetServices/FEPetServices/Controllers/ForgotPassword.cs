using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace FEPetServices.Controllers
{
    public class ForgotPassword : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Quên mật khẩu";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordEmail(string email)
        {
            ViewBag.Title = "Quên mật khẩu";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri("https://pet-service-api.azurewebsites.net/"); 
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsJsonAsync("api/Account/ForgotPassword", email);

                if (response.IsSuccessStatusCode)
                {
                    var resultString = await response.Content.ReadAsStringAsync();

                    // Sử dụng JSON.NET để phân tích chuỗi JSON thành đối tượng
                    var result = JsonConvert.DeserializeObject<PasswordResetResponse>(resultString);

                    if (result.NewPassword == "NotFound")
                    {
                        ViewBag.ErrorToast = "Tài khoản không tồn tại.";
                        return View("Index");
                    }
                    else
                    {
                        string pass = result.NewPassword;
                        // Gửi mật khẩu mới qua email
                        SendPasswordResetEmail(email, pass);
                        TempData["SuccessToast"] = "Yêu cầu đặt lại mật khẩu đã được gửi thành công. Vui lòng kiểm tra email của bạn.";
                        return RedirectToAction("Index","Login");
                    }
                }
                else
                {
                    ViewBag.ErrorToast = "Có lỗi xảy ra khi gửi yêu cầu đặt lại mật khẩu hoặc tài khoản không tồn tại.";
                    return View("Index");
                }
            }
        }


        private void SendPasswordResetEmail(string email, string newPassword)
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
                    message.Subject = "Yêu cầu đặt lại mật khẩu";
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
                                                                Thông báo lấy lại mật khẩu.
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
                                                        <p style=""margin: 0px; padding: 0px;"">Lấy lại mật khẩu thành công ✨✨✨</p>
                                                        <p style=""margin: 0px; padding: 0px;""><br></p>
                                                        <p style=""margin: 0px; padding: 0px;"">Đây là mật khẩu mới của bạn !!!</p>
                                                        <p style=""margin: 0px; padding: 0px; font-size: 17px; text-align: center;"">Mật khẩu của bạn là: " + newPassword + @"</p>
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
            catch (Exception ex)
            {
                // Xử lý lỗi khi gửi email
                // Ví dụ: Log lỗi hoặc hiển thị thông báo lỗi
            }
        }
    }
}
