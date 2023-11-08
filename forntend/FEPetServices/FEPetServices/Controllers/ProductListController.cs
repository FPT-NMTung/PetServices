using FEPetServices.Areas.DTO;
using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Controllers
{
    public class ProductListController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlProductList = "";
        private string DefaultApiUrlProductDetail = "";
        private string DefaultApiUrlProductCategoryList = "";

        public ProductListController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            DefaultApiUrl = "";
            DefaultApiUrlProductList = "https://pet-service-api.azurewebsites.net/api/Product";
            DefaultApiUrlProductDetail = "https://pet-service-api.azurewebsites.net/api/Product/ProductID"; 
            DefaultApiUrlProductCategoryList = "https://pet-service-api.azurewebsites.net/api/ProductCategory/GetAll";
        }

        public async Task<IActionResult> Index(ProductDTO productDTO, ProductSearch searchDTO)
        {
            try
            {
                var json = JsonConvert.SerializeObject(productDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlProductList + "/GetAll");
                HttpResponseMessage ProductCategoryResponse = await client.GetAsync(DefaultApiUrlProductCategoryList);
                if (ProductCategoryResponse.IsSuccessStatusCode)
                {
                    var categories = await ProductCategoryResponse.Content.ReadFromJsonAsync<List<ProductCategoryDTO>>();
                    ViewBag.categories = new SelectList(categories, "ProCategoriesId", "ProCategoriesName");
                }
                if (response.IsSuccessStatusCode)
                {
                    var rep = await response.Content.ReadAsStringAsync();
                    
                    if (!string.IsNullOrEmpty(rep))
                    {
                        var productList = JsonConvert.DeserializeObject<List<ProductDTO>>(rep);
                        if (!string.IsNullOrEmpty(searchDTO.productcategory))
                        {
                            int productCategoriesId = int.Parse(searchDTO.productcategory);
                            productList = productList.Where(r => r.ProCategoriesId == productCategoriesId).ToList();
                        }
                        if (!string.IsNullOrEmpty(searchDTO.pricefrom) || !string.IsNullOrEmpty(searchDTO.priceto))
                        {
                            if (string.IsNullOrEmpty(searchDTO.pricefrom))
                            {
                                int priceTo = int.Parse(searchDTO.priceto);
                                productList = productList.Where(r => r.Price < priceTo).ToList();
                            }
                            if (string.IsNullOrEmpty(searchDTO.priceto))
                            {
                                int priceFrom = int.Parse(searchDTO.pricefrom);
                                productList = productList.Where(r => r.Price > priceFrom).ToList();
                            }
                            if (!string.IsNullOrEmpty(searchDTO.pricefrom) && !string.IsNullOrEmpty(searchDTO.priceto))
                            {
                                int PriceTo = int.Parse(searchDTO.priceto);
                                int PriceFrom = int.Parse(searchDTO.pricefrom);

                                productList = productList.Where(r => r.Price > PriceFrom && r.Price < PriceTo).ToList();
                            }
                        }
                        switch (searchDTO.sortby)
                        {
                            case "name_desc":
                                productList = productList.OrderByDescending(r => r.ProductName).ToList();
                                break;
                            case "price":
                                productList = productList.OrderBy(r => r.Price).ToList();
                                break;
                            case "price_desc":
                                productList = productList.OrderByDescending(r => r.Price).ToList();
                                break;
                            default:
                                productList = productList.OrderBy(r => r.ProductName).ToList();
                                break;
                        }
                        int page = searchDTO.page ?? 1; ;
                        int pagesize = searchDTO.pagesize ?? 6;

                        int totalItems = productList.Count;
                        int totalPages = (int)Math.Ceiling(totalItems / (double)pagesize);
                        int startIndex = (page - 1) * pagesize;
                        List<ProductDTO> currentPageProductList = productList.Skip(startIndex).Take(pagesize).ToList();

                        ViewBag.TotalPages = totalPages;
                        ViewBag.CurrentPage = searchDTO.page;
                        ViewBag.PageSize = searchDTO.pagesize;

                        ViewBag.productcategory = searchDTO.productcategory;
                        ViewBag.pricefrom = searchDTO.pricefrom;
                        ViewBag.priceto = searchDTO.priceto;
                        ViewBag.sortby = searchDTO.sortby;
                        ViewBag.roomname = searchDTO.productname;
                        ViewBag.pagesize = searchDTO.pagesize;
                        ViewBag.viewstyle = searchDTO.viewstyle;
                        return View(currentPageProductList);
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
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int proId, ProductDTO productDTO)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlProductDetail + "/" + proId);
                HttpResponseMessage proCateResponse = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/ProductCategory/GetAll");
                if (proCateResponse.IsSuccessStatusCode)
                {
                    var proCategories = await proCateResponse.Content.ReadFromJsonAsync<List<ProductCategoryDTO>>();
                    ViewBag.ProCategories = new SelectList(proCategories, "ProCategoriesId", "ProCategoriesName");
                }
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var option = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    ProductDTO managerInfos = System.Text.Json.JsonSerializer.Deserialize<ProductDTO>(responseContent, option);
                    return View(managerInfos);
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
    }
}
