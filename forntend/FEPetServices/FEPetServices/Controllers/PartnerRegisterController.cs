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
            _defaultApiUrl = "https://localhost:7255/api/Account/RegisterPartner"; 
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
                string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/", filename);
                var stream = new FileStream(uploadfile, FileMode.Create);
                file.CopyToAsync(stream);
                registerInfo.ImageCertificate = "/img/" + filename;
            }
            try
            {              
                var json = JsonConvert.SerializeObject(registerInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync(_defaultApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.SuccessMessage = "Đăng ký thành công!";
                }
                else
                {
                    ViewBag.ErrorMessage = "Đăng ký không thành công. Mã lỗi HTTP: " + (int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
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
