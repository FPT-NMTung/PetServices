using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "ManaOnly")]
    public class ServiceCategoryController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;
        /*private string DefaultApiUrlServiceCategoryList = "";
        private string DefaultApiUrlServiceCategoryDetail = "";
        private string DefaultApiUrlServiceCategoryAdd = "";
        private string DefaultApiUrlServiceCategoryUpdate = "";*/

        public ServiceCategoryController(IConfiguration configuration)
        {
            client = new HttpClient();
            this.configuration = configuration;
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");

            //DefaultApiUrlServiceCategoryList = "https://pet-service-api.azurewebsites.net/api/ServiceCategory";
            //DefaultApiUrlServiceCategoryDetail = "https://pet-service-api.azurewebsites.net/api/ServiceCategory/ServiceCategorysID";
            //DefaultApiUrlServiceCategoryAdd = "https://pet-service-api.azurewebsites.net/api/ServiceCategory/AddServiceCategory";
            //DefaultApiUrlServiceCategoryUpdate = "https://pet-service-api.azurewebsites.net/api/ServiceCategory/EditServiceCategory?serCategoriesId=";

        }

        public async Task<IActionResult> Index(ServiceCategoryDTO serviceCategory)
        {
            try
            {
                ViewBag.Title = "Danh sách loại dịch vụ";
                var json = JsonConvert.SerializeObject(serviceCategory);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "ServiceCategory/GetAllServiceCategory");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var servicecategoryList = JsonConvert.DeserializeObject<List<ServiceCategoryDTO>>(responseContent);
                        //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                        return View(servicecategoryList);
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


            return View();
        }

        public async Task<IActionResult> AddServiceCategory([FromForm] ServiceCategoryDTO serviceCategory, List<IFormFile> image)
        {
            try
            {
                ViewBag.Title = "Thêm loại dịch vụ mới";
                if (ModelState.IsValid) // Kiểm tra xem biểu mẫu có hợp lệ không
                {
                    if (serviceCategory.SerCategoriesName == null) { return View(); }
                    foreach (var file in image)
                    {
                        string filename = GenerateRandomNumber(5) + file.FileName;
                        filename = Path.GetFileName(filename);
                        string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/ServiceCategory/", filename);
                        var stream = new FileStream(uploadfile, FileMode.Create);
                        file.CopyToAsync(stream);
                        serviceCategory.Picture = "/img/ServiceCategory/" + filename;
                    }

                    // mặc định status là true
                    serviceCategory.Status = true;

                    var json = JsonConvert.SerializeObject(serviceCategory);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(DefaultApiUrl + "ServiceCategory/AddServiceCategory", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessToast"] = "Thêm dịch vụ thành công!";
                        return View(serviceCategory);
                    }
                    else
                    {
                        TempData["ErrorToast"] = "Thêm dịch vụ thất bại. Vui lòng thử lại sau.";
                        return View(serviceCategory);
                    }
                }
                else
                {
                    return View(serviceCategory);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(serviceCategory);
            }
        }

        public static string GenerateRandomNumber(int length)
        {
            Random random = new Random();
            const string chars = "0123456789";
            char[] randomChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomChars);
        }

        [HttpGet]
        public async Task<IActionResult> EditServiceCategory(int serCategoriesId)
        {

            try
            {
                ViewBag.Title = "Chỉnh sửa chi tiết loại dịch vụ";
                // Gọi API để lấy thông tin ServiceCategory cần chỉnh sửa
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "ServiceCategory/ServiceCategorysID/" + serCategoriesId);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    ServiceCategoryDTO managerInfos = System.Text.Json.JsonSerializer.Deserialize<ServiceCategoryDTO>(responseContent, options);
                    HttpResponseMessage responseListService = await client.GetAsync(DefaultApiUrl + "Service/GetServicesByCategory/" + managerInfos.SerCategoriesId);

                    if (responseListService.IsSuccessStatusCode)
                    {
                        var ServiceList = await responseListService.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(ServiceList))
                        {
                            var service = JsonConvert.DeserializeObject<List<ServiceDTO>>(ServiceList);
                            ViewBag.service = service;
                        }

                    }

                    //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
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

            // Return the view with or without an error message
            return View();
        }
        public class ServiceModel
        {
            public ServiceCategoryDTO ServiceCategoryDTO { get; set; }
            public List<ServiceDTO> serviceDTO { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> EditServiceCategory([FromForm] ServiceCategoryDTO serviceCategory, IFormFile image, int serCategoriesId)
        {
            try
            {
                if (image != null && image.Length > 0)
                {
                    // An image file has been uploaded, so update the image path.
                    Console.WriteLine(image);
                    // Save the image to a location (e.g., a folder in your application)
                    var imagePath = "/img/ServiceCategory/" + image.FileName;
                    serviceCategory.Picture = imagePath;

                    // Save the image file to a folder on your server
                    var physicalImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "ServiceCategory", image.FileName);
                    using (var stream = new FileStream(physicalImagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                }
                else
                {

                    //HttpResponseMessage responseForImage = await client.GetAsync(DefaultApiUrl + "ServiceCategory/ServiceCategorysID/" + serCategoriesId);
                    HttpResponseMessage responseForImage = await client.GetAsync(DefaultApiUrl + "ServiceCategory/ServiceCategorysID/" + serCategoriesId);

                    if (responseForImage.IsSuccessStatusCode)
                    {
                        var responseContent = await responseForImage.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var existingServiceCategory = JsonConvert.DeserializeObject<ServiceCategoryDTO>(responseContent);

                            if (existingServiceCategory != null)
                            {
                                serviceCategory.Picture = existingServiceCategory.Picture;
                            }
                        }
                    }
                }
                if (Request.Form["Status"] == "on")
                {
                    serviceCategory.Status = true;
                }
                else
                {
                    serviceCategory.Status = false;
                }
                HttpResponseMessage responseService = await client.GetAsync(DefaultApiUrl + "Service/GetServicesByCategory/" + serCategoriesId);

                if (responseService.IsSuccessStatusCode)
                {
                    var ServiceList = await responseService.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(ServiceList))
                    {
                        var service = JsonConvert.DeserializeObject<List<ServiceDTO>>(ServiceList);
                        ViewBag.service = service;
                    }

                }
                HttpResponseMessage responseListService = await client.GetAsync(DefaultApiUrl + "Service/GetServicesByCategory/" + serCategoriesId);

                if (responseListService.IsSuccessStatusCode)
                {
                    var ServiceList = await responseListService.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(ServiceList))
                    {
                        var service = JsonConvert.DeserializeObject<List<ServiceDTO>>(ServiceList);
                        ViewBag.service = service;
                    }
                }

                var json = JsonConvert.SerializeObject(serviceCategory);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi dữ liệu lên máy chủ
                HttpResponseMessage response = await client.PutAsync(DefaultApiUrl + "ServiceCategory/EditServiceCategory?serCategoriesId=" + serCategoriesId, content);
                //HttpResponseMessage response = await client.PutAsync(DefaultApiUrlServiceCategoryUpdate + serCategoriesId, content);

                if (response.IsSuccessStatusCode)
                {

                    TempData["SuccessToast"] = "Chỉnh sửa dịch vụ thành công!";
                    return View(serviceCategory); // Chuyển hướng đến trang thành công hoặc trang danh sách
                }
                else
                {
                    TempData["ErrorToast"] = "Chỉnh sửa dịch vụ thất bại. Vui lòng thử lại sau.";
                    return View(serviceCategory); // Hiển thị lại biểu mẫu với dữ liệu đã điền
                }
            }

            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(serviceCategory); // Hiển thị lại biểu mẫu với dữ liệu đã điền
            }
        }

    }
}

