using Microsoft.AspNetCore.Mvc;

namespace FEPetServices.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult NotFound()
        {
            //Test
            return View();
        }
    }
}
