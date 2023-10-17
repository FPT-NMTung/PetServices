using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FEPetServices.Areas.Manager.Controllers
{
    public class ProductCategoryController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlProductCategoryList = "";
        private string DefaultApiUrlProductCategoryDetail = "";
        private string DefaultApiUrlProductCategoryAdd = "";
        private string DefaultApiUrlProductCategoryUpdate = "";

        public ProductCategoryController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "";
            DefaultApiUrlProductCategoryList = "https://localhost:7255/api/ProductCategory";
            DefaultApiUrlProductCategoryDetail = "https://localhost:7255/api/ProductCategory/ProductCategorysID";
            DefaultApiUrlProductCategoryAdd = "https://localhost:7255/api/ProductCategory/CreateNewProductCategory";
            DefaultApiUrlProductCategoryUpdate = "https://localhost:7255/api/ProductCategory/Update?procateId=";
        }

        public async Task<IActionResult> Index(ProductCategoryDTO productCategoryDTO)
        {
            try
            {
                var json = JsonConvert.SerializeObject(productCategoryDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlProductCategoryList + "/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    var rep = await response.Content.ReadAsStringAsync();
                    if(!string.IsNullOrEmpty(rep))
                    {
                        var productCateList = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(rep);
                        return View(productCateList);
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
            catch(Exception ex)
            {
                    ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View();
        }
        public async Task<IActionResult> CreateNewProductCategory([FromForm] ProductCategoryDTO proCategory, IFormFile image)
        {
            try
            {
                if (ModelState.IsValid) // Kiểm tra xem biểu mẫu có hợp lệ không
                {
                    if (image != null && image.Length > 0)
                    {
                        // Xử lý và lưu trữ ảnh
                        Console.WriteLine(image);
                        proCategory.Prictue = "/img/" + image.FileName.ToString();
                    }

                    var json = JsonConvert.SerializeObject(proCategory);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Gửi dữ liệu lên máy chủ
                    HttpResponseMessage response = await client.PostAsync(DefaultApiUrlProductCategoryAdd, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Thêm dịch vụ thành công!";
                        return View(proCategory); // Chuyển hướng đến trang thành công hoặc trang danh sách
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Thêm dịch vụ thất bại. Vui lòng thử lại sau.";
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
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return View(proCategory); // Hiển thị lại biểu mẫu với dữ liệu đã điền
            }
        }



        [HttpGet]
        public async Task<IActionResult> Update(int procateId)
        {
            try
            {
                //goi api de lay thong tin can sua
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlProductCategoryDetail + "/" + procateId);
                if (response.IsSuccessStatusCode)
                {
                    var rep = await response.Content.ReadAsStringAsync();
                    if(!string.IsNullOrEmpty(rep))
                    {
                        //deserialize du lieu tu api thanh ds cac doi tuongdto
                        var existPCL = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(rep);
                        //lay doi tuong can chinh tu ds
                        var existPC = existPCL.FirstOrDefault();
                        return View(existPC);
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
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View();
        }
    }
}
