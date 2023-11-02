using FEPetServices.Areas.DTO;
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
        private string DefaultApiUrlBlogList = "";

        public HomeController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrlServiceCategoryList = "https://localhost:7255/api/ServiceCategory";
            DefaultApiUrlServiceCategoryDetail = "https://localhost:7255/api/ServiceCategory/ServiceCategorysID/";
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