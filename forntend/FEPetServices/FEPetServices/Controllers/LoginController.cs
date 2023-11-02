using FEPetServices.Form;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace FEPetServices.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";

        public LoginController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "https://localhost:7255/api/Account";
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Index1()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm, Bind("Email", "Password")] LoginForm loginInfo)
        {
            try
            {
                var json = JsonConvert.SerializeObject(loginInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(DefaultApiUrl + "/Login", content); // Hãy đảm bảo rằng URL API của bạn là chính xác

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

                    if (loginResponse.Successful)
                    {                       
                        var roleName = loginResponse.RoleName;
                        if(loginResponse.Status != true)
                        {
                            ViewBag.ErrorToast = "Tài khoản chưa được kích hoạt";
                            return View();
                        }

                        if (!string.IsNullOrEmpty(roleName))
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, loginInfo.Email),
                                new Claim(ClaimTypes.Role, roleName)
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                            HttpContext.Session.SetString("UserName", loginResponse.UserName);
                            HttpContext.Session.SetString("UserImage", loginResponse.UserImage);
                            // Redirect based on the role
                            if (roleName == "MANAGER")
                            {
                                TempData["SuccessLoginToast"] = "Đăng nhập thành công.";
                                return RedirectToAction("Index", "Information", new { area = "Manager" });
                            }
                            else if (roleName == "CUSTOMER")
                            {
                                TempData["SuccessLoginToast"] = "Đăng nhập thành công.";
                                return RedirectToAction("Index", "Home");
                            }
                            else if (roleName == "PARTNER")
                            {
                                TempData["SuccessLoginToast"] = "Đăng nhập thành công.";
                                return RedirectToAction("Index", "Information", new { area = "Partner" });
                            }
                        }
                        else
                        {
                            ViewBag.ErrorToast = "Đăng nhập không thành công. Tài khoản không có vai trò được xác định.";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.ErrorToast = "Đăng nhập không thành công.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrorToast = "Đăng nhập không thành công. Tài khoản hoặc mật khẩu không chính xác";
                    return View();
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorToast = "Đã xảy ra lỗi: " + ex.Message;
                return View();
            }
        }
    }
}
