using Microsoft.AspNetCore.Mvc;

namespace FEPetServices.Areas.Manager.Controllers
{
    public class InformationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
