using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace FEPetServices.Controllers
{
    public class LogoutController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                HttpContext.Session.Remove("UserName");
                HttpContext.Session.Remove("UserImage");
                HttpContext.Session.Clear();

            }

            return RedirectToAction("Index", "Home");
        }
    }
}
