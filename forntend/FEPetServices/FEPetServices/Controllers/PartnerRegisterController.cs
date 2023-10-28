using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace FEPetServices.Controllers
{
    public class PartnerRegisterController : Controller
    {
        private readonly HttpClient _client;
        private string _defaultApiUrl;
        public PartnerRegisterController()
        {
            _client = new HttpClient();
            _defaultApiUrl = "https://localhost:7255/api/Account/RegisterPartner"; // URL của API Register   
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] RegisterDTO registerInfo, IFormFile image)
        {
            try
            {
                /* if (image != null && image.Length > 0)
                 {
                     // Xử lý và lưu trữ ảnh
                     Console.WriteLine(image);
                     registerInfo.ImageCertificate = "/img/" + image.FileName.ToString();
                 }*/
                if (image != null)
                {
                    string filename = GenerateRandomNumber(5) + image.FileName;
                    filename = Path.GetFileName(filename);
                    string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/", filename);
                    using (var stream = new FileStream(uploadfile, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    registerInfo.ImageCertificate = "/img/" + filename;
                }
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

        public static string GenerateRandomNumber(int length)
        {
            Random random = new Random();
            const string chars = "0123456789"; // Chuỗi chứa các chữ số từ 0 đến 9
            char[] randomChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomChars);
        }
    }


}
