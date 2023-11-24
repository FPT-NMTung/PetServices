using Microsoft.AspNetCore.Mvc;

namespace FEPetServices.Areas.Manager.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
