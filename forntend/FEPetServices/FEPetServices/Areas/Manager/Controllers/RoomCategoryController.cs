using FEPetServices.Areas.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;
using System.Net.Http.Headers;
using System.Text;

namespace FEPetServices.Areas.Manager.Controllers
{
    public class RoomCategoryController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlRoomCategoryList = "";
        private string DefaultApiUrlRoomCategoryDetail = "";
        private string DefaultApiUrlRoomCategoryAdd = "";
        private string DefaultApiUrlRoomCategoryUpdate = "";

        public RoomCategoryController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "";
            DefaultApiUrlRoomCategoryList = "https://localhost:7255/api/RoomCategory/GetAllRoomCategory";
            DefaultApiUrlRoomCategoryDetail = "https://localhost:7255/api/RoomCategory/GetRoomCategory";
            DefaultApiUrlRoomCategoryAdd = "https://localhost:7255/api/RoomCategory/AddRoomCategory";
            DefaultApiUrlRoomCategoryUpdate = "https://localhost:7255/api/RoomCategory/UpdateRoomCategory?roomCategoryId=";

        }

        public async Task<ActionResult> Index(RoomCategoryDTO roomCategoryDTO)
        {
            try
            {
                var json = JsonConvert.SerializeObject(roomCategoryDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlRoomCategoryList);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var roomCategoryList = JsonConvert.DeserializeObject<List<RoomCategoryDTO>>(responseContent);
                        return View(roomCategoryList);
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

        /*public async Task<ActionResult> AddServiceCategory([FromForm] ServiceCategoryDTO serviceCategory, IFormFile image)
        {
            try
            {
                if (ModelState.IsValid) // Kiểm tra xem biểu mẫu có hợp lệ không
                {
                    if (image != null && image.Length > 0)
                    {
                        // Xử lý và lưu trữ ảnh
                        Console.WriteLine(image);
                        serviceCategory.Picture = "/img/" + image.FileName.ToString();
                    }

                    var json = JsonConvert.SerializeObject(serviceCategory);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Gửi dữ liệu lên máy chủ
                    HttpResponseMessage response = await client.PostAsync(DefaultApiUrlServiceCategoryAdd, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Thêm dịch vụ thành công!";
                        return View(serviceCategory); // Chuyển hướng đến trang thành công hoặc trang danh sách
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Thêm dịch vụ thất bại. Vui lòng thử lại sau.";
                        return View(serviceCategory); // Hiển thị lại biểu mẫu với dữ liệu đã điền
                    }
                }
                else
                {
                    return View(serviceCategory);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return View(serviceCategory); // Hiển thị lại biểu mẫu với dữ liệu đã điền
            }
        }



        [HttpGet]
        public async Task<IActionResult> EditServiceCategory(int serCategoriesId)
        {
            try
            {
                // Gọi API để lấy thông tin ServiceCategory cần chỉnh sửa
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlServiceCategoryDetail + "/" + serCategoriesId);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        // Deserialize dữ liệu từ API thành danh sách các đối tượng ServiceCategoryDTO
                        var existingServiceCategoryList = JsonConvert.DeserializeObject<List<ServiceCategoryDTO>>(responseContent);

                        // Lấy đối tượng cần chỉnh sửa từ danh sách (có thể là phần tử đầu tiên)
                        var existingServiceCategory = existingServiceCategoryList.FirstOrDefault();

                        return View(existingServiceCategory);
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


        [HttpPost]
        public async Task<IActionResult> EditServiceCategory([FromForm] ServiceCategoryDTO serviceCategory, IFormFile image, int serCategoriesId)
        {
            try
            {

                if (image != null && image.Length > 0)
                {
                    // Xử lý và lưu trữ ảnh
                    Console.WriteLine(image);
                    serviceCategory.Picture = "/img/" + image.FileName.ToString();
                }

                var json = JsonConvert.SerializeObject(serviceCategory);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi dữ liệu lên máy chủ
                HttpResponseMessage response = await client.PutAsync(DefaultApiUrlServiceCategoryUpdate + serCategoriesId, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Thêm dịch vụ thành công!";
                    return View(serviceCategory);

                }
                else
                {
                    ViewBag.ErrorMessage = "Thêm dịch vụ thất bại. Vui lòng thử lại sau.";
                    return RedirectToAction("EditServiceCategory");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return RedirectToAction("EditServiceCategory");
            }
        }*/
    }
}
