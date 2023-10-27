﻿using FEPetServices.Areas.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;
using System.Diagnostics;
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
            ApiUrlRoomCategoryDetail = "https://localhost:7255/api/RoomCategory/GetRoomCategory/";
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
                        TempData["ErrorMessage"] = "Thêm loại phòng thất bại. Vui lòng thử lại sau!";
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


        
        [HttpGet]
        public async Task<ActionResult> EditRoomCategory(int roomCategoryId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(ApiUrlRoomCategoryDetail + roomCategoryId);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var existingRoomCategory = JsonConvert.DeserializeObject<RoomCategoryDTO>(responseContent);

                        return View(existingRoomCategory);
                    }
                    else
                    {
                        TempData["ErrorToast"] = "API trả về dữ liệu rỗng.";
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

            return View();
        }


        [HttpPost]
        public async Task<ActionResult> EditRoomCategory([FromForm] RoomCategoryDTO roomCategoryDTO, IFormFile image, int roomCategoryId)
        {
            try
            {
                if (image != null && image.Length > 0)
                {
                    Console.WriteLine(image);
                    var imagePath = "/img/" + image.FileName;
                    roomCategoryDTO.Picture = imagePath;

                    var physicalImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", image.FileName);
                    using (var stream = new FileStream(physicalImagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                }
                else
                {
                    HttpResponseMessage responseForImage = await client.GetAsync(ApiUrlRoomCategoryDetail + roomCategoryId);

                    if (responseForImage.IsSuccessStatusCode)
                    {
                        var responseContent = await responseForImage.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var existingRoomCategory = JsonConvert.DeserializeObject<RoomCategoryDTO>(responseContent);

                            Console.WriteLine(existingRoomCategory);
                            if (existingRoomCategory != null)
                            {
                                roomCategoryDTO.Picture = existingRoomCategory.Picture;
                            }
                        }
                    }
                }
                if (Request.Form["Status"] == "on")
                {
                    roomCategoryDTO.Status = true;
                }
                else
                {
                    roomCategoryDTO.Status = false;
                }

                var json = JsonConvert.SerializeObject(roomCategoryDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(ApiUrlRoomCategoryUpdate + roomCategoryId, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Chỉnh sửa dịch vụ thành công!";
                    return View(roomCategoryDTO); 
                }
                else
                {
                    TempData["ErrorToast"] = "Chỉnh sửa dịch vụ thất bại. Vui lòng thử lại sau.";
                    return View(roomCategoryDTO);
                }
            }

            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(roomCategoryDTO);
            }
        }

    }
}
