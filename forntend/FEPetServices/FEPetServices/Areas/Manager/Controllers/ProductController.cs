using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FEPetServices.Areas.Manager.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private string DefaultApiUrlProductList = "";
        private string DefaultApiUrlProductDetail = "";
        private string DefaultApiUrlProductAdd = "";
        private string DefaultApiUrlProductUpdate = "";
        public ProductController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "";
            DefaultApiUrlProductList = "https://localhost:7255/api/Product";
            DefaultApiUrlProductDetail = "https://localhost:7255/api/Product/ProductID";
            DefaultApiUrlProductAdd = "https://localhost:7255/api/Product/Add";
            DefaultApiUrlProductUpdate = "https://localhost:7255/api/Product/Update?proId=";
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
        public async Task<IActionResult> Add([FromForm] ProductDTO pro, IFormFile image)
        {
            try
            {
                if (ModelState.IsValid) // Kiểm tra xem biểu mẫu có hợp lệ không
                {
                    if (image != null && image.Length > 0)
                    {
                        // Xử lý và lưu trữ ảnh
                        Console.WriteLine(image);
                        pro.Prictue = "/img/" + image.FileName.ToString();
                    }

                    var json = JsonConvert.SerializeObject(pro);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Gửi dữ liệu lên máy chủ
                    HttpResponseMessage response = await client.PostAsync(DefaultApiUrlProductAdd, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Thêm dịch vụ thành công!";
                        return View(pro); // Chuyển hướng đến trang thành công hoặc trang danh sách
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Thêm dịch vụ thất bại. Vui lòng thử lại sau.";
                        return View(pro); // Hiển thị lại biểu mẫu với dữ liệu đã điền
                    }
                }
                else
                {
                    return View(pro);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return View(pro); // Hiển thị lại biểu mẫu với dữ liệu đã điền
            }
        }
    }
}
