using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Areas.Customer.Controllers
{
    public class ProductListController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlProductList = "";
        private string DefaultApiUrlProductDetail = "";

        public ProductListController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            DefaultApiUrl = "";
            DefaultApiUrlProductList = "https://localhost:7255/api/Product";
            DefaultApiUrlProductDetail = "https://localhost:7255/api/Product/ProductID";
        }
        public async Task<IActionResult> Index(ProductDTO productDTO)
        {
            try
            {
                var json = JsonConvert.SerializeObject(productDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlProductList + "/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    var rep = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(rep))
                    {
                        var productList = JsonConvert.DeserializeObject<List<ProductDTO>>(rep);
                        return View(productList);
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
                HttpResponseMessage proCateResponse = await client.GetAsync("https://localhost:7255/api/ProductCategory/GetAll");
                if(proCateResponse.IsSuccessStatusCode)
                {
                    var proCategories = await proCateResponse.Content.ReadFromJsonAsync<List<ProductCategoryDTO>>();
                    ViewBag.ProCategories = new SelectList(proCategories, "ProCategoriesId", "ProCategoriesName");
                }
                if(response.IsSuccessStatusCode)
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
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View();
        }
    }
}
