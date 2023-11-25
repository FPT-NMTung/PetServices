using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace FEPetServices.Areas.Customer.Controllers
{
    public class PetController : Controller
    {
        private readonly HttpClient client = null;
        private readonly IConfiguration configuration;
        private string DefaultApiUrl = "";

        public PetController(IConfiguration configuration)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.configuration = configuration;
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
        }

       public class PetModel{
            public AccountInfo AccountInfo { get; set; }
          
        }
        public async Task<IActionResult> AddPet([FromForm] PetInfo petInfo, List<IFormFile> image)
        {     
            PetModel model = new PetModel();
            try
            {
                ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
                string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
                HttpResponseMessage PetInfo = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/PetInfo/" + email);

                if (PetInfo.IsSuccessStatusCode)
                {
                    var responsePetInfo = await PetInfo.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responsePetInfo))
                    {
                        var account = JsonConvert.DeserializeObject<AccountInfo>(responsePetInfo);
                        ViewBag.account = account;
                    }
                }


                if (petInfo.PetName == null) { return View(); }
                    foreach (var file in image)
                    {
                        string filename = GenerateRandomNumber(5) + file.FileName;
                        filename = Path.GetFileName(filename);
                        string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Pet/", filename);
                        var stream = new FileStream(uploadfile, FileMode.Create);
                        await file.CopyToAsync(stream); // Chờ đợi hoàn thành việc sao chép file
                       petInfo.ImagePet = "/img/Pet/" + filename;
                    }

                    var json = JsonConvert.SerializeObject(petInfo);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("https://localhost:7255/api/PetInfo/CreatePet", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessToast"] = "Thêm thông tin thú cưng thành công!";
                        return View(petInfo);
                    }
                    else
                    {
                        TempData["ErrorToast"] = "Thêm thông tin thú cưng thất bại. Vui lòng thử lại sau.";
                        return View(petInfo);
                    }
                }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(petInfo);
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
