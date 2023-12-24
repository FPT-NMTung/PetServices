using FEPetServices.Areas.DTO;
using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Areas.Partner.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "PartnerOnly")]
    public class InformationPartnerController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;
        //private string DefaultApiUrlPartner = "";

        public InformationPartnerController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/UserInfo";
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Thông tin người dùng";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "UserInfo/" + email);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                AccountInfo managerInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);

                return View(managerInfos);
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

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] PartnerInfo partnerInfo, IFormFile image, IFormFile imagePartner)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            if (partnerInfo.Phone == null)
            {
                TempData["ErrorToast"] = "Số điện thoại không được để trống";
                return RedirectToAction("Index");
            }
            if (partnerInfo.Phone.Length != 10 && !partnerInfo.Phone.StartsWith("0"))
            {
                TempData["ErrorToast"] = "Số điện thoại phải bắt đầu bằng số 0 và có 10 chữ số";
                return RedirectToAction("Index");
            }

            if (partnerInfo.Address == null)
            {
                TempData["ErrorToast"] = "Địa chỉ cụ thể không được để trống";
                return RedirectToAction("Index");
            }
            if (partnerInfo.Address.Length <= 10)
            {
                TempData["ErrorToast"] = "Địa chỉ cụ thể phải lớn hơn 10 ký tự";
                return RedirectToAction("Index");
            }
            if (image != null)
            {
                string filename = GenerateRandomNumber(5) + image.FileName;
                filename = Path.GetFileName(filename);
                string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Profile/", filename);
                using (var stream = new FileStream(uploadfile, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                partnerInfo.ImagePartner = "/img/Profile/" + filename;
            }
            else
            {
                HttpResponseMessage responsePartner = await _client.GetAsync(DefaultApiUrl + "Partner/" + email);
                if (responsePartner.IsSuccessStatusCode)
                {
                    string responseContent = await responsePartner.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    AccountInfo managerInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                    partnerInfo.ImagePartner = managerInfos.PartnerInfo.ImagePartner;
                }
            }
            if (imagePartner != null)
            {
                string filename = GenerateRandomNumber(5) + imagePartner.FileName;
                filename = Path.GetFileName(filename);
                string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Certificate/", filename);
                using (var stream = new FileStream(uploadfile, FileMode.Create))
                {
                    await imagePartner.CopyToAsync(stream);
                }
                partnerInfo.ImageCertificate = "/img/Certificate/" + filename;
            }
            else
            {
                HttpResponseMessage responsePartner = await _client.GetAsync(DefaultApiUrl + "Partner/" + email);
                if (responsePartner.IsSuccessStatusCode)
                {
                    string responseContent = await responsePartner.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    AccountInfo managerInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                    partnerInfo.ImageCertificate = managerInfos.PartnerInfo.ImageCertificate;
                }
            }

            // Check if Dob is greater than the current date
            if (partnerInfo.Dob.HasValue && partnerInfo.Dob.Value > DateTime.Now)
            {
                TempData["ErrorToast"] = "Ngày sinh không thể lớn hơn ngày hiện tại";
                return RedirectToAction("Index");
            }

            if (partnerInfo.Address == null || partnerInfo.FirstName == null || partnerInfo.LastName == null)
            {
                TempData["ErrorToast"] = "Vui lòng điền đầy đủ thông tin";
                return RedirectToAction("Index");
            }

            if (partnerInfo.Lat != null && partnerInfo.Lng != null)
            {
                double? lat = TempData["Lat"] as double?;
                double? lng = TempData["Lng"] as double?;
                if (lat.HasValue && lng.HasValue)
                {
                    partnerInfo.Lat = lat.ToString();
                    partnerInfo.Lng = lng.ToString();
                }
            }
            HttpResponseMessage responseUser = await _client.GetAsync(DefaultApiUrl + "UserInfo/" + email);
            if (partnerInfo.Province == null || partnerInfo.District == null || partnerInfo.Commune == null )
            {
                
                if (responseUser.IsSuccessStatusCode)
                {
                    string responseContent = await responseUser.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    AccountInfo managerInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                    partnerInfo.Province = managerInfos.PartnerInfo.Province;
                    partnerInfo.District = managerInfos.PartnerInfo.District;
                    partnerInfo.Commune = managerInfos.PartnerInfo.Province; 
                }
            }
            if(partnerInfo.CardName == null)
            {
                if (responseUser.IsSuccessStatusCode)
                {
                    string responseContent = await responseUser.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    AccountInfo managerInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                    partnerInfo.CardName = managerInfos.PartnerInfo.CardName;
                }
            }

            // Update the user information, including the image URL
            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrl + "Partner/updateInfo?email=" + email, partnerInfo);
            //HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrlPartner + "?email=" + email, partnerInfo);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessToast"] = "Cập nhật thông tin thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationUpdateModel locationUpdate)
        {
            try
            {
                ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
                string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

                // Update the location in the database
                var updateLocationModel = new PartnerLocationDTO { Lat = locationUpdate.Lat.ToString(), Lng = locationUpdate.Lng.ToString() };
                HttpResponseMessage response = await _client.PutAsJsonAsync($"{DefaultApiUrl}Partner/UpdateLocation?email={email}", updateLocationModel);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.SuccessToast = "Cập nhật vị trí thành công";
                    return Ok();
                }
                else
                {
                    TempData["ErrorToast"] = "Lỗi khi cập nhật vị trí, vui lòng thử lại sau";
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult SaveLocation(double lat, double lng)
        {
            TempData["Lat"] = lat;
            TempData["Lng"] = lng;

            return Ok();
        }
    }
}