using FEPetServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace FEPetServices.Controllers
{
    /* [Authorize(Policy = "CusOnly")]*/
    public class HomeController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrlServiceCategoryList = "";
        private string DefaultApiUrlServiceCategoryDetail = "";
        private string DefaultApiUrlServiceCategoryandService = "";

        public HomeController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrlServiceCategoryList = "https://localhost:7255/api/ServiceCategory";
            DefaultApiUrlServiceCategoryDetail = "https://localhost:7255/api/ServiceCategory/ServiceCategorysID/";
            
        }

        public async Task<IActionResult> ServiceList(ServiceCategoryDTO serviceCategory, int page = 1, int pageSize = 4, string CategoriesName = "" )
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

                        int totalItems = servicecategoryList.Count;
                        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                        int startIndex = (page - 1) * pageSize;
                        List<ServiceCategoryDTO> currentPageServicecategoryList = servicecategoryList.Skip(startIndex).Take(pageSize).ToList();

                        ViewBag.TotalPages = totalPages;
                        ViewBag.CurrentPage = page;
                        ViewBag.PageSize = pageSize;
                        ViewBag.CategoriesName =CategoriesName ;
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

        public IActionResult Index()
        {
            return View();
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