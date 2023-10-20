using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

                HttpResponseMessage proCateResponse = await client.GetAsync("https://localhost:7255/api/ProductCategory/GetAll");
                if (proCateResponse.IsSuccessStatusCode)
                {
                    var proCategories = await proCateResponse.Content.ReadFromJsonAsync<List<ProductCategoryDTO>>();
                    ViewBag.ProCategories = new SelectList(proCategories, "ProCategoriesId", "ProCategoriesName");
                }
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
        [HttpGet]
        public async Task<IActionResult> Update(int proId)
        {
            try
            {
                //goi api de lay thong tin can sua
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrlProductDetail + "/" + proId);
                
                if (response.IsSuccessStatusCode)
                {
                    var rep = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(rep))
                    {
                        //deserialize du lieu tu api thanh ds cac doi tuongdto
                        var existPL = JsonConvert.DeserializeObject<List<ProductDTO>>(rep);
                        if(existPL.Count >0)
                        {
                            var existProduct = existPL[0];
                            HttpResponseMessage proCateResponse = await client.GetAsync("https://localhost:7255/api/ProductCategory/GetAll");
                            if (proCateResponse.IsSuccessStatusCode)
                            {
                                var proCate = await proCateResponse.Content.ReadAsStringAsync();
                                var proCategories = JsonConvert.DeserializeObject<List<ProductCategoryDTO>>(proCate);
                                //var cateSelectList = new SelectList(proCategories, "ProCategoriesId", "ProCategoriesName", existProduct.ProCategoriesId);

                                ViewBag.ProCategories = new SelectList(proCategories, "ProCategoriesId", "ProCategoriesName", existProduct.ProCategoriesId);
                                return View(existProduct);
                            }
                            else
                            {
                                ViewBag.ErrorMessage = "Tải danh sách loại sản phẩm thất bại";
                            }
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Không tìm thấy sản phẩm với ID được cung cấp.";
                        }
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
    }
}
