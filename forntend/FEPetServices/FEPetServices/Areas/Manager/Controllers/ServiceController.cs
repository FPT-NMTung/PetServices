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
                            var existProduct = existPL[0];
                            HttpResponseMessage SerCateResponse = await client.GetAsync("https://localhost:7255/api/ServiceCategory/GetAllServiceCategory");
                            if (SerCateResponse.IsSuccessStatusCode)
                            {
                                var serCate = await SerCateResponse.Content.ReadAsStringAsync();
                                var serCategories = JsonConvert.DeserializeObject<List<ServiceCategoryDTO>>(serCate);
                                //var cateSelectList = new SelectList(proCategories, "ProCategoriesId", "ProCategoriesName", existProduct.ProCategoriesId);

                                ViewBag.Categories = new SelectList(serCategories, "SerCategoriesId", "SerCategoriesName", existProduct.SerCategoriesId);
                                return View(existProduct);
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


    }
}



      