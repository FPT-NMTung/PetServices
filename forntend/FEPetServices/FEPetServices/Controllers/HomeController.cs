﻿using FEPetServices.Areas.DTO;
using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PetServices.DTO;
using PetServices.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Controllers
{
    /*[Authorize(Policy = "CusOnly")]*/
    public class HomeController : Controller
    {
        private readonly HttpClient client = null;
        /*private string ApiUrlRoomList;
        private string ApiUrlRoomCategoryList;
        private string ApiUrlRoomDetail;
        private string DefaultApiUrlServiceCategoryList = "";
        private string DefaultApiUrlServiceCategoryDetail = "";
        private string DefaultApiUrlServiceCategoryandService = "";
        private string DefaultApiUrlBlogList = "";
        private string DefaultApiUrlBlogDetail = "";
        private string DefaultApiUrlProductList = "";
        private string DefaultApiUrlRoomCategoryList = "";
        private string DefaultApiUrlProductCategoryList = "";*/

        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);

            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");

            /*ApiUrlRoomList = "https://pet-service-api.azurewebsites.net/api/Room/GetAllRoomCustomer";
            ApiUrlRoomCategoryList = "https://pet-service-api.azurewebsites.net/api/Room/GetRoomCategory";
            ApiUrlRoomDetail = "https://pet-service-api.azurewebsites.net/api/Room/GetRoom/";
            DefaultApiUrlServiceCategoryList = "https://pet-service-api.azurewebsites.net/api/ServiceCategory";
            DefaultApiUrlProductCategoryList = "https://pet-service-api.azurewebsites.net/api/ServiceCategory";
            DefaultApiUrlServiceCategoryDetail = "https://pet-service-api.azurewebsites.net/api/ServiceCategory/ServiceCategorysID/";
            DefaultApiUrlBlogList = "https://localhost:7255/api/Blog";
            DefaultApiUrlProductList = "https://pet-service-api.azurewebsites.net/api/Product";
            DefaultApiUrlRoomCategoryList = "https://pet-service-api.azurewebsites.net/api/Room";
            DefaultApiUrlBlogDetail = "https://pet-service-api.azurewebsites.net/api/Blog/BlogID/";*/
        }

        public async Task<ActionResult> Room(RoomDTO roomDTO, RoomSearchDTO searchDTO)
        {
            try
            {
                var json = JsonConvert.SerializeObject(roomDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //HttpResponseMessage response = await client.GetAsync(ApiUrlRoomList);
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Room/GetAllRoomCustomer");
                if (response.IsSuccessStatusCode)
                {
                    //HttpResponseMessage roomCategoryResponse = await client.GetAsync(ApiUrlRoomCategoryList);
                    HttpResponseMessage roomCategoryResponse = await client.GetAsync(DefaultApiUrl + "Room/GetRoomCategory");

                    if (roomCategoryResponse.IsSuccessStatusCode)
                    {
                        var categories = await roomCategoryResponse.Content.ReadFromJsonAsync<List<RoomCategoryDTO>>();
                        ViewBag.Categories = new SelectList(categories, "RoomCategoriesId", "RoomCategoriesName");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var roomList = JsonConvert.DeserializeObject<List<RoomDTO>>(responseContent);

                        if (searchDTO.startdate != null && searchDTO.enddate != null)
                        {
                            HttpResponseMessage roomvalidResponse = await client.GetAsync(DefaultApiUrl + "Room/SearchRoomByDate?startDate=" + searchDTO.startdate + "&endDate=" + searchDTO.enddate);
                            roomList = JsonConvert.DeserializeObject<List<RoomDTO>>(responseContent);
                        }

                        if (!string.IsNullOrEmpty(searchDTO.roomname))
                        {
                            roomList = roomList?.Where(r => r.RoomName.Contains(searchDTO.roomname, StringComparison.OrdinalIgnoreCase)).ToList();
                        }

                        if (!string.IsNullOrEmpty(searchDTO.roomcategory))
                        {
                            int roomCategoriesId = int.Parse(searchDTO.roomcategory);
                            roomList = roomList?.Where(r => r.RoomCategoriesId == roomCategoriesId).ToList();
                        }

                        if (!string.IsNullOrEmpty(searchDTO.pricefrom) || !string.IsNullOrEmpty(searchDTO.priceto))
                        {
                            if (string.IsNullOrEmpty(searchDTO.pricefrom) && !string.IsNullOrEmpty(searchDTO.priceto))
                            {
                                int priceTo = int.Parse(searchDTO.priceto);
                                roomList = roomList?.Where(r => r.Price < priceTo).ToList();
                            }
                            if (string.IsNullOrEmpty(searchDTO.priceto) && !string.IsNullOrEmpty(searchDTO.pricefrom))
                            {
                                int priceFrom = int.Parse(searchDTO.pricefrom);
                                roomList = roomList?.Where(r => r.Price > priceFrom).ToList();
                            }
                            if (!string.IsNullOrEmpty(searchDTO.pricefrom) && !string.IsNullOrEmpty(searchDTO.priceto))
                            {
                                int PriceTo = int.Parse(searchDTO.priceto);
                                int PriceFrom = int.Parse(searchDTO.pricefrom);

                                roomList = roomList?.Where(r => r.Price > PriceFrom && r.Price < PriceTo).ToList();
                            }
                        }

                        switch (searchDTO.sortby)
                        {
                            case "name_desc":
                                roomList = roomList?.OrderByDescending(r => r.RoomName).ToList();
                                break;
                            case "price":
                                roomList = roomList?.OrderBy(r => r.Price).ToList();
                                break;
                            case "price_desc":
                                roomList = roomList?.OrderByDescending(r => r.Price).ToList();
                                break;
                            default:
                                roomList = roomList?.OrderBy(r => r.RoomName).ToList();
                                break;
                        }

                        ViewBag.roomcategory = searchDTO.roomcategory;
                        ViewBag.pricefrom = searchDTO.pricefrom;
                        ViewBag.priceto = searchDTO.priceto;
                        ViewBag.sortby = searchDTO.sortby;
                        ViewBag.roomname = searchDTO.roomname;
                        ViewBag.startdate = searchDTO.startdate.ToString();
                        ViewBag.enddate = searchDTO.enddate.ToString();

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

        public async Task<ActionResult> RoomDetail(int roomId, string sortby, int? page)
        {
            var viewModel = new HomeModel();

            try
            {
                //HttpResponseMessage serviceAvailableResponse = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/Room/GetServiceInRoom?roomId=" + roomId);
                HttpResponseMessage serviceAvailableResponse = await client.GetAsync(DefaultApiUrl + "Room/GetServiceInRoom?roomId=" + roomId);

                if (serviceAvailableResponse.IsSuccessStatusCode)
                {
                    var services = await serviceAvailableResponse.Content.ReadFromJsonAsync<List<ServiceDTO>>();

                    ViewBag.ServiceAvailable = new SelectList(services, "ServiceId", "ServiceName");
                }

                //HttpResponseMessage feedbackResponse = await client.GetAsync("https://localhost:7255/api/Feedback/GetAllFeedbackInRoom?roomID=" + roomId);
                HttpResponseMessage feedbackResponse = await client.GetAsync(DefaultApiUrl + "Feedback/GetAllFeedbackInRoom?roomID=" + roomId);

                if (feedbackResponse.IsSuccessStatusCode)
                {
                    var feedback = await feedbackResponse.Content.ReadFromJsonAsync<List<FeedbackDTO>>();

                    ViewBag.FeedbackCount = feedback?.Count();

                    switch (sortby)
                    {
                        case "5star":
                            feedback = feedback?.Where(f => f.NumberStart == 5).ToList();
                            break;
                        case "4star":
                            feedback = feedback?.Where(f => f.NumberStart == 4).ToList();
                            break;
                        case "3star":
                            feedback = feedback?.Where(f => f.NumberStart == 3).ToList();
                            break;
                        case "2star":
                            feedback = feedback?.Where(f => f.NumberStart == 2).ToList();
                            break;
                        case "1star":
                            feedback = feedback?.Where(f => f.NumberStart == 1).ToList();
                            break;
                        default:
                            break;
                    }
                    ViewBag.sortby = sortby;

                    ViewBag.FeedbacksCount = feedback?.Count();

                    page = page ?? 1;

                    //HttpResponseMessage paggingResponse = await client.GetAsync("https://localhost:7255/api/Feedback/PaginationInRoom?roomID=" + roomId + "&starnumber=" + (sortby ?? "0") + "&pagenumber=" + page);
                    HttpResponseMessage paggingResponse = await client.GetAsync(DefaultApiUrl + "Feedback/PaginationInRoom?roomID=" + roomId + "&starnumber=" + (sortby ?? "0") + "&pagenumber=" + page);

                    ViewBag.CurrentPage = page;

                    if (paggingResponse.IsSuccessStatusCode)
                    {
                        var feedbacks = await paggingResponse.Content.ReadFromJsonAsync<List<FeedbackDTO>>();
                        viewModel.Feedback = feedbacks;

                    }
                    else
                    {
                        viewModel.Feedback = feedback;
                    }
                }

                //HttpResponseMessage serviceUnavailableResponse = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/Room/GetServiceOutRoom?roomId=" + roomId);
                HttpResponseMessage serviceUnavailableResponse = await client.GetAsync(DefaultApiUrl + "Room/GetServiceOutRoom?roomId=" + roomId);

                if (serviceUnavailableResponse.IsSuccessStatusCode)
                {
                    var services = await serviceUnavailableResponse.Content.ReadFromJsonAsync<List<ServiceDTO>>();

                    ViewBag.ServiceUnavailable = services;
                }

                //HttpResponseMessage voteNumberResponse = await client.GetAsync("https://localhost:7255/api/Feedback/GetRoomVoteNumber?roomID=" + roomId);
                HttpResponseMessage voteNumberResponse = await client.GetAsync(DefaultApiUrl + "Feedback/GetRoomVoteNumber?roomID=" + roomId);

                if (voteNumberResponse.IsSuccessStatusCode)
                {
                    var voteNumber = await voteNumberResponse.Content.ReadFromJsonAsync<VoteNumberDTO>();

                    viewModel.VoteNumberas = voteNumber;
                }

                //HttpResponseMessage roomStarResponse = await client.GetAsync("https://localhost:7255/api/Feedback/GetRoomStar?roomID=" + roomId);
                HttpResponseMessage roomStarResponse = await client.GetAsync(DefaultApiUrl + "Feedback/GetRoomStar?roomID=" + roomId);

                if (roomStarResponse.IsSuccessStatusCode)
                {
                    var content = await roomStarResponse.Content.ReadAsStringAsync();

                    if (double.TryParse(content, out double roomStar))
                    {
                        ViewBag.RoomStar = roomStar;
                    }
                }

                //HttpResponseMessage response = await client.GetAsync(ApiUrlRoomDetail + roomId);
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Room/GetRoom/" + roomId);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var roomDto = JsonConvert.DeserializeObject<RoomDTO>(responseContent);

                    viewModel.Room = roomDto;

                    return View(viewModel);
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


        public async Task<IActionResult> ServiceList(ServiceCategoryDTO serviceCategory, int page = 1, int pagesize = 6, string CategoriesName = "", string viewstyle = "grid", string sortby = "")
        {
            try
            {
                var json = JsonConvert.SerializeObject(serviceCategory);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlServiceCategoryList + "/GetAllServiceCategory");
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "ServiceCategory/GetAllServiceCategory");


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

                        ViewBag.CategoriesName = CategoriesName;
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
            public List<FeedbackDTO> Feedback { get; set; }
            public VoteNumberDTO VoteNumberas { get; set; }
            public RoomDTO Room { get; set; }
            public List<ServiceCategoryDTO> ListServiceCategory { get; set; }
            public List<ProductDTO> ListProductTop8 { get; set; }
            public List<ProductDTO> ListProductSecond8 { get; set; }
            public List<RoomCategoryDTO> ListRoomCategory { get; set; }
            public List<ProductCategoryDTO> ListProductCategories { get; set; }
        }

        public async Task<IActionResult> Index()
        {
            HomeModel homeModel = new HomeModel();
            try
            {
                //HttpResponseMessage responseCategoryProduct = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/ProductCategory/GetAll");
                HttpResponseMessage responseCategoryProduct = await client.GetAsync(DefaultApiUrl + "ProductCategory/GetAll");
                //HttpResponseMessage responseProduct = await client.GetAsync(DefaultApiUrlProductList + "/GetAll");
                HttpResponseMessage responseProduct = await client.GetAsync(DefaultApiUrl + "Product/GetAll");
                if (responseProduct.IsSuccessStatusCode && responseCategoryProduct.IsSuccessStatusCode)
                {
                    //HttpResponseMessage responseCategory = await client.GetAsync(DefaultApiUrlServiceCategoryList + "/GetAllServiceCategory");
                    HttpResponseMessage responseCategory = await client.GetAsync(DefaultApiUrl + "ServiceCategory/GetAllServiceCategory");
                    if (responseCategory.IsSuccessStatusCode)
                    {
                        //HttpResponseMessage responseRoomCategory = await client.GetAsync(DefaultApiUrlRoomCategoryList + "/GetAllRoomCustomer");
                        HttpResponseMessage responseRoomCategory = await client.GetAsync(DefaultApiUrl + "Room/GetAllRoomCustomer");
                        if (responseCategory.IsSuccessStatusCode)
                        {
                            var responseRoomCategoryContent = await responseRoomCategory.Content.ReadAsStringAsync();

                            if (!string.IsNullOrEmpty(responseRoomCategoryContent))
                            {
                                homeModel.ListRoomCategory = JsonConvert.DeserializeObject<List<RoomCategoryDTO>>(responseRoomCategoryContent);
                            }
                        }
                        var responseCategoryContent = await responseCategory.Content.ReadAsStringAsync();
                        var responseCategoryProductContent = await responseCategoryProduct.Content.ReadAsStringAsync();


                        if (!string.IsNullOrEmpty(responseCategoryContent))
                        {
                            homeModel.ListServiceCategory = JsonConvert.DeserializeObject<List<ServiceCategoryDTO>>(responseCategoryContent);
                        }
                        if (!string.IsNullOrEmpty(responseCategoryProductContent))
                        {
                            homeModel.ListProductCategories = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(responseCategoryProductContent);
                        }

                    }
                    var rep = await responseProduct.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(rep))
                    {
                        homeModel.ListProductTop8 = JsonConvert.DeserializeObject<List<ProductDTO>>(rep);

                        int currentPage = 1;
                        int pageSize = 8;

                        var firstPageProducts = homeModel.ListProductTop8
                            .Where(p => p.Quantity > 0)
                            .OrderByDescending(p => p.QuantitySold)
                            .Skip((currentPage - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        currentPage++;

                        var secondPageProducts = homeModel.ListProductTop8
                            .Where(p => p.Quantity > 0)
                            .OrderByDescending(p => p.QuantitySold)
                            .Skip((currentPage - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        homeModel.ListProductTop8 = firstPageProducts;
                        homeModel.ListProductSecond8 = secondPageProducts;
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

        public async Task<IActionResult> ServiceDetail(int serviceCategoryId, int serviceIds, string sortby, int? page)
        {
            ServiceDetailModel model = new ServiceDetailModel();
            //HttpResponseMessage response = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/ServiceCategory/ServiceCategorysID/" + serviceCategoryId);
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "ServiceCategory/ServiceCategorysID/" + serviceCategoryId);
            //HttpResponseMessage partnerResponse = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/Partner/GetAllPartner");
            HttpResponseMessage partnerResponse = await client.GetAsync(DefaultApiUrl + "Partner/GetAllPartner");

            if (User.Identity.IsAuthenticated)
            {
                ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
                string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

                //HttpResponseMessage PetInfo = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/PetInfo/" + email);
                HttpResponseMessage PetInfo = await client.GetAsync(DefaultApiUrl + "PetInfo/" + email);

                if (PetInfo.IsSuccessStatusCode)
                {
                    var responsePetInfo = await PetInfo.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responsePetInfo))
                    {
                        var petInfo = JsonConvert.DeserializeObject<AccountInfo>(responsePetInfo);
                        model.account = petInfo;
                    }
                }
            }

            if (partnerResponse.IsSuccessStatusCode)
            {
                var responsepartnerContent = await partnerResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responsepartnerContent))
                {
                    var partners = JsonConvert.DeserializeObject<List<PartnerInfo>>(responsepartnerContent);
                    ViewBag.Partners = new SelectList(partners, "PartnerInfoId", "LastName");
                }
            }

            //HttpResponseMessage ServiceStarResponse = await client.GetAsync("https://localhost:7255/api/Feedback/GetServiceStar?serviceID=" + serviceIds);
            HttpResponseMessage ServiceStarResponse = await client.GetAsync(DefaultApiUrl + "Feedback/GetServiceStar?serviceID=" + serviceIds);

            if (ServiceStarResponse.IsSuccessStatusCode)
            {
                var content = await ServiceStarResponse.Content.ReadAsStringAsync();

                if (double.TryParse(content, out double serviceStar))
                {
                    ViewBag.serviceStar = serviceStar;
                }
            }

            //HttpResponseMessage voteNumberResponse = await client.GetAsync("https://localhost:7255/api/Feedback/GetServiceVoteNumber?serviceID=" + serviceIds);
            HttpResponseMessage voteNumberResponse = await client.GetAsync(DefaultApiUrl + "Feedback/GetServiceVoteNumber?serviceID=" + serviceIds);

            if (voteNumberResponse.IsSuccessStatusCode)
            {
                var voteNumber = await voteNumberResponse.Content.ReadFromJsonAsync<VoteNumberDTO>();

                model.VoteNumberas = voteNumber;
            }

            //HttpResponseMessage feedbackResponse = await client.GetAsync("https://localhost:7255/api/Feedback/GetAllFeedbackInService?serviceID=" + serviceIds);
            HttpResponseMessage feedbackResponse = await client.GetAsync(DefaultApiUrl + "Feedback/GetAllFeedbackInService?serviceID=" + serviceIds);

            if (feedbackResponse.IsSuccessStatusCode)
            {
                var feedback = await feedbackResponse.Content.ReadFromJsonAsync<List<FeedbackDTO>>();

                ViewBag.FeedbackCount = feedback?.Count();

                switch (sortby)
                {
                    case "5star":
                        feedback = feedback?.Where(f => f.NumberStart == 5).ToList();
                        break;
                    case "4star":
                        feedback = feedback?.Where(f => f.NumberStart == 4).ToList();
                        break;
                    case "3star":
                        feedback = feedback?.Where(f => f.NumberStart == 3).ToList();
                        break;
                    case "2star":
                        feedback = feedback?.Where(f => f.NumberStart == 2).ToList();
                        break;
                    case "1star":
                        feedback = feedback?.Where(f => f.NumberStart == 1).ToList();
                        break;
                    default:
                        break;
                }
                ViewBag.sortby = sortby;

                ViewBag.FeedbacksCount = feedback?.Count();
                page = page ?? 1;

                //HttpResponseMessage paggingResponse = await client.GetAsync("https://localhost:7255/api/Feedback/PaginationInService?serviceID=" + serviceIds + "&starnumber=" + (sortby ?? "0") + "&pagenumber=" + page);
                HttpResponseMessage paggingResponse = await client.GetAsync(DefaultApiUrl + "Feedback/PaginationInService?serviceID=" + serviceIds + "&starnumber=" + (sortby ?? "0") + "&pagenumber=" + page);

                ViewBag.CurrentPage = page;

                if (paggingResponse.IsSuccessStatusCode)
                {
                    var feedbacks = await paggingResponse.Content.ReadFromJsonAsync<List<FeedbackDTO>>();
                    model.Feedback = feedbacks;
                }
                else
                {
                    model.Feedback = feedback;
                }
            }

            if (response.IsSuccessStatusCode)
            {
                //HttpResponseMessage responseCategory = await client.GetAsync(DefaultApiUrlServiceCategoryList + "/GetAllServiceCategory");
                HttpResponseMessage responseCategory = await client.GetAsync(DefaultApiUrl + "ServiceCategory/GetAllServiceCategory");
                if (responseCategory.IsSuccessStatusCode)
                {
                    var responseCategoryContent = await responseCategory.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseCategoryContent))
                    {
                        var serviceCategories = JsonConvert.DeserializeObject<List<ServiceCategoryDTO>>(responseCategoryContent);
                        model.CaServices = serviceCategories;
                    }
                }
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
                        /*HttpResponseMessage responseService = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/ServiceCategory/GetServiceByServiceCategoryAndServiceID/"
                        + "?serviceCategoryId=" + serviceCategoryId + "&serviceId=" + serviceId);*/
                        
                        HttpResponseMessage responseService = await client.GetAsync(DefaultApiUrl + "ServiceCategory/GetServiceByServiceCategoryAndServiceID/"
                        + "?serviceCategoryId=" + serviceCategoryId + "&serviceId=" + serviceId);

                        var responseContentService = await responseService.Content.ReadAsStringAsync();
                        var service = JsonConvert.DeserializeObject<ServiceDTO>(responseContentService);
                        model.Service = service;
                        model.ServiceCategory = servicecategory;

                        return View(model);
                    }
                    else
                    {
                        /*HttpResponseMessage responseService = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/ServiceCategory/GetServiceByServiceCategoryAndServiceID/"
                     + "?serviceCategoryId=" + serviceCategoryId + "&serviceId=" + serviceIds);*/

                        HttpResponseMessage responseService = await client.GetAsync(DefaultApiUrl + "ServiceCategory/GetServiceByServiceCategoryAndServiceID/"
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
            public List<FeedbackDTO> Feedback { get; set; }
            public VoteNumberDTO VoteNumberas { get; set; }
            public ServiceDTO Service { get; set; }
            public ServiceCategoryDTO ServiceCategory { get; set; }
            public ProductDTO Product { get; set; }
            public List<ServiceCategoryDTO> CaServices { get; set; }
            public List<PartnerInfo> Partners { get; set; }
            public List<PetInfo> PetInfo { get; set; }
            public AccountInfo account { get; set; }

        }

        public class BlogModel
        {
            public List<BlogDTO> Blog { get; set; }
            public List<ProductDTO> ListProductTop3 { get; set; }
            public List<BlogDTO> ListBlogTop3 { get; set; }
        }


        public async Task<IActionResult> BlogList(BlogDTO blog, int page = 1, int pagesize = 6, string BlogName = "", string sortby = "")
        {
            BlogModel blogModel = new BlogModel();
            try
            {
                var json = JsonConvert.SerializeObject(blog);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlBlogList + "/GetAllBlog");
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Blog/GetAllBlog");

                if (response.IsSuccessStatusCode)
                {
                    //HttpResponseMessage responseProduct = await client.GetAsync(DefaultApiUrlProductList + "/GetAll");
                    HttpResponseMessage responseProduct = await client.GetAsync(DefaultApiUrl + "Product/GetAll");

                    if (responseProduct.IsSuccessStatusCode)
                    {
                        var rep = await responseProduct.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(rep))
                        {
                            blogModel.ListProductTop3 = JsonConvert.DeserializeObject<List<ProductDTO>>(rep);

                            int currentPage = 1;
                            int pageSize = 3;

                            var firstPageProducts = blogModel.ListProductTop3.OrderByDescending(p => p.QuantitySold).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                            currentPage++;

                            blogModel.ListProductTop3 = firstPageProducts;
                        }
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var rep = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(rep))
                        {
                            blogModel.ListBlogTop3 = JsonConvert.DeserializeObject<List<BlogDTO>>(rep);

                            int currentPage = 1;
                            int pageSize = 3;
                            var newestProducts = blogModel.ListBlogTop3
                            .OrderByDescending(p => p.PublisheDate)
                            .Skip((currentPage - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                            currentPage++;

                            blogModel.ListBlogTop3 = newestProducts;
                        }
                    }
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var blogList = JsonConvert.DeserializeObject<List<BlogDTO>>(responseContent);
                        // tìm kiếm theo tên 
                        if (!string.IsNullOrEmpty(BlogName) && blogList != null)
                        {
                            blogList = blogList
                                .Where(c => c.PageTile != null && c.PageTile.Contains(BlogName, StringComparison.OrdinalIgnoreCase))
                                .ToList();
                        }
                        //tìm kiếm tên theo bảng chữ cái từ a-z và từ z-a
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
                        blogModel.Blog = currentPageBlogList;


                        return View(blogModel);
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
        public class BlogDetailModel
        {
            public BlogDTO BlogDetail { get; set; }
            public List<BlogDTO> Blog { get; set; }
            public List<ProductDTO> ListProductTop3 { get; set; }
            public List<BlogDTO> ListBlogTop3 { get; set; }

        }
        public async Task<IActionResult> BlogDetail(int blogId)
        {
            BlogDetailModel blog = new BlogDetailModel();
            try
            {
                //HttpResponseMessage responseBlogDetail = await client.GetAsync(DefaultApiUrlBlogDetail + blogId);
                HttpResponseMessage responseBlogDetail = await client.GetAsync(DefaultApiUrl + "Blog/BlogID/" + blogId);
                //HttpResponseMessage responseBlogList = await client.GetAsync(DefaultApiUrlBlogList + "/GetAllBlog");
                HttpResponseMessage responseBlogList = await client.GetAsync(DefaultApiUrl + "Blog/GetAllBlog");
                //HttpResponseMessage responseProduct = await client.GetAsync(DefaultApiUrlProductList + "/GetAll");
                HttpResponseMessage responseProduct = await client.GetAsync(DefaultApiUrl + "Product/GetAll");
                if (responseBlogDetail.IsSuccessStatusCode)
                {
                    // list ra 3 sản phẩm bán chạy nhất 
                    if (responseProduct.IsSuccessStatusCode)
                    {
                        var product = await responseProduct.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(product))
                        {
                            blog.ListProductTop3 = JsonConvert.DeserializeObject<List<ProductDTO>>(product);

                            int currentPage = 1;
                            int pageSize = 3;

                            var firstPageProducts = blog.ListProductTop3.OrderByDescending(p => p.QuantitySold).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                            currentPage++;

                            blog.ListProductTop3 = firstPageProducts;
                        }
                    }
                    // List ra danh sách blog
                    if (responseBlogList.IsSuccessStatusCode)
                    {
                        var Blog = await responseBlogList.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(Blog))
                        {
                            blog.Blog = JsonConvert.DeserializeObject<List<BlogDTO>>(Blog);
                        }
                    }
                    // list ra detail của id đó 
                    var BlogDetail = await responseBlogDetail.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(BlogDetail))
                    {
                        blog.BlogDetail = JsonConvert.DeserializeObject<BlogDTO>(BlogDetail);
                    }
                    if (responseBlogList.IsSuccessStatusCode)
                    {
                        var rep = await responseBlogList.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(rep))
                        {
                            blog.ListBlogTop3 = JsonConvert.DeserializeObject<List<BlogDTO>>(rep);

                            int currentPage = 1;
                            int pageSize = 3;
                            var newestProducts = blog.ListBlogTop3
                            .OrderByDescending(p => p.PublisheDate)
                            .Skip((currentPage - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                            currentPage++;

                            blog.ListBlogTop3 = newestProducts;
                        }
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
            return View(blog);
        }
        public class PartModel
        {
            public List<PartnerInfo> partner { set; get; }
            public List<ProductDTO> ListProductTop3 { get; set; }
            public List<ServiceCategoryDTO> CaServices { get; set; }



        }
        public class PartDeatilModel
        {
            public PartnerInfo Partner { get; set; }
            public List<PartnerInfo> partner { set; get; }
            public List<ProductDTO> ListProductTop3 { get; set; }
            public List<ServiceCategoryDTO> CaServices { get; set; }



        }
        public async Task<IActionResult> Partner(int page = 1, int pagesize = 6, string PartName = "")
        {
            PartModel partModel = new PartModel();
            try
            {
                //HttpResponseMessage responseProduct = await client.GetAsync(DefaultApiUrlProductList + "/GetAll");
                HttpResponseMessage responseProduct = await client.GetAsync(DefaultApiUrl + "Product/GetAll");

                if (responseProduct.IsSuccessStatusCode)
                {
                    var rep = await responseProduct.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(rep))
                    {
                        partModel.ListProductTop3 = JsonConvert.DeserializeObject<List<ProductDTO>>(rep);

                        int currentPage = 1;
                        int pageSize = 3;

                        var firstPageProducts = partModel.ListProductTop3.OrderByDescending(p => p.QuantitySold).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                        currentPage++;

                        partModel.ListProductTop3 = firstPageProducts;
                    }
                }
                //HttpResponseMessage responseCategoryService = await client.GetAsync(DefaultApiUrlServiceCategoryList + "/GetAllServiceCategory");
                HttpResponseMessage responseCategoryService = await client.GetAsync(DefaultApiUrl + "ServiceCategory/GetAllServiceCategory");
                if (responseCategoryService.IsSuccessStatusCode)
                {
                    var responseCategoryContent = await responseCategoryService.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseCategoryContent))
                    {
                        var serviceCategories = JsonConvert.DeserializeObject<List<ServiceCategoryDTO>>(responseCategoryContent);
                        partModel.CaServices = serviceCategories;
                    }
                }
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Partner/GetAllPartner");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var partList = JsonConvert.DeserializeObject<List<PartnerInfo>>(responseContent);
                        // tìm kiếm theo tên 
                        if (!string.IsNullOrEmpty(PartName) && partList != null)
                        {
                            partList = partList
                                .Where(c => c.FirstName != null && c.FirstName.Contains(PartName, StringComparison.OrdinalIgnoreCase))
                                .ToList();
                        }


                        int totalItems = partList.Count;
                        int totalPages = (int)Math.Ceiling(totalItems / (double)pagesize);
                        int startIndex = (page - 1) * pagesize;
                        List<PartnerInfo> currentPagePartnerList = partList.Skip(startIndex).Take(pagesize).ToList();

                        ViewBag.TotalPages = totalPages;
                        ViewBag.CurrentPage = page;
                        ViewBag.PageSize = pagesize;

                        ViewBag.PartName = PartName;
                        ViewBag.pagesize = pagesize;
                        partModel.partner = currentPagePartnerList;

                        return View(partModel);
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
        public async Task<IActionResult> PartnerDetail(int partnerID)
        {
            PartDeatilModel partModel = new PartDeatilModel();
            try
            {
                //HttpResponseMessage responseProduct = await client.GetAsync(DefaultApiUrlProductList + "/GetAll");
                HttpResponseMessage responseProduct = await client.GetAsync(DefaultApiUrl + "Product/GetAll");
                //HttpResponseMessage responsedetail = await client.GetAsync("https://localhost:7255/api/Partner/PartnerInfoId?PartnerInfoId=" + partnerID);
                HttpResponseMessage responsedetail = await client.GetAsync(DefaultApiUrl + "Partner/PartnerInfoId?PartnerInfoId=" + partnerID);
                if (responsedetail.IsSuccessStatusCode)
                {
                    if (responseProduct.IsSuccessStatusCode)
                    {
                        var rep = await responseProduct.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(rep))
                        {
                            partModel.ListProductTop3 = JsonConvert.DeserializeObject<List<ProductDTO>>(rep);

                            int currentPage = 1;
                            int pageSize = 3;

                            var firstPageProducts = partModel.ListProductTop3.OrderByDescending(p => p.QuantitySold).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                            currentPage++;

                            partModel.ListProductTop3 = firstPageProducts;
                        }
                    }
                    //HttpResponseMessage responseCategoryService = await client.GetAsync(DefaultApiUrlServiceCategoryList + "/GetAllServiceCategory");
                    HttpResponseMessage responseCategoryService = await client.GetAsync(DefaultApiUrl + "ServiceCategory/GetAllServiceCategory");
                    if (responseCategoryService.IsSuccessStatusCode)
                    {
                        var responseCategoryContent = await responseCategoryService.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseCategoryContent))
                        {
                            var serviceCategories = JsonConvert.DeserializeObject<List<ServiceCategoryDTO>>(responseCategoryContent);
                            partModel.CaServices = serviceCategories;
                        }
                    }
                    HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Partner/GetAllPartner");
                    if (response.IsSuccessStatusCode)
                    {
                        var responsePartContent = await response.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responsePartContent))
                        {
                            partModel.partner = JsonConvert.DeserializeObject<List<PartnerInfo>>(responsePartContent);
                        }
                    }

                    var Partnerdetail = await responsedetail.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(Partnerdetail))
                    {
                        partModel.Partner = JsonConvert.DeserializeObject<PartnerInfo>(Partnerdetail);
                        return View(partModel);
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


        public class CartItem
        {
            // Product
            public int quantityProduct { set; get; }
            public ProductDTO product { set; get; }

            // Service
            public int ServiceId { get; set; }
            public double? Price { get; set; }
            public double? Weight { get; set; }
            public double? PriceService { get; set; }
            public int? PartnerInfoId { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public PartnerInfo? PartnerInfo { get; set; }
            public ServiceDTO service { set; get; }
            // Room
        }

        public const string CARTKEY = "cart";

        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            }
            return new List<CartItem>();
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int ServiceId, int PriceService, double Weight, int PartnerId, DateTime StartTime, DateTime EndTime)
        {
            ServiceDTO service = null;
            PartnerInfo partner = null;

            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Service/ServiceID/" + ServiceId);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var option = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                service = System.Text.Json.JsonSerializer.Deserialize<ServiceDTO>(responseContent, option);
            }
            if (PartnerId != 0)
            {
                HttpResponseMessage responsePartner = await client.GetAsync(DefaultApiUrl + "Partner/PartnerInfoId?PartnerInfoId=" + PartnerId);
                if (responsePartner.IsSuccessStatusCode)
                {
                    string responseContent = await responsePartner.Content.ReadAsStringAsync();
                    var option = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    partner = System.Text.Json.JsonSerializer.Deserialize<PartnerInfo>(responseContent, option);
                }
            }

            if (service != null)
            {
                var cart = GetCartItems();
                var cartitem = cart.Find(s => s.service != null && s.service.ServiceId == ServiceId);

                if (cartitem != null)
                {
                    TempData["WatingToast"] = "Dịch vụ đã có trong giỏ hàng.";
                    return RedirectToAction("Index", "Cart");
                }
                else
                {
                    // Thêm mới
                    cart.Add(new CartItem()
                    {
                        service = service,
                        PartnerInfo = partner,
                        Weight = Weight,
                        Price = service.Price,
                        PriceService = PriceService,
                        StartTime = StartTime,
                        EndTime = EndTime,
                        PartnerInfoId = PartnerId != 0 ? PartnerId : null
                    });
                }

                // Lưu cart vào Session
                SaveCartSession(cart);
            }

            // Kiểm tra xem đây có phải là yêu cầu Ajax không
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var cartItems = GetCartItems();
                int totalQuantity = cartItems.Select(item => item?.service?.ServiceId ?? 0)
                                              .Union(cartItems.Where(item => item?.product != null)
                                                              .Select(item => item.product.ProductId))
                                              .Count();

                return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng.", totalQuantity });
            }
            else
            {
                // Nếu không phải Ajax, chuyển hướng như trước
                return RedirectToAction("Index", "Cart");
            }
        }

        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }

        public IActionResult NotFound()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Thực hiện chuyển hướng đến trang 404 tùy chỉnh
            return RedirectToAction("NotFound", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }
        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Introduce()
        {
            return View();
        }

    }
}