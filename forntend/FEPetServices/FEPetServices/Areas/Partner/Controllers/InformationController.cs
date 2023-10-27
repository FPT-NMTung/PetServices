using Microsoft.AspNetCore.Mvc;

namespace FEPetServices.Areas.Partner.Controllers
{
    public class InformationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
