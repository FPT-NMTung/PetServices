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
        private readonly IConfiguration configuration;

        public LoginController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            /*DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Account";*/
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm, Bind("Email", "Password")] LoginForm loginInfo,string returnUrl)
        {
            try
            {
                var json = JsonConvert.SerializeObject(loginInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(DefaultApiUrl + "Account/Login", content); 

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

                    if (loginResponse.Successful)
                    {
                        var roleName = loginResponse.RoleName;
                        if (loginResponse.Status != true)
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

                            HttpContext.Session.SetString("UserName", loginResponse.UserName == null ? "aaa_a" : loginResponse.UserName);
                            HttpContext.Session.SetString("UserImage", loginResponse.UserImage == null ? "aaa_a" : loginResponse.UserImage);
                            // Redirect based on the role

                            if (roleName == "MANAGER")
                            {
                                if (!string.IsNullOrEmpty(returnUrl))
                                {
                                    return LocalRedirect(returnUrl);
                                }
                                TempData["SuccessLoginToast"] = "Đăng nhập thành công.";
                                return RedirectToAction("Index", "DashBoard", new { area = "Manager" });
                            }
                            else if (roleName == "CUSTOMER")
                            {
                                if (!string.IsNullOrEmpty(returnUrl))
                                {
                                    return LocalRedirect(returnUrl);
                                }
                                TempData["SuccessLoginToast"] = "Đăng nhập thành công.";
                                return RedirectToAction("Index", "Home");
                            }
                            else if (roleName == "PARTNER")
                            {
                                if (!string.IsNullOrEmpty(returnUrl))
                                {
                                    return LocalRedirect(returnUrl);
                                }
                                TempData["SuccessLoginToast"] = "Đăng nhập thành công.";
                                return RedirectToAction("Index", "DashboardPartner", new { area = "Partner" });
                            }
                            else if (roleName == "ADMIN")
                            {
                                if (!string.IsNullOrEmpty(returnUrl))
                                {
                                    return LocalRedirect(returnUrl);
                                }
                                TempData["SuccessLoginToast"] = "Đăng nhập thành công.";
                                return RedirectToAction("Index", "Account", new { area = "Admin" });
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
                    ViewBag.ErrorToast = "Tài khoản mật khẩu không chính xác hoặc lỗi hệ thống vui lòng thử lại sau";
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