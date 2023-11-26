using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

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

        [HttpGet]
        public async Task<IActionResult> EditPet(int petId)
        {
            try
            {

                HttpResponseMessage response = await client.GetAsync("https://localhost:7255/api/PetInfo/PetID/" + petId);


                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    PetInfo managerInfos = System.Text.Json.JsonSerializer.Deserialize<PetInfo>(responseContent, options);

                    return View(managerInfos);
                }
                else
                {
                    TempData["ErrorToast"] = "Tải dữ liệu lên thất bại. Vui lòng tải lại trang.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> EditPet([FromForm] PetInfo petInfo, IFormFile image, int petId)
        {
            try
            {
                if (image != null && image.Length > 0)
                {
                    Console.WriteLine(image);
                    var imagePath = "/img/Pet/" + image.FileName;
                    petInfo.ImagePet = imagePath;

                    var physicalImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Pet", image.FileName);
                    using (var stream = new FileStream(physicalImagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                }
                else
                {
                    HttpResponseMessage responseForImage = await client.GetAsync("https://localhost:7255/api/PetInfo/PetID/" + petId);

                    if (responseForImage.IsSuccessStatusCode)
                    {
                        var responseContent = await responseForImage.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var pet = JsonConvert.DeserializeObject<PetInfo>(responseContent);

                            if (pet != null)
                            {
                                // Assign the existing image path to serviceCategory.Prictue.
                                petInfo.ImagePet = pet.ImagePet;
                            }
                        }
                    }
                }

                var json = JsonConvert.SerializeObject(petInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync("https://localhost:7255/api/PetInfo/UpdatePet?id=" + petId, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Chỉnh sửa thông tin thú cưng thành công!";
                    return View(petInfo); 
                }
                else
                {
                    TempData["ErrorToast"] = "Chỉnh sửa thông tin thú cưng thất bại. Vui lòng thử lại sau.";
                    return View(petInfo);
                }
            }

            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(petInfo); 
            }
        }

    }
}
