using Microsoft.AspNetCore.Mvc;
using System.Text;
using FEPetServices.Form; 
using Newtonsoft.Json;
namespace FEPetServices.Controllers
{
    public class PartnerRegisterController : Controller
    {
        private readonly HttpClient _client;
        /*private string _defaultApiUrl;*/
        private string DefaultApiUrl = "";

        private readonly IConfiguration configuration;
        public PartnerRegisterController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            /*_defaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Account/RegisterPartner";*/
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
        }
        public IActionResult Index()
        {
            ViewBag.Title = "Đăng ký tài khoản nhân viên";
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
                var json = JsonConvert.SerializeObject(registerInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //HttpResponseMessage response = await _client.PostAsync(_defaultApiUrl, content);
                HttpResponseMessage response = await _client.PostAsync(DefaultApiUrl + "Account/RegisterPartner", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Vui lòng chờ đợi quản trị viên xét duyệt thông tin tài khoản của bạn";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    ViewBag.ErrorToast = "Đăng ký không thành công. Mã lỗi HTTP: " + (int)response.StatusCode;
                    return View();
                }
            }
            catch (Exception ex)
            {
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
