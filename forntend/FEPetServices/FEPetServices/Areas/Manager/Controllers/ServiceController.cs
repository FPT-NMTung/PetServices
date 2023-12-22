using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PetServices.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "ManaOnly")]
    public class ServiceController : Controller
    {
        private readonly HttpClient client = null;
        private readonly IConfiguration configuration;
        private string DefaultApiUrl = "";
        //private string DefaultApiUrlServiceList = "";
        //private string DefaultApiUrlServiceAdd = "";
        //private string DefaultApiUrlServiceCategoryList = "";
        //private string DefaultApiUrlServiceDetail = "";
        //private string DefaultApiUrlServiceUpdate = "";

        public ServiceController(IConfiguration configuration)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.configuration = configuration;
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrl = "";
            //DefaultApiUrlServiceList = "https://pet-service-api.azurewebsites.net/api/Service/GetAllService";
            //DefaultApiUrlServiceAdd = "https://pet-service-api.azurewebsites.net/api/Service/CreateService";
            //DefaultApiUrlServiceCategoryList = "https://pet-service-api.azurewebsites.net/api/ServiceCategory/GetAllServiceCategory";
            //DefaultApiUrlServiceDetail = "https://pet-service-api.azurewebsites.net/api/Service/ServiceID";
            //DefaultApiUrlServiceUpdate = "https://pet-service-api.azurewebsites.net/api/Service/UpdateServices?serviceId=";

        }
        public async Task<IActionResult> Index(ServiceDTO service)
        {
            try
            {
                ViewBag.Title = "Danh sách dịch vụ";
                var json = JsonConvert.SerializeObject(service);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Service/GetAllService");
                //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlServiceList);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var serviceList = JsonConvert.DeserializeObject<List<ServiceDTO>>(responseContent);
                        //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                        return View(serviceList);
                    }
                    else
                    {
                        ViewBag.ErrorToast = "API trả về dữ liệu rỗng.";
                    }
                }
                else
                {
                    ViewBag.ErrorToast = "Tải dữ liệu lên thất bại. Vui lòng tải lại trang.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorToast = "Đã xảy ra lỗi: " + ex.Message;
            }

            // Return the view with or without an error message
            return View();
        }

        public async Task<IActionResult> AddService([FromForm] ServiceDTO service, List<IFormFile> image)
        {
            try
            {
                ViewBag.Title = "Thêm dịch vụ mới";
                HttpResponseMessage categoryResponse = await client.GetAsync(DefaultApiUrl + "ServiceCategory/GetAllServiceCategory");

                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categories = await categoryResponse.Content.ReadFromJsonAsync<List<ServiceCategoryDTO>>();
                    categories = categories.Where(r => r.Status == true).ToList();
                    ViewBag.Categories = new SelectList(categories, "SerCategoriesId", "SerCategoriesName");
                }

                if (service.ServiceName == null) { return View(); }
                foreach (var file in image)
                {
                    string filename = GenerateRandomNumber(5) + file.FileName;
                    filename = Path.GetFileName(filename);
                    string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Service/", filename);
                    var stream = new FileStream(uploadfile, FileMode.Create);
                    file.CopyToAsync(stream);
                    service.Picture = "/img/Service/" + filename;
                }

                service.Status = true;

                var json = JsonConvert.SerializeObject(service);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi dữ liệu lên máy chủ
                HttpResponseMessage response = await client.PostAsync(DefaultApiUrl + "Service/CreateService", content);
                //HttpResponseMessage response = await client.PostAsync(DefaultApiUrlServiceAdd, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Thêm dịch vụ thành công!";
                    return View(); // Chuyển hướng đến trang thành công hoặc trang danh sách
                }
                else
                {
                    TempData["ErrorToast"] = "Thêm dịch vụ thất bại. Vui lòng thử lại sau.";
                    return View(); // Hiển thị lại biểu mẫu với dữ liệu đã điền
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(service); // Hiển thị lại biểu mẫu với dữ liệu đã điền
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
        public async Task<IActionResult> EditService(int ServiceId)
        {
            try
            {
                ViewBag.Title = "Chỉnh sửa thông tin dịch vụ";
                HttpResponseMessage categoryResponse = await client.GetAsync(DefaultApiUrl + "ServiceCategory/GetAllServiceCategory");
                //HttpResponseMessage categoryResponse = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/ServiceCategory/GetAllServiceCategory");

                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categories = await categoryResponse.Content.ReadFromJsonAsync<List<ServiceCategoryDTO>>();
                    categories = categories.Where(r => r.Status == true).ToList();
                    ViewBag.Categories = new SelectList(categories, "SerCategoriesId", "SerCategoriesName");
                }
                //goi api de lay thong tin can sua
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Service/ServiceID/" + ServiceId);
                //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlServiceDetail + "/" + ServiceId);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    ServiceDTO managerInfos = System.Text.Json.JsonSerializer.Deserialize<ServiceDTO>(responseContent, options);

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
        public async Task<IActionResult> EditService([FromForm] ServiceDTO service, IFormFile image, int ServiceId, int SelectedCategory)
        {

            try
            {

                HttpResponseMessage categoryResponse = await client.GetAsync(DefaultApiUrl + "ServiceCategory/GetAllServiceCategory");
                //HttpResponseMessage categoryResponse = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/ServiceCategory/GetAllServiceCategory");

                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categories = await categoryResponse.Content.ReadFromJsonAsync<List<ServiceCategoryDTO>>();
                    ViewBag.Categories = new SelectList(categories, "SerCategoriesId", "SerCategoriesName");
                }

                if (image != null && image.Length > 0)
                {
                    var imagePath = "/img/Service/" + image.FileName;
                    service.Picture = imagePath;

                    var physicalImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Service",image.FileName);
                    using (var stream = new FileStream(physicalImagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                }
                else
                {
                    HttpResponseMessage responseForImage = await client.GetAsync(DefaultApiUrl + "Service/ServiceID/" + ServiceId);
                    if (responseForImage.IsSuccessStatusCode)
                    {
                        var responseContent = await responseForImage.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var existingServiceCategory = JsonConvert.DeserializeObject<ServiceDTO>(responseContent);
                            if (existingServiceCategory != null)
                            {
                                service.Picture = existingServiceCategory.Picture;
                            }
                        }
                    }
                }

                if (Request.Form["Status"] == "on")
                {
                    service.Status = true;
                }
                else
                {
                    service.Status = false;
                }

                var json = JsonConvert.SerializeObject(service);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send the data to the server
                HttpResponseMessage response = await client.PutAsync(DefaultApiUrl + "Service/UpdateServices?serviceId=" + ServiceId, content);
                //HttpResponseMessage response = await client.PutAsync(DefaultApiUrlServiceUpdate + ServiceId, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Chỉnh sửa dịch vụ thành công!";
                    return RedirectToAction("EditService", new { serviceId = ServiceId }); // Redirect to a success page or a list page
                }
                else
                {
                    TempData["ErrorToast"] = "Chỉnh sửa dịch vụ thất bại. Vui lòng thử lại sau.";
                    return View(service); // Display the form with the filled data
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(service); // Display the form with the filled data
            }
        }



    }
}



