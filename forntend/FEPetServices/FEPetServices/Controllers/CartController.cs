using FEPetServices.Form;
using FEPetServices.Models.ErrorResult;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public CartController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            /*DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Product";*/
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
        public IActionResult Index()
        {
            ViewBag.Title = "Giỏ hàng";
            var cartItems = GetCartItems();
            return View(cartItems); 
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            ProductDTO product = null;
            // Cập nhật Cart thay đổi số lượng quantity ...
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Product/ProductID/" + productid);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var option = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                product = System.Text.Json.JsonSerializer.Deserialize<ProductDTO>(responseContent, option);
            }

            if (product.Quantity < quantity)
            {
                return new ErrorResult("Số lượng đặt hàng vượt quá số lượng sản phẩm còn lại");
            }

            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantityProduct = quantity;
            }
            SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }

        [HttpPost]
        public IActionResult RemoveCart([FromForm] int productid, [FromForm] int serviceid)
        {
            var cart = GetCartItems();

            if (productid > 0)
            {
                var productCartItem = cart.Find(p => p.product != null && p.product.ProductId == productid);

                if (productCartItem != null)
                {
                    cart.Remove(productCartItem);
                    SaveCartSession(cart);
                }
            }

            if (serviceid > 0)
            {
                var serviceCartItem = cart.Find(s => s.service != null && s.service.ServiceId == serviceid);

                if (serviceCartItem != null)
                {
                    cart.Remove(serviceCartItem);
                    SaveCartSession(cart);
                }
            }

            if(cart.Count() == 0)
            {
                ClearCart();
            }

            return RedirectToAction("Index", "Cart");
        }

    }
}
