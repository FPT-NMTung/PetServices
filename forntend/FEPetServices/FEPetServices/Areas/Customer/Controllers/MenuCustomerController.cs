using Microsoft.AspNetCore.Mvc;

namespace FEPetServices.Areas.Customer.Controllers
{
    public class MenuCustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
