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
        private string ApiUrlRoomCategoryList = "";
        private string ApiUrlRoomCategoryDetail = "";
        private string ApiUrlRoomCategoryAdd = "";
        private string ApiUrlRoomCategoryUpdate = "";

        public RoomCategoryController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "";
            ApiUrlRoomCategoryList = "https://localhost:7255/api/RoomCategory/GetAllRoomCategory";
            ApiUrlRoomCategoryDetail = "https://localhost:7255/api/RoomCategory/GetRoomCategory";
            ApiUrlRoomCategoryAdd = "https://localhost:7255/api/RoomCategory/AddRoomCategory";
            ApiUrlRoomCategoryUpdate = "https://localhost:7255/api/RoomCategory/UpdateRoomCategory?roomCategoryId=";

        }

        public async Task<ActionResult> Index(RoomCategoryDTO roomCategoryDTO)
        {
            try
            {
                var json = JsonConvert.SerializeObject(roomCategoryDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(ApiUrlRoomCategoryList);
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

        public async Task<ActionResult> AddRoomCategory([FromForm] RoomCategoryDTO roomCategoryDTO, IFormFile image)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (image != null && image.Length > 0)
                    {
                        Console.WriteLine(image);
                        roomCategoryDTO.Picture = "/img/" + image.FileName.ToString();
                    }

                    roomCategoryDTO.Status = true;

                    var json = JsonConvert.SerializeObject(roomCategoryDTO);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(ApiUrlRoomCategoryAdd, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Thêm loại phòng thành công!";
                        return View(roomCategoryDTO);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Thêm loại phòng thất bại. Vui lòng thử lại sau!";
                        return View(roomCategoryDTO);
                    }
                }
                else
                {
                    return View(roomCategoryDTO);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return View(roomCategoryDTO);
            }
        }


        /*
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
