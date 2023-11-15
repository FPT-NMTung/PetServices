using Microsoft.AspNetCore.Mvc;

namespace FEPetServices.Controllers
{
    public class CheckoutRoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
