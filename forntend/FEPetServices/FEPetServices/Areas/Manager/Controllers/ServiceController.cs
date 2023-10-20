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
                else
                {
                    // Xử lý lỗi nếu không có tệp ảnh được tải lên
                    /*ViewBag.ErrorMessage = "Vui lòng chọn một tệp ảnh.";*/
                    return View(service); // Hiển thị lại biểu mẫu với thông báo lỗi
                }
                service.Status = true;

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
        [HttpGet]
        public async Task<IActionResult> EditService(int ServiceId)
        {
            try
            {
                //goi api de lay thong tin can sua
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlServiceDetail + "/" + ServiceId);

                if (response.IsSuccessStatusCode)
                {
                    var rep = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(rep))
                    {
                        //deserialize du lieu tu api thanh ds cac doi tuongdto
                        var existPL = JsonConvert.DeserializeObject<List<ServiceDTO>>(rep);
                        if (existPL.Count > 0)
                        {
                            var existService = existPL[0];
                            HttpResponseMessage SerCateResponse = await client.GetAsync("https://localhost:7255/api/ServiceCategory/GetAllServiceCategory");
                            if (SerCateResponse.IsSuccessStatusCode)
                            {
                                var serCate = await SerCateResponse.Content.ReadAsStringAsync();
                                var serCategories = JsonConvert.DeserializeObject<List<ServiceCategoryDTO>>(serCate);
                                ViewBag.Categories = new SelectList(serCategories, "SerCategoriesId", "SerCategoriesName", existService.SerCategoriesId);
                                return View(existService);
                            }
                            else
                            {
                                ViewBag.ErrorMessage = "Tải danh sách loại sản phẩm thất bại";
                            }
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Không tìm thấy sản phẩm với ID được cung cấp.";
                        }
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
            return View();
        }
        /*[HttpPost]
        public async Task<IActionResult> EditService([FromForm] ServiceDTO service, IFormFile image, int ServiceId)
        {
            try
            {
                HttpResponseMessage categoryResponse = await client.GetAsync("https://localhost:7255/api/ServiceCategory/GetAllServiceCategory");

                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categories = await categoryResponse.Content.ReadFromJsonAsync<List<ServiceCategoryDTO>>();
                    var categoriesSelectList = new SelectList(categories, "SerCategoriesId", "SerCategoriesName");
                    ViewBag.Categories = categoriesSelectList;

                    if (service.SerCategoriesId != null)
                    {
                        // Set the selected category based on the value from the model.
                        categoriesSelectList = new SelectList(categories, "SerCategoriesId", "SerCategoriesName", service.SerCategoriesId);
                        ViewBag.Categories = categoriesSelectList;
                    }
                }


                if (image != null && image.Length > 0)
                {
                    // Cập nhật đường dẫn ảnh nếu có tải lên ảnh mới.
                    Console.WriteLine(image);
                    var imagePath = "/img/" + image.FileName;
                    service.Picture = imagePath;

                    // Lưu tệp ảnh vào thư mục trên máy chủ
                    var physicalImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", image.FileName);
                    using (var stream = new FileStream(physicalImagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                }
                else
                {
                    // If no new image is uploaded, retain the existing image path
                    HttpResponseMessage responseForImage = await client.GetAsync(DefaultApiUrlServiceDetail + "/" + ServiceId);

                    if (responseForImage.IsSuccessStatusCode)
                    {
                        var responseContent = await responseForImage.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var existingServiceCategoryList = JsonConvert.DeserializeObject<List<ServiceDTO>>(responseContent);
                            var existingServiceCategory = existingServiceCategoryList.FirstOrDefault();
                            if (existingServiceCategory != null)
                            {
                                // Assign the existing image path to service.Picture.
                                service.Picture = existingServiceCategory.Picture;
                            }
                        }
                    }
                }

                if (Request.Form.ContainsKey("Status") && Request.Form["Status"] == "on")
                {
                    service.Status = true;
                }
                else
                {
                    service.Status = false;
                }

                var json = JsonConvert.SerializeObject(service);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi dữ liệu lên máy chủ
                HttpResponseMessage response = await client.PutAsync(DefaultApiUrlServiceUpdate + ServiceId, content);

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
        [HttpPost]
        public async Task<IActionResult> EditService([FromForm] ServiceDTO service, IFormFile image, int ServiceId)
        {
            try
            {
                if (ModelState.IsValid) // Check if the model state is valid
                {
                    HttpResponseMessage categoryResponse = await client.GetAsync("https://localhost:7255/api/ServiceCategory/GetAllServiceCategory");

                    if (categoryResponse.IsSuccessStatusCode)
                    {
                        var categories = await categoryResponse.Content.ReadFromJsonAsync<List<ServiceCategoryDTO>>();
                        var categoriesSelectList = new SelectList(categories, "SerCategoriesId", "SerCategoriesName");
                        ViewBag.Categories = categoriesSelectList;

                        if (service.SerCategoriesId != null)
                        {
                            // Set the selected category based on the value from the model.
                            categoriesSelectList = new SelectList(categories, "SerCategoriesId", "SerCategoriesName", service.SerCategoriesId);
                            ViewBag.Categories = categoriesSelectList;
                        }
                    }

                    if (image != null && image.Length > 0)
                    {
                        // Rest of your code for handling the image
                    }

                    // Rest of your code for handling other fields and saving to the database
                }
                else
                {
                    // Handle the case when ModelState is not valid, possibly by returning the view with validation errors.
                    // For example, you can return the view with the current model and display validation errors.
                    return View(service);
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



      