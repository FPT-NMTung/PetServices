using Microsoft.AspNetCore.Mvc;

namespace FEPetServices.Areas.Partner.Controllers
{
    public class HomePartnerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
