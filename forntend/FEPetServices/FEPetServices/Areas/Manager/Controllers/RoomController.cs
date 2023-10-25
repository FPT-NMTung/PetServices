using FEPetServices.Areas.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PetServices.Models;
using System.Net.Http.Headers;
using System.Text;

namespace FEPetServices.Areas.Manager.Controllers
{
    public class RoomController : Controller
    {
        private readonly HttpClient client = null;
        private string ApiUrlRoomList;
        private string ApiUrlRoomAdd;
        private string ApiUrlRoomCategoryList;
        private string ApiUrlRoomDetail;
        private string ApiUrlRoomUpdate;

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
                        var roomList = JsonConvert.DeserializeObject<List<RoomDTO>>(responseContent);
                        return View(roomList);
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

                if (image != null && image.Length > 0)
                {
                    roomDTO.Picture = "/img/" + image.FileName.ToString();
                }
                else
                {
                    return View(roomDTO); 
                }
                roomDTO.Status = true;

                var json = JsonConvert.SerializeObject(roomDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(ApiUrlRoomAdd, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Thêm phòng thành công!";
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = "Thêm phòng thất bại. Vui lòng thử lại sau.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(roomDTO);
            }
        }


    }
}
