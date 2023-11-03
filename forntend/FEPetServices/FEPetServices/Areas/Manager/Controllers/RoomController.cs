using FEPetServices.Areas.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PetServices.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;

namespace FEPetServices.Areas.Manager.Controllers
{
    [Authorize(Policy = "ManaOnly")]
    public class RoomController : Controller
    {
        private readonly HttpClient client = null;
        private string ApiUrlRoomList;
        private string ApiUrlRoomAdd;
        private string ApiUrlRoomCategoryList;
        private string ApiUrlRoomDetail;
        private string ApiUrlRoomUpdate;
        private string ApiUrlServiceList;

        public RoomController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);

            ApiUrlRoomList = "https://localhost:7255/api/Room/GetAllRoom";
            ApiUrlRoomAdd = "https://localhost:7255/api/Room/AddRoom";
            ApiUrlRoomCategoryList = "https://localhost:7255/api/Room/GetRoomCategory";
            ApiUrlRoomDetail = "https://localhost:7255/api/Room/GetRoom/";
            ApiUrlRoomUpdate = "https://localhost:7255/api/Room/UpdateRoom?roomId=";
            ApiUrlServiceList = "https://localhost:7255/api/Room/GetAllService";
        }

        public async Task<ActionResult> Index(RoomDTO roomDTO)
        {
            try
            {
                var json = JsonConvert.SerializeObject(roomDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(ApiUrlRoomList);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                        var roomList = JsonConvert.DeserializeObject<List<RoomDTO>>(responseContent);
                        return View(roomList);
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

        public async Task<IActionResult> AddRoom([FromForm] RoomDTO roomDTO, IFormFile image)
        {
            try
            {
                HttpResponseMessage roomCategoryResponse = await client.GetAsync(ApiUrlRoomCategoryList);

                if (roomCategoryResponse.IsSuccessStatusCode)
                {
                    var categories = await roomCategoryResponse.Content.ReadFromJsonAsync<List<RoomCategoryDTO>>();
                    ViewBag.Categories = new SelectList(categories, "RoomCategoriesId", "RoomCategoriesName");
                }

                HttpResponseMessage serviceResponse = await client.GetAsync(ApiUrlServiceList);

                if (serviceResponse.IsSuccessStatusCode)
                {
                    var services = await serviceResponse.Content.ReadFromJsonAsync<List<ServiceDTO>>();
                    Console.WriteLine(services);

                    ViewBag.services = new SelectList(services, "ServiceId", "ServiceName");
                }

                if (image != null && image.Length > 0)
                {
                    string filename = GenerateRandomNumber(5) + image.FileName;
                    filename = Path.GetFileName(filename);
                    string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/", filename);
                    var stream = new FileStream(uploadfile, FileMode.Create);
                    image.CopyToAsync(stream);
                    roomDTO.Picture = "/img/" + filename;
                }
                else
                {
                    return View(roomDTO); 
                }
                roomDTO.Status = true;

                roomDTO.ServiceIds = Request.Form["SelectedServices"].ToString().Split(',').Select(int.Parse).ToList();

                var json = JsonConvert.SerializeObject(roomDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(ApiUrlRoomAdd, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Thêm phòng thành công!";
                    return View();
                }
                else
                {
                    TempData["ErrorToast"] = "Thêm phòng thất bại. Vui lòng thử lại sau.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(roomDTO);
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
        public async Task<ActionResult> EditRoom(int RoomId)
        {
            try
            {
                HttpResponseMessage serviceResponse = await client.GetAsync(ApiUrlServiceList);

                if (serviceResponse.IsSuccessStatusCode)
                {
                    var services = await serviceResponse.Content.ReadFromJsonAsync<List<ServiceDTO>>();

                    ViewBag.services = new SelectList(services, "ServiceId", "ServiceName");
                }

                HttpResponseMessage categoryResponse = await client.GetAsync(ApiUrlRoomCategoryList);

                if (!categoryResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorToast"] = "Tải danh sách loại phòng thất bại. Vui lòng tải lại trang.";
                    return View();
                }

                var categories = await categoryResponse.Content.ReadFromJsonAsync<List<RoomCategoryDTO>>();
                ViewBag.Categories = new SelectList(categories, "RoomCategoriesId", "RoomCategoriesName");

                HttpResponseMessage response = await client.GetAsync(ApiUrlRoomDetail + RoomId);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorToast"] = "Tải thông tin phòng thất bại. Vui lòng tải lại trang.";
                    return View();
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var roomDto = JsonConvert.DeserializeObject<RoomDTO>(responseContent);

                return View(roomDto);

            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> EditRoom([FromForm] RoomDTO roomDTO, IFormFile image, int RoomId, int SelectedCategory, List<int> selectedServices)
        {
            try
            {
                HttpResponseMessage categoryResponse = await client.GetAsync(ApiUrlRoomCategoryList);

                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categories = await categoryResponse.Content.ReadFromJsonAsync<List<RoomCategoryDTO>>();
                    ViewBag.Categories = new SelectList(categories, "RoomCategoriesId", "RoomCategoriesName");
                }

                roomDTO.ServiceIds = Request.Form["SelectedServices"].ToString().Split(',').Select(int.Parse).ToList();

                if (image != null && image.Length > 0)
                {
                    var imagePath = "/img/" + image.FileName;
                    roomDTO.Picture = imagePath;

                    var physicalImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", image.FileName);
                    using (var stream = new FileStream(physicalImagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                }
                else
                {
                    HttpResponseMessage responseForImage = await client.GetAsync(ApiUrlRoomDetail + RoomId);

                    if (responseForImage.IsSuccessStatusCode)
                    {
                        var responseContent = await responseForImage.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var existingRoom = JsonConvert.DeserializeObject<RoomDTO>(responseContent);
                            if (existingRoom != null)
                            {
                                roomDTO.Picture = existingRoom.Picture;
                            }
                        }
                    }
                }

                if (Request.Form["Status"] == "on")
                {
                    roomDTO.Status = true;
                }
                else
                {
                    roomDTO.Status = false;
                }

                var json = JsonConvert.SerializeObject(roomDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(ApiUrlRoomUpdate + RoomId, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Chỉnh sửa dịch vụ thành công!";
                    return RedirectToAction("EditRoom", new { roomId = RoomId });
                }
                else
                {
                    TempData["ErrorToast"] = "Chỉnh sửa dịch vụ thất bại. Vui lòng thử lại sau.";
                    return View(roomDTO);
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(roomDTO);
            }
        }

    }
}
