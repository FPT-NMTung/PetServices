using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FEPetServices.Areas.Manager.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "ManaOnly")]
    public class ProductCategoryController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        //private string DefaultApiUrlProductCategoryList = "";
        //private string DefaultApiUrlProductCategoryDetail = "";
        //private string DefaultApiUrlProductCategoryAdd = "";
        //private string DefaultApiUrlProductCategoryUpdate = "";
        private readonly IConfiguration configuration;


        public ProductCategoryController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrlProductCategoryList = "https://pet-service-api.azurewebsites.net/api/ProductCategory";
            //DefaultApiUrlProductCategoryDetail = "https://pet-service-api.azurewebsites.net/api/ProductCategory/ProductCategorysID";
            //DefaultApiUrlProductCategoryAdd = "https://pet-service-api.azurewebsites.net/api/ProductCategory/CreateNewProductCategory";
            //DefaultApiUrlProductCategoryUpdate = "https://pet-service-api.azurewebsites.net/api/ProductCategory/Update?procateId=";
        }

        public async Task<IActionResult> Index(ProductCategoryDTO productCategoryDTO)
        {
            try
            {
                ViewBag.Title = "Danh sách loại sản phẩm";
                var json = JsonConvert.SerializeObject(productCategoryDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "ProductCategory/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    var rep = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(rep))
                    {
                        var productCateList = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(rep);

                        //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                        return View(productCateList);
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
        public async Task<IActionResult> CreateNewProductCategory([FromForm] ProductCategoryDTO proCategory, List<IFormFile> image)
        {
            try
            {
                ViewBag.Title = "Thêm loại sản phẩm mới";
                if (ModelState.IsValid) // Kiểm tra xem biểu mẫu có hợp lệ không
                {
                    if (proCategory.ProCategoriesName == null) { return View(); }
                    foreach (var file in image)
                    {
                        string filename = GenerateRandomNumber(5) + file.FileName;
                        filename = Path.GetFileName(filename);
                        string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/ProductCategory/", filename);
                        var stream = new FileStream(uploadfile, FileMode.Create);
                        file.CopyToAsync(stream);
                        proCategory.Picture = "/img/ProductCategory/" + filename;
                    }

                    var json = JsonConvert.SerializeObject(proCategory);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Gửi dữ liệu lên máy chủ
                    HttpResponseMessage response = await client.PostAsync(DefaultApiUrl + "ProductCategory/CreateNewProductCategory", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessToast"] = "Thêm loại sản phẩm thành công!";
                        return View(proCategory); // Chuyển hướng đến trang thành công hoặc trang danh sách
                    }
                    else
                    {
                        TempData["ErrorToast"] = "Thêm loại sản phẩm thất bại. Vui lòng thử lại sau.";
                        return View(proCategory); // Hiển thị lại biểu mẫu với dữ liệu đã điền
                    }
                }
                else
                {
                    return View(proCategory);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(proCategory); // Hiển thị lại biểu mẫu với dữ liệu đã điền
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
        public async Task<IActionResult> Update(int procateId, ProductCategoryDTO productCategoryDTO)
        {
            try
            {
                ViewBag.Title = "Chỉnh sửa thông tin loại sản phẩm";
                //goi api de lay thong tin can sua
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "ProductCategory/ProductCategorysID/"   + procateId);
                if (response.IsSuccessStatusCode)
                {
                    var rep = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(rep))
                    {
                        //deserialize du lieu tu api thanh ds cac doi tuongdto
                        var existPCL = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(rep);
                        //lay doi tuong can chinh tu ds
                        var existPC = existPCL.FirstOrDefault();
                        return View(existPC);
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
                if (Request.Form["Status"] == "on")
                {
                    productCategoryDTO.Status = true;
                }
                else
                {
                    productCategoryDTO.Status = false;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Update(ProductCategoryDTO productCategoryDTO, int procateId, IFormFile image)
        {
            try
            {
                if (image != null && image.Length > 0)
                {
                    // An image file has been uploaded, so update the image path.
                    Console.WriteLine(image);
                    // Save the image to a location (e.g., a folder in your application)
                    var imagePath = "/img/ProductCategory/" + image.FileName;
                    productCategoryDTO.Picture = imagePath;

                    // Save the image file to a folder on your server
                    var physicalImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "ProductCategory", image.FileName);
                    using (var stream = new FileStream(physicalImagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                }
                else
                {

                    HttpResponseMessage responseForImage = await client.GetAsync(DefaultApiUrl + "ProductCategory/ProductCategorysID/" + procateId);

                    if (responseForImage.IsSuccessStatusCode)
                    {
                        var responseContent = await responseForImage.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var existinPCL = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(responseContent);
                            var existingPC = existinPCL.FirstOrDefault();
                            if (existingPC != null)
                            {
                                // Assign the existing image path to serviceCategory.Prictue.
                                productCategoryDTO.Picture = existingPC.Picture;
                            }
                        }
                    }
                }
                if (Request.Form["Status"] == "on")
                {
                    productCategoryDTO.Status = true;
                }
                else
                {
                    productCategoryDTO.Status = false;
                }

                var json = JsonConvert.SerializeObject(productCategoryDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi dữ liệu lên máy chủ
                HttpResponseMessage response = await client.PutAsync(DefaultApiUrl + "ProductCategory/Update?procateId=" + procateId, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Chỉnh sửa sản phẩm thành công!";
                    return View(productCategoryDTO); // Chuyển hướng đến trang thành công hoặc trang danh sách
                }
                else
                {
                    TempData["ErrorToast"] = "Chỉnh sửa sản phẩm  thất bại. Vui lòng thử lại sau.";
                    return View(productCategoryDTO); // Hiển thị lại biểu mẫu với dữ liệu đã điền
                }
            }

            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(productCategoryDTO); // Hiển thị lại biểu mẫu với dữ liệu đã điền
            }
        }

    }
}
