using FEPetServices.Areas.DTO;
using FEPetServices.Form;
using FEPetServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using PetServices.Models;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
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
        private string DefaultApiUrlServiceCategoryList = "";
        private string DefaultApiUrlServiceCategoryDetail = "";
        private string DefaultApiUrlServiceCategoryandService = "";
        private string DefaultApiUrlBlogList = "";
        private string DefaultApiUrlProductList = "";
        private string DefaultApiUrlRoomCategoryList = "";
        private string DefaultApiUrlProductCategoryList = "";

        public HomeController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);

            ApiUrlRoomList = "https://localhost:7255/api/Room/GetAllRoomCustomer";
            ApiUrlRoomCategoryList = "https://localhost:7255/api/Room/GetRoomCategory";
            ApiUrlRoomDetail = "https://localhost:7255/api/Room/GetRoom/";
            DefaultApiUrlServiceCategoryList = "https://localhost:7255/api/ServiceCategory";
            DefaultApiUrlProductCategoryList = "https://localhost:7255/api/ServiceCategory";
            DefaultApiUrlServiceCategoryDetail = "https://localhost:7255/api/ServiceCategory/ServiceCategorysID/";
            DefaultApiUrlBlogList = "https://localhost:7255/api/Blog";
            DefaultApiUrlProductList = "https://localhost:7255/api/Product";
            DefaultApiUrlRoomCategoryList = "https://localhost:7255/api/Room";
        }

        public async Task<ActionResult> Room(RoomDTO roomDTO, RoomSearchDTO searchDTO)
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

                    int page = searchDTO.page ?? 1; ;
                    int pagesize = searchDTO.pagesize ?? 6;

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var roomList = JsonConvert.DeserializeObject<List<RoomDTO>>(responseContent);

                        if (!string.IsNullOrEmpty(searchDTO.roomname))
                        {
                            roomList = roomList.Where(r => r.RoomName.Contains(searchDTO.roomname, StringComparison.OrdinalIgnoreCase)).ToList();
                        }

                        if (!string.IsNullOrEmpty(searchDTO.roomcategory))
                        {
                            int roomCategoriesId = int.Parse(searchDTO.roomcategory);
                            roomList = roomList.Where(r => r.RoomCategoriesId == roomCategoriesId).ToList();
                        }

                        if (!string.IsNullOrEmpty(searchDTO.pricefrom) || !string.IsNullOrEmpty(searchDTO.priceto))
                        {
                            if (string.IsNullOrEmpty(searchDTO.pricefrom))
                            {
                                int priceTo = int.Parse(searchDTO.priceto);
                                roomList = roomList.Where(r => r.Price < priceTo).ToList();
                            }
                            if (string.IsNullOrEmpty(searchDTO.priceto))
                            {
                                int priceFrom = int.Parse(searchDTO.pricefrom);
                                roomList = roomList.Where(r => r.Price > priceFrom).ToList();
                            }
                            if (!string.IsNullOrEmpty(searchDTO.pricefrom) && !string.IsNullOrEmpty(searchDTO.priceto))
                            {
                                int PriceTo = int.Parse(searchDTO.priceto);
                                int PriceFrom = int.Parse(searchDTO.pricefrom);

                                roomList = roomList.Where(r => r.Price > PriceFrom && r.Price < PriceTo).ToList();
                            }
                        }

                        switch (searchDTO.sortby)
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
                        int totalPages = (int)Math.Ceiling(totalItems / (double)pagesize);
                        int startIndex = (page - 1) * pagesize;
                        List<RoomDTO> currentPageRoomList = roomList.Skip(startIndex).Take(pagesize).ToList();

                        ViewBag.TotalPages = totalPages;
                        ViewBag.CurrentPage = searchDTO.page;
                        ViewBag.PageSize = searchDTO.pagesize;

                        ViewBag.roomcategory = searchDTO.roomcategory;
                        ViewBag.pricefrom = searchDTO.pricefrom;
                        ViewBag.priceto = searchDTO.priceto;
                        ViewBag.sortby = searchDTO.sortby;
                        ViewBag.roomname = searchDTO.roomname;
                        ViewBag.pagesize = searchDTO.pagesize;
                        ViewBag.viewstyle = searchDTO.viewstyle;

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

                    ViewBag.ServiceUnavailable = services;
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
            DefaultApiUrlBlogList = "https://localhost:7255/api/Blog";


        }

        public async Task<IActionResult> ServiceList(ServiceCategoryDTO serviceCategory, int page = 1, int pagesize = 6, string CategoriesName = "" , string viewstyle = "grid", string sortby = "")
        {
            try
            {
                var json = JsonConvert.SerializeObject(serviceCategory);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlServiceCategoryList + "/GetAllServiceCategory");
                

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var servicecategoryList = JsonConvert.DeserializeObject<List<ServiceCategoryDTO>>(responseContent);

             

                        if (!string.IsNullOrEmpty(CategoriesName) && servicecategoryList != null)
                        {
                            servicecategoryList = servicecategoryList
                                .Where(c => c.SerCategoriesName != null && c.SerCategoriesName.Contains(CategoriesName, StringComparison.OrdinalIgnoreCase))
                                .ToList();
                            Console.WriteLine(1);
                        }

                        switch (sortby)
                        {
                            case "name_desc":
                                servicecategoryList = servicecategoryList.OrderByDescending(r => r.SerCategoriesName).ToList();
                                break;
                            default:
                                servicecategoryList = servicecategoryList.OrderBy(r => r.SerCategoriesName).ToList();
                                break;
                        }


                        int totalItems = servicecategoryList.Count;
                        int totalPages = (int)Math.Ceiling(totalItems / (double)pagesize);
                        int startIndex = (page - 1) * pagesize;
                        List<ServiceCategoryDTO> currentPageServicecategoryList = servicecategoryList.Skip(startIndex).Take(pagesize).ToList();

                        ViewBag.TotalPages = totalPages;
                        ViewBag.CurrentPage = page;
                        ViewBag.PageSize = pagesize;

                        ViewBag.CategoriesName =CategoriesName ;
                        ViewBag.sortby = sortby;
                        ViewBag.pagesize = pagesize;
                        ViewBag.viewstyle = viewstyle;

                        return View(currentPageServicecategoryList);
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

        public class HomeModel
        {
            public List<ServiceCategoryDTO> ListServiceCategory { get; set; }
            public List<ProductDTO> ListProduct { get; set; }
            public List<RoomCategoryDTO> ListRoomCategory { get; set; }
        }

        public async Task<IActionResult> Index()
        {
            HomeModel homeModel = new HomeModel();
            try
            {
                HttpResponseMessage responseProduct = await client.GetAsync(DefaultApiUrlProductList + "/GetAll");
                if (responseProduct.IsSuccessStatusCode)
                {
                    HttpResponseMessage responseCategory = await client.GetAsync(DefaultApiUrlServiceCategoryList + "/GetAllServiceCategory");
                    if (responseCategory.IsSuccessStatusCode)
                    {
                        HttpResponseMessage responseRoomCategory = await client.GetAsync(DefaultApiUrlRoomCategoryList + "/GetAllRoomCustomer");
                        if (responseCategory.IsSuccessStatusCode)
                        {
                            var responseRoomCategoryContent = await responseRoomCategory.Content.ReadAsStringAsync();

                            if (!string.IsNullOrEmpty(responseRoomCategoryContent))
                            {
                                homeModel.ListRoomCategory = JsonConvert.DeserializeObject<List<RoomCategoryDTO>>(responseRoomCategoryContent);
                            }
                        }
                        var responseCategoryContent = await responseCategory.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseCategoryContent))
                        {
                            homeModel.ListServiceCategory = JsonConvert.DeserializeObject<List<ServiceCategoryDTO>>(responseCategoryContent);
                        }
                    }
                    var rep = await responseProduct.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(rep))
                    {
                        homeModel.ListProduct = JsonConvert.DeserializeObject<List<ProductDTO>>(rep);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "API trả về dữ liệu rỗng";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Tải dữ liệu lên thất bại. Vui lòng tải lại trang!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View(homeModel);
        }
        
        public async Task<IActionResult> ServiceDetail(int serviceCategoryId, int serviceIds)
        {
            ServiceDetailModel model = new ServiceDetailModel();

            HttpResponseMessage response = await client.GetAsync("https://localhost:7255/api/ServiceCategory/ServiceCategorysID/" + serviceCategoryId);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseContent))
                {
                    ServiceCategoryDTO servicecategory = JsonConvert.DeserializeObject<ServiceCategoryDTO>(responseContent);
                    int serviceId = 0;

                    foreach (var sr in servicecategory.Services)
                    {
                        serviceId = sr.ServiceId;
                        break;
                    }

                    if (serviceIds == 0)
                    {
                        HttpResponseMessage responseService = await client.GetAsync("https://localhost:7255/api/ServiceCategory/GetServiceByServiceCategoryAndServiceID/"
                        + "?serviceCategoryId=" + serviceCategoryId + "&serviceId=" + serviceId);

                        var responseContentService = await responseService.Content.ReadAsStringAsync();
                        var service = JsonConvert.DeserializeObject<ServiceDTO>(responseContentService);
                        model.Service = service;
                        model.ServiceCategory = servicecategory;

                        return View(model);
                    }
                    else
                    {
                        HttpResponseMessage responseService = await client.GetAsync("https://localhost:7255/api/ServiceCategory/GetServiceByServiceCategoryAndServiceID/"
                     + "?serviceCategoryId=" + serviceCategoryId + "&serviceId=" + serviceIds);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContentService = await responseService.Content.ReadAsStringAsync();
                            var service = JsonConvert.DeserializeObject<ServiceDTO>(responseContentService);
                            model.Service = service;
                            model.ServiceCategory = servicecategory;

                            return View(model);
                        }
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Tải dữ liệu lên thất bại. Vui lòng tải lại trang.";
                }
            }

            return View();
        }

        public class ServiceDetailModel
        {
            public ServiceDTO Service { get; set; }
            public ServiceCategoryDTO ServiceCategory { get; set; }
        }

        public async Task<IActionResult> BlogList(BlogDTO blog, int page = 1, int pagesize = 6, string BlogName = "", string sortby = "")
        {
            try
            {
                var json = JsonConvert.SerializeObject(blog);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlBlogList + "/GetAllBlog");


                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var blogList = JsonConvert.DeserializeObject<List<BlogDTO>>(responseContent);



                        if (!string.IsNullOrEmpty(BlogName) && blogList != null)
                        {
                            blogList = blogList
                                .Where(c => c.PageTile != null && c.PageTile.Contains(BlogName, StringComparison.OrdinalIgnoreCase))
                                .ToList();
                            Console.WriteLine(1);
                        }

                        switch (sortby)
                        {
                            case "name_desc":
                                blogList = blogList.OrderByDescending(r => r.PageTile).ToList();
                                break;
                            default:
                                blogList = blogList.OrderBy(r => r.PageTile).ToList();
                                break;
                        }


                        int totalItems = blogList.Count;
                        int totalPages = (int)Math.Ceiling(totalItems / (double)pagesize);
                        int startIndex = (page - 1) * pagesize;
                        List<BlogDTO> currentPageBlogList = blogList.Skip(startIndex).Take(pagesize).ToList();

                        ViewBag.TotalPages = totalPages;
                        ViewBag.CurrentPage = page;
                        ViewBag.PageSize = pagesize;

                        ViewBag.BlogName = BlogName;
                        ViewBag.sortby = sortby;
                        ViewBag.pagesize = pagesize;

                        return View(currentPageBlogList);
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
        //public IActionResult Index()
        //{
        //    return View();
        //}



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