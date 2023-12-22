using FEPetServices.Areas.DTO;
using FEPetServices.Form;
using FEPetServices.Models.ErrorResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using PetServices.DTO;
using PetServices.Models;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Controllers
{
    public class ProductListController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;
        /*private string DefaultApiUrl = "";
        private string DefaultApiUrlProductList = "";
        private string DefaultApiUrlProductDetail = "";
        private string DefaultApiUrlProductCategoryList = "";*/

        public ProductListController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            /* DefaultApiUrlProductList = "https://pet-service-api.azurewebsites.net/api/Product";
             DefaultApiUrlProductDetail = "https://pet-service-api.azurewebsites.net/api/Product/ProductID"; 
             DefaultApiUrlProductCategoryList = "https://pet-service-api.azurewebsites.net/api/ProductCategory/GetAll";*/
        }

        public async Task<IActionResult> Index(ProductDTO productDTO, ProductSearch searchDTO)
        {
            try
            {
                ViewBag.Title = "Sản phẩm";
                 var json = JsonConvert.SerializeObject(productDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //HttpResponseMessage response = await client.GetAsync("https://localhost:7255/api/Product/GetAllProductWhenCategoryTrue");
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Product/GetAllProductWhenCategoryTrue");
                //HttpResponseMessage ProductCategoryResponse = await client.GetAsync(DefaultApiUrlProductCategoryList);
                HttpResponseMessage ProductCategoryResponse = await client.GetAsync(DefaultApiUrl + "ProductCategory/GetAll");
                if (ProductCategoryResponse.IsSuccessStatusCode)
                {
                    var categories = await ProductCategoryResponse.Content.ReadFromJsonAsync<List<ProductCategoryDTO>>();
                    categories = categories.Where(r => r.Status == true).ToList();
                    ViewBag.categories = new SelectList(categories, "ProCategoriesId", "ProCategoriesName");
                }
                if (response.IsSuccessStatusCode)
                {
                    var rep = await response.Content.ReadAsStringAsync();
                    
                    if (!string.IsNullOrEmpty(rep))
                    {
                        var productList = JsonConvert.DeserializeObject<List<ProductDTO>>(rep);
                        productList = productList.Where(r => r.Status == true).ToList();

                        if (!string.IsNullOrEmpty(searchDTO.productname))
                        {
                            productList = productList.Where(r => r.ProductName.Contains(searchDTO.productname, StringComparison.OrdinalIgnoreCase)).ToList();
                        }
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
                        int pagesize = searchDTO.pagesize ?? 9;

                        int totalItems = productList.Count;
                        int totalPages = (int)Math.Ceiling(totalItems / (double)pagesize);
                        int startIndex = (page - 1) * pagesize;
                        List<ProductDTO> currentPageProductList = productList.Skip(startIndex).Take(pagesize).ToList();

                        ViewBag.TotalPages = totalPages;
                        ViewBag.CurrentPage = searchDTO.page ?? 1;
                        ViewBag.PageSize = searchDTO.pagesize;

                        ViewBag.productcategory = searchDTO.productcategory;
                        ViewBag.pricefrom = searchDTO.pricefrom;
                        ViewBag.priceto = searchDTO.priceto;
                        ViewBag.sortby = searchDTO.sortby;
                        ViewBag.productname = searchDTO.productname;
                        ViewBag.pagesize = searchDTO.pagesize;
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

        
        public async Task<IActionResult> Detail(int proId, string sortby, int? page)
        {
            ProductDetailModel model = new ProductDetailModel();
            try
            {
                ViewBag.Title = "Chi tiết sản phẩm";
                //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlProductDetail + "/" + proId);
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Product/ProductID" + "/" + proId);
                //HttpResponseMessage listResponse = await client.GetAsync(DefaultApiUrlProductList + "/GetAll");
                HttpResponseMessage listResponse = await client.GetAsync(DefaultApiUrl + "Product/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    
                    if (listResponse.IsSuccessStatusCode)
                    {
                        var responseproductContent = await listResponse.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseproductContent))
                        {
                            var product = JsonConvert.DeserializeObject<List<ProductDTO>>(responseproductContent);
                            model.products = product;
                        }
                    }
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var option = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    model.Product  = System.Text.Json.JsonSerializer.Deserialize<ProductDTO>(responseContent, option);
                    //HttpResponseMessage listCateProductResponse = await client.GetAsync("https://pet-service-api.azurewebsites.net/api/Product/GetByCategory/" + model.Product.ProCategoriesId);
                    HttpResponseMessage listCateProductResponse = await client.GetAsync(DefaultApiUrl + "Product/GetByCategory/" + model.Product.ProCategoriesId);
                    if (listCateProductResponse.IsSuccessStatusCode)
                    {
                        var responseCateProductContent = await listCateProductResponse.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseCateProductContent))
                        {
                            var product = JsonConvert.DeserializeObject<List<ProductDTO>>(responseCateProductContent);
                            model.CateProduct = product;
                        }
                    }

                    //HttpResponseMessage voteNumberResponse = await client.GetAsync("https://localhost:7255/api/Feedback/GetProductVoteNumber?productID=" + proId);
                    HttpResponseMessage voteNumberResponse = await client.GetAsync(DefaultApiUrl + "Feedback/GetProductVoteNumber?productID=" + proId);

                    if (voteNumberResponse.IsSuccessStatusCode)
                    {
                        var voteNumber = await voteNumberResponse.Content.ReadFromJsonAsync<VoteNumberDTO>();

                        model.VoteNumberas = voteNumber;
                    }

                    //HttpResponseMessage productStarResponse = await client.GetAsync("https://localhost:7255/api/Feedback/GetProductStar?productID=" + proId);
                    HttpResponseMessage productStarResponse = await client.GetAsync(DefaultApiUrl + "Feedback/GetProductStar?productID=" + proId);

                    if (productStarResponse.IsSuccessStatusCode)
                    {
                        var content = await productStarResponse.Content.ReadAsStringAsync();

                        if (double.TryParse(content, out double productStar))
                        {
                            ViewBag.productStar = productStar;
                        }
                    }

                    //HttpResponseMessage feedbackResponse = await client.GetAsync("https://localhost:7255/api/Feedback/GetAllFeedbackInProduct?productID=" + proId);
                    HttpResponseMessage feedbackResponse = await client.GetAsync(DefaultApiUrl + "Feedback/GetAllFeedbackInProduct?productID=" + proId);

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

                        //HttpResponseMessage paggingResponse = await client.GetAsync("https://localhost:7255/api/Feedback/PaginationInProduct?productID=" + proId + "&starnumber=" + (sortby ?? "0") + "&pagenumber=" + page);
                        HttpResponseMessage paggingResponse = await client.GetAsync(DefaultApiUrl + "Feedback/PaginationInProduct?productID=" + proId + "&starnumber=" + (sortby ?? "0") + "&pagenumber=" + page);

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

                    return View(model);
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
        public class ProductDetailModel
        {
            public List<FeedbackDTO> Feedback { get; set; }
            public VoteNumberDTO VoteNumberas { get; set; }
            public ProductDTO Product { get; set; }
            public List<ProductDTO> products { get; set; }
            public List<ProductDTO> CateProduct { get; set; }
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
        public async Task<IActionResult> AddToCart([FromForm] int ProductId, int quantityProduct)
        {
            ProductDTO product = null;
            if(quantityProduct <= 1)
            {
                quantityProduct = 1;
            }
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlProductDetail + "/" + ProductId);
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "/Product/ProductID/" + ProductId);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var option = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                product = System.Text.Json.JsonSerializer.Deserialize<ProductDTO>(responseContent, option);
            }

            if (product.Quantity < quantityProduct)
            {
                return new ErrorResult("Số lượng đặt hàng vượt quá số lượng sản phẩm còn lại");
            }

            if (product != null)
            {
                var cart = GetCartItems();
                var cartitem = cart.Find(p => p.product != null && p.product.ProductId == ProductId);

                if (cartitem != null)
                {
                    cartitem.quantityProduct += quantityProduct;
                }
                else
                {
                    cart.Add(new CartItem() { quantityProduct = quantityProduct, product = product });
                }

                SaveCartSession(cart);
            }

            // Kiểm tra xem đây có phải là yêu cầu Ajax không
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var cartItems = GetCartItems();
                int totalQuantity = cartItems.Select(item => item?.product?.ProductId ?? 0)
                                             .Union(cartItems.Where(item => item?.service != null)
                                                             .Select(item => item.service.ServiceId))
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
    }
}
