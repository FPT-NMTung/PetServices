﻿using FEPetServices.Form;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.Models;

namespace FEPetServices.Controllers
{
    public class CartController : Controller
    {
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
            var cartItems = GetCartItems();

            return View(cartItems); 
        }

        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
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

            return RedirectToAction("Index", "Cart");
        }

    }
}
