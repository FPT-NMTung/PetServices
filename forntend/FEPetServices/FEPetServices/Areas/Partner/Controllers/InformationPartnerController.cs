using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Areas.Partner.Controllers
{
    [Authorize(Policy = "PartnerOnly")]
    public class InformationPartnerController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlPartner = "";

        public InformationPartnerController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/UserInfo";
            DefaultApiUrlPartner = "https://pet-service-api.azurewebsites.net/api/Partner/updateInfo";
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "/" + email);

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
        public async Task<IActionResult> Index([FromForm] PartnerInfo partnerInfo, IFormFile image)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            if (partnerInfo.Phone == null)
            {
                TempData["ErrorToast"] = "Số điện thoại không được để trống";
                return RedirectToAction("Index");
            }
            if (partnerInfo.Phone.Length == 10 && partnerInfo.Phone.StartsWith("0"))
            {

            }
            else
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
            // Handle the uploaded image
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
                HttpResponseMessage responseUser = await _client.GetAsync(DefaultApiUrlPartner + "?email=" + email);
                if (responseUser.IsSuccessStatusCode)
                {
                    string responseContent = await responseUser.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    AccountInfo managerInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                    partnerInfo.ImagePartner = managerInfos.PartnerInfo.ImagePartner;
                }
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

            if (partnerInfo.Province == null || partnerInfo.District == null || partnerInfo.Commune == null)
            {
                HttpResponseMessage responseUser = await _client.GetAsync(DefaultApiUrl + "/" + email);
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

            // Update the user information, including the image URL
            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrlPartner + "?email=" + email, partnerInfo);
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
        public IActionResult SaveLocation(double lat, double lng)
        {
            TempData["Lat"] = lat;
            TempData["Lng"] = lng;

            return Ok();
        }
    }
}