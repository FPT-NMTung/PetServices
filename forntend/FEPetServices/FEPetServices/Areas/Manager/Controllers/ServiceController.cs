using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PetServices.Models;
using System.Net.Http.Headers;
using System.Text;

namespace FEPetServices.Areas.Manager.Controllers
{
    public class ServiceController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlServiceList = "";
        private string DefaultApiUrlServiceAdd = "";
        private string DefaultApiUrlServiceCategoryList = "";
        private string DefaultApiUrlServiceDetail = "";
        private string DefaultApiUrlServiceUpdate = "";

        public ServiceController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "";
            DefaultApiUrlServiceList = "https://localhost:7255/api/Service/GetAllService";
            DefaultApiUrlServiceAdd = "https://localhost:7255/api/Service/CreateService";
            DefaultApiUrlServiceCategoryList = "https://localhost:7255/api/ServiceCategory/GetAllServiceCategory";
            DefaultApiUrlServiceDetail = "https://localhost:7255/api/Service/ServiceID";
            DefaultApiUrlServiceUpdate = "https://localhost:7255/api/Service/UpdateServices?serviceId=";

        }
        public async Task<IActionResult> Index(ServiceDTO serviceCategory)
        {
            try
            {
                var json = JsonConvert.SerializeObject(serviceCategory);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlServiceList);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var serviceList = JsonConvert.DeserializeObject<List<ServiceDTO>>(responseContent);
                        return View(serviceList);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "API trả về dữ liệu rỗng.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Tải dữ liệu lên thất bại. Vui lòng tải lại trang.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }

            // Return the view with or without an error message
            return View();
        }
      
        public async Task<IActionResult> AddService([FromForm] ServiceDTO service, IFormFile image)
        {
            try
            {
                HttpResponseMessage categoryResponse = await client.GetAsync("https://localhost:7255/api/ServiceCategory/GetAllServiceCategory");

                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categories = await categoryResponse.Content.ReadFromJsonAsync<List<ServiceCategoryDTO>>();
                    ViewBag.Categories = new SelectList(categories, "SerCategoriesId", "SerCategoriesName");
                }

                if (image != null && image.Length > 0)
                {
                    // Xử lý và lưu trữ ảnh
                    Console.WriteLine(image);
                    service.Picture = "/img/" + image.FileName.ToString();
                }


                var json = JsonConvert.SerializeObject(service);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi dữ liệu lên máy chủ
                HttpResponseMessage response = await client.PostAsync(DefaultApiUrlServiceAdd, content);

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

        /*[HttpGet]
        public async Task<IActionResult> EditService(int serviceId)
        {
            try
            {
                // Fetch the existing service based on serviceId
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlServiceDetail + "/" + serviceId);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var existingServiceList = JsonConvert.DeserializeObject<List<ServiceDTO>>(responseContent);

                        if (existingServiceList.Count > 0)
                        {
                            var existingService = existingServiceList[0];

                            // Fetch the list of service categories and set ViewBag data
                            HttpResponseMessage categoryResponse = await client.GetAsync(DefaultApiUrlServiceCategoryList);

                            if (categoryResponse.IsSuccessStatusCode)
                            {
                                var categoryResponseContent = await categoryResponse.Content.ReadAsStringAsync();
                                var serviceCategories = JsonConvert.DeserializeObject<List<ServiceCategoryDTO>>(categoryResponseContent);

                                // Create a SelectList of service categories
                                var categorySelectList = new SelectList(serviceCategories, "SerCategoriesId", "SerCategoriesName", existingService.SerCategoriesId);

                                // Pass the service and service categories to the view
                                ViewBag.Categories = categorySelectList;

                                return View(existingService);
                            }
                            else
                            {
                                TempData["ErrorToast"] = "Tải danh sách loại dịch vụ thất bại.";
                            }
                        }
                        else
                        {
                            TempData["ErrorToast"] = "Không tìm thấy dịch vụ với ID cung cấp.";
                        }
                    }
                    else
                    {
                        TempData["ErrorToast"] = "API trả về dữ liệu không phải là một mảng.";
                    }
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






        [HttpPost]
        public async Task<IActionResult> EditService([FromForm] ServiceDTO service, IFormFile image, int serviceId)
        {
            try
            {
                if (image != null && image.Length > 0)
                {
                    // An image file has been uploaded, so update the image path.
                    // Save the image to a location (e.g., a folder in your application)
                    var imagePath = "/img/" + image.FileName;
                    service.Picture = imagePath;

                    // Save the image file to a folder on your server
                    var physicalImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", image.FileName);
                    using (var stream = new FileStream(physicalImagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                }
                else
                {
                    // No image file has been uploaded, so do not update the image path.
                    // Retrieve the existing image path from the database and assign it to service.Picture.

                    HttpResponseMessage responseForImage = await client.GetAsync(DefaultApiUrlServiceDetail + "/" + serviceId);

                    if (responseForImage.IsSuccessStatusCode)
                    {
                        var responseContent = await responseForImage.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var existingServiceList = JsonConvert.DeserializeObject<List<ServiceDTO>>(responseContent);
                            var existingService = existingServiceList.FirstOrDefault();
                            if (existingService != null)
                            {
                                // Assign the existing image path to service.Picture.
                                service.Picture = existingService.Picture;
                            }
                        }
                    }
                }

                var json = JsonConvert.SerializeObject(service);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi dữ liệu lên máy chủ
                HttpResponseMessage response = await client.PutAsync(DefaultApiUrlServiceUpdate + serviceId, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Chỉnh sửa dịch vụ thành công!";
                    return View(service); // Chuyển hướng đến trang thành công hoặc trang danh sách
                }
                else
                {
                    TempData["ErrorToast"] = "Chỉnh sửa dịch vụ thất bại. Vui lòng thử lại sau.";
                    return View(service); // Hiển thị lại biểu mẫu với dữ liệu đã điền
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(service); // Hiển thị lại biểu mẫu với dữ liệu đã điền
            }
        }
*/
        [HttpGet]
        public async Task<IActionResult> EditService(int serviceId)
        {
            try
            {
                // Thực hiện gọi API để lấy thông tin dịch vụ dựa trên serviceId
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlServiceDetail + serviceId);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        // Kiểm tra nếu responseContent là một JSON array
                        if (responseContent.StartsWith("["))
                        {
                            // Phân tích cú pháp thành danh sách ServiceDTO
                            var serviceList = JsonConvert.DeserializeObject<List<ServiceDTO>>(responseContent);

                            // Trong trường hợp này, bạn nên chọn một dịch vụ cụ thể từ danh sách hoặc xử lý nó theo nhu cầu của bạn.
                            var selectedService = serviceList.FirstOrDefault(s => s.ServiceId == serviceId);

                            // Lấy danh sách loại dịch vụ để hiển thị trong dropdown
                            HttpResponseMessage categoryResponse = await client.GetAsync(DefaultApiUrlServiceCategoryList);
                            if (categoryResponse.IsSuccessStatusCode)
                            {
                                var categories = await categoryResponse.Content.ReadFromJsonAsync<List<ServiceCategoryDTO>>();
                                ViewBag.Categories = new SelectList(categories, "SerCategoriesId", "SerCategoriesName");
                            }

                            return View(selectedService);
                        }
                        else
                        {
                            TempData["ErrorToast"] = "API trả về dữ liệu không hợp lệ.";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["ErrorToast"] = "API trả về dữ liệu rỗng.";
                        return View();
                    }
                }
                else
                {
                    TempData["ErrorToast"] = "Tải dữ liệu dịch vụ thất bại. Vui lòng thử lại sau.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditService(ServiceDTO service, IFormFile image)
        {
            try
            {
                if (image != null && image.Length > 0)
                {
                    // Xử lý và lưu trữ ảnh
                    Console.WriteLine(image);
                    service.Picture = "/img/" + image.FileName.ToString();
                }

                var json = JsonConvert.SerializeObject(service);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi dữ liệu cập nhật lên máy chủ
                HttpResponseMessage response = await client.PostAsync(DefaultApiUrlServiceUpdate + service.ServiceId, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Cập nhật dịch vụ thành công!";
                    return RedirectToAction("Index"); // Chuyển hướng đến trang danh sách sau khi cập nhật
                }
                else
                {
                    TempData["ErrorToast"] = "Cập nhật dịch vụ thất bại. Vui lòng thử lại sau.";
                    return View(service); // Hiển thị lại biểu mẫu với dữ liệu đã điền
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(service); // Hiển thị lại biểu mẫu với dữ liệu đã điền
            }
        }


    }
}



      