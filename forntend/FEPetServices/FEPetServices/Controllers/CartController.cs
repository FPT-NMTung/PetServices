using Microsoft.AspNetCore.Mvc;
using FEPetServices.Form;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FEPetServices.Controllers
{
    public class CartController : Controller
    {
        public class CartItem
        {
            public int quantity { set; get; }
            public ProductDTO product { set; get; }
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
        public IActionResult Index()
        {
            // Assuming you have a method to get cart items
            var cartItems = GetCartItems();

            return View(cartItems); // Pass cart items to the view
        }

    }
}
