using FEPetServices.Form;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FEPetServices.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient client = null;
        private readonly IConfiguration configuration;
        private string DefaultApiUrl = "";

        public LoginController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Đăng nhập";
            return View();
        }

            [HttpPost]
            public async Task<IActionResult> Index([FromForm, Bind("Email", "Password")] LoginForm loginInfo, string returnUrl)
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
                                    new Claim(ClaimTypes.Role, roleName),
                                    new Claim("AccessToken", loginResponse.Token) 
                                };

                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                                var accessToken = ((ClaimsIdentity)User.Identity).FindFirst("AccessToken")?.Value;

                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                                HttpContext.Session.SetString("UserName", loginResponse.UserName ?? "aaa_a");
                                HttpContext.Session.SetString("UserImage", loginResponse.UserImage ?? "aaa_a");

                                // Redirect based on the role
                                if (!string.IsNullOrEmpty(returnUrl))
                                {
                                    return LocalRedirect(returnUrl);
                                }

                                TempData["SuccessLoginToast"] = "Đăng nhập thành công.";

                                return RedirectToAction(roleName switch
                                {
                                    "MANAGER" => "Index",
                                    "CUSTOMER" => "Index",
                                    "PARTNER" => "Index",
                                    "ADMIN" => "Index",
                                    _ => "Index"
                                }, roleName switch
                                {
                                    "MANAGER" => "DashBoard",
                                    "CUSTOMER" => "Home",
                                    "PARTNER" => "DashboardPartner",
                                    "ADMIN" => "Account",
                                    _ => "Home"
                                }, roleName switch
                                {
                                    "MANAGER" => new { area = "Manager" },
                                    "PARTNER" => new { area = "Partner" },
                                    "ADMIN" => new { area = "Admin" },
                                    _ => null
                                });
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
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorToast = "Đã xảy ra lỗi: " + ex.Message;
                    return View();
                }
            }
    }
}
