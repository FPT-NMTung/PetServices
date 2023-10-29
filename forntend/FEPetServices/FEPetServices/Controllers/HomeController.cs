using FEPetServices.Areas.DTO;
using FEPetServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PetServices.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace FEPetServices.Controllers
{
    /*[Authorize(Policy = "CusOnly")]*/
    public class HomeController : Controller
    {
        private readonly HttpClient client = null;
        private string ApiUrlRoomList;
        private string ApiUrlRoomCategoryList;
        private string ApiUrlRoomDetail;

        public HomeController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);

            ApiUrlRoomList = "https://localhost:7255/api/Room/GetAllRoom";
            ApiUrlRoomCategoryList = "https://localhost:7255/api/Room/GetRoomCategory";
            ApiUrlRoomDetail = "https://localhost:7255/api/Room/GetRoom/";
        }

        public async Task<ActionResult> Room(RoomDTO roomDTO, int page = 1, int pageSize = 8, string RoomName = "", string sortOrder = "", string RoomCategoriesId = "")
        {
            try
            {
                var json = JsonConvert.SerializeObject(roomDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(ApiUrlRoomList);
                if (response.IsSuccessStatusCode)
                {
                    HttpResponseMessage roomCategoryResponse = await client.GetAsync(ApiUrlRoomCategoryList);

                    if (roomCategoryResponse.IsSuccessStatusCode)
                    {
                        var categories = await roomCategoryResponse.Content.ReadFromJsonAsync<List<RoomCategoryDTO>>();
                        ViewBag.Categories = new SelectList(categories, "RoomCategoriesId", "RoomCategoriesName");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var roomList = JsonConvert.DeserializeObject<List<RoomDTO>>(responseContent);

                        if (!string.IsNullOrEmpty(RoomName))
                        {
                            roomList = roomList.Where(r => r.RoomName.Contains(RoomName, StringComparison.OrdinalIgnoreCase)).ToList();
                        }

                        if (!string.IsNullOrEmpty(RoomCategoriesId))
                        {
                            int roomCategoriesId = int.Parse(RoomCategoriesId);
                            roomList = roomList.Where(r => r.RoomCategoriesId == roomCategoriesId).ToList();
                        }

                        ViewBag.RoomCategoriesId = RoomCategoriesId;
                        ViewBag.SortOrder = sortOrder;
                        ViewBag.PageSize = pageSize;
                        ViewBag.RoomName = RoomName;

                        switch (sortOrder)
                        {
                            case "name_desc":
                                roomList = roomList.OrderByDescending(r => r.RoomName).ToList();
                                break;
                            case "price":
                                roomList = roomList.OrderBy(r => r.Price).ToList();
                                break;
                            case "price_desc":
                                roomList = roomList.OrderByDescending(r => r.Price).ToList();
                                break;
                            default:
                                roomList = roomList.OrderBy(r => r.RoomName).ToList();
                                break;
                        }

                        int totalItems = roomList.Count;
                        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                        int startIndex = (page - 1) * pageSize;
                        List<RoomDTO> currentPageRoomList = roomList.Skip(startIndex).Take(pageSize).ToList();

                        ViewBag.TotalPages = totalPages;
                        ViewBag.CurrentPage = page;
                        ViewBag.PageSize = pageSize;


                        return View(currentPageRoomList);
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

        public async Task<ActionResult> RoomDetail(int roomId)
        {
            try
            {
                HttpResponseMessage serviceAvailableResponse = await client.GetAsync("https://localhost:7255/api/Room/GetServiceInRoom?roomId=" + roomId);

                if (serviceAvailableResponse.IsSuccessStatusCode)
                {
                    var services = await serviceAvailableResponse.Content.ReadFromJsonAsync<List<ServiceDTO>>();

                    ViewBag.ServiceAvailable = new SelectList(services, "ServiceId", "ServiceName");
                }

                HttpResponseMessage serviceUnavailableResponse = await client.GetAsync("https://localhost:7255/api/Room/GetServiceOutRoom?roomId=" + roomId);

                if (serviceUnavailableResponse.IsSuccessStatusCode)
                {
                    var services = await serviceUnavailableResponse.Content.ReadFromJsonAsync<List<ServiceDTO>>();

                    ViewBag.ServiceUnavailable = new SelectList(services, "ServiceId", "ServiceName");
                }

                HttpResponseMessage response = await client.GetAsync(ApiUrlRoomDetail + roomId);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var roomDto = JsonConvert.DeserializeObject<RoomDTO>(responseContent);

                    return View(roomDto);
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}