using Microsoft.AspNetCore.Mvc;
using System.Text;
using FEPetServices.Form; 
using Newtonsoft.Json;
namespace FEPetServices.Controllers
{
    public class PartnerRegisterController : Controller
    {
        private readonly HttpClient _client;
        private string _defaultApiUrl;
        public PartnerRegisterController()
        {
            _client = new HttpClient();
            _defaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Account/RegisterPartner"; 
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] RegisterDTO registerInfo, List<IFormFile> image)
        {
            foreach (var file in image)
            {
                string filename = GenerateRandomNumber(5) + file.FileName;
                filename = Path.GetFileName(filename);
                string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/partner/", filename);
                var stream = new FileStream(uploadfile, FileMode.Create);
                file.CopyToAsync(stream);
                registerInfo.ImageCertificate = "/img/partner/" + filename;
            }
            try
            {
                // Chuyển thông tin đăng ký thành dạng JSON
                var json = JsonConvert.SerializeObject(registerInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync(_defaultApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Đăng ký thành công, bạn có thể xử lý kết quả ở đây (ví dụ: hiển thị thông báo thành công)
                    TempData["SuccessRegisterSuccessToast"] = "Vui lòng chờ đợi quản trị viên xét duyệt thông tin tài khoản của bạn";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    // Đăng ký không thành công, bạn có thể xử lý kết quả ở đây (ví dụ: hiển thị thông báo lỗi)
                    ViewBag.ErrorToast = "Đăng ký không thành công. Mã lỗi HTTP: " + (int)response.StatusCode;
                    return View();
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                ViewBag.ErrorToast = "Đã xảy ra lỗi: " + ex.Message;
                return View();
            }
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
