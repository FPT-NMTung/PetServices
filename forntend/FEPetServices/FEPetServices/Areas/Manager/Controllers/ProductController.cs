using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "ManaOnly")]
    public class ProductController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        //private string DefaultApiUrlProductList = "";
        //private string DefaultApiUrlProductDetail = "";
        //private string DefaultApiUrlProductAdd = "";
        //private string DefaultApiUrlProductUpdate = "";
        public ProductController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrlProductList = "https://pet-service-api.azurewebsites.net/api/Product";
            //DefaultApiUrlProductDetail = "https://pet-service-api.azurewebsites.net/api/Product/ProductID";
            //DefaultApiUrlProductAdd = "https://pet-service-api.azurewebsites.net/api/Product/Add";
            //DefaultApiUrlProductUpdate = "https://pet-service-api.azurewebsites.net/api/Product/Update?proId=";
        }
        public async Task<IActionResult> Index(ProductDTO productDTO)
        {
            try
            {
                ViewBag.Title = "Danh sách sản phẩm";
                var json = JsonConvert.SerializeObject(productDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Product" + "/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    var rep = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(rep))
                    {
                        var productList = JsonConvert.DeserializeObject<List<ProductDTO>>(rep);
                        //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                        return View(productList);
                    }
                    else
                    {
                        ViewBag.ErrorToast = "API trả về dữ liệu rỗng";
                    }
                }
                else
                {
                    ViewBag.ErrorToast = "Tải dữ liệu lên thất bại. Vui lòng tải lại trang!";
                }
                
            }
            catch (Exception ex)
            {
                ViewBag.ErrorToast = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View();
        }
        public async Task<IActionResult> Add([FromForm] ProductDTO pro, IFormFile image)
        {
            try
            {
                ViewBag.Title = "Thêm sản phẩm mới";
                HttpResponseMessage proCateResponse = await client.GetAsync(DefaultApiUrl + "ProductCategory/GetAll");
                if (proCateResponse.IsSuccessStatusCode)
                {
                    var proCategories = await proCateResponse.Content.ReadFromJsonAsync<List<ProductCategoryDTO>>();
                    proCategories = proCategories.Where(r => r.Status == true).ToList();
                    ViewBag.ProCategories = new SelectList(proCategories, "ProCategoriesId", "ProCategoriesName");
                }

                if (pro.ProductName == null) { return View(); }
                if (image != null && image.Length > 0)
                {
                    string filename = GenerateRandomNumber(5) + image.FileName;
                    filename = Path.GetFileName(filename);
                    string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Product/", filename);
                    var stream = new FileStream(uploadfile, FileMode.Create);
                    image.CopyToAsync(stream);
                    pro.Picture = "/img/Product/" + filename;
                }
                else
                {
                    return View(pro);
                }
                var json = JsonConvert.SerializeObject(pro);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Gửi dữ liệu lên máy chủ
                    HttpResponseMessage response = await client.PostAsync(DefaultApiUrl + "Product/Add", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessToast"] = "Thêm sản phẩm thành công!";
                        return View(pro); // Chuyển hướng đến trang thành công hoặc trang danh sách.
                    }
                    else
                    {
                        TempData["ErrorToast"] = "Thêm sản phẩm thất bại. Vui lòng thử lại sau.";
                        return View(pro); // Hiển thị lại biểu mẫu với dữ liệu đã điền.
                    }
                
               
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(pro); // Hiển thị lại biểu mẫu với dữ liệu đã điền
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
        public async Task<IActionResult> Update(int proId, ProductDTO productDTO)
        {
            try
            {
                ViewBag.Title = "Chỉnh sửa Thông tin sản phẩm";
                //goi api de lay thong tin can sua
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Product/ProductID/" + proId);
                //HttpResponseMessage response = await client.GetAsync("https://localhost:7255/api/Product/Add" + proId);

                HttpResponseMessage proCateResponse = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/ProductCategory/GetAll");
                if (proCateResponse.IsSuccessStatusCode)
                {
                    var proCategories = await proCateResponse.Content.ReadFromJsonAsync<List<ProductCategoryDTO>>();
                    proCategories = proCategories.Where(r => r.Status == true).ToList();
                    ViewBag.ProCategories = new SelectList(proCategories, "ProCategoriesId", "ProCategoriesName");
                }
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    ProductDTO managerInfos = System.Text.Json.JsonSerializer.Deserialize<ProductDTO>(responseContent, options);

                    return View(managerInfos);

                }
                else
                {
                    TempData["ErrorToast"] = "Tải dữ liệu lên thất bại. Vui lòng tải lại trang.";
                }
                if (Request.Form["Status"] == "on")
                {
                    productDTO.Status = true;
                }
                else
                {
                    productDTO.Status = false;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromForm] ProductDTO productDTO, IFormFile image, int proId, int SelectedCategory)
        {
            try
            {
                HttpResponseMessage proCateResponse = await client.GetAsync(DefaultApiUrl + "ProductCategory/GetAll");
                if (proCateResponse.IsSuccessStatusCode)
                {
                    var proCategories = await proCateResponse.Content.ReadFromJsonAsync<List<ProductCategoryDTO>>();
                    ViewBag.ProCategories = new SelectList(proCategories, "ProCategoriesId", "ProCategoriesName");
                }
                if (image != null && image.Length > 0)
                {
                    // Handle the case when a new image is uploaded
                    var imagePath = "/img/Product/" + image.FileName;
                    productDTO.Picture = imagePath;

                    var physicalImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Product", image.FileName);
                    //add
                    using (var stream = new FileStream(physicalImagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                }
                else
                {
                    // Handle the case when no new image is uploaded
                    HttpResponseMessage responseForImage = await client.GetAsync(DefaultApiUrl + "Product/ProductID/" + proId);

                    if (responseForImage.IsSuccessStatusCode)
                    {
                        var responseContent = await responseForImage.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var existingServiceCategory = JsonConvert.DeserializeObject<ProductDTO>(responseContent);
                            /*var existingServiceCategory = existingServiceCategoryList.FirstOrDefault();*/
                            if (existingServiceCategory != null)
                            {
                                // Assign the existing image path to service.Picture.
                                productDTO.Picture = existingServiceCategory.Picture;
                            }
                        }
                    }
                }
                if (Request.Form["Status"] == "on")
                {
                    productDTO.Status = true;
                }
                else
                {
                    productDTO.Status = false;
                }
                
                var json = JsonConvert.SerializeObject(productDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(DefaultApiUrl + "Product/Update?proId=" + proId, content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Chỉnh sửa sản phẩm thành công!";
                    return RedirectToAction("Update", new { proId = proId });
                }
                else
                {
                    TempData["ErrorToast"] = "Chỉnh sửa sản phẩm không thành công!";
                    return View(productDTO);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(productDTO);
            }
        }
    }
}
