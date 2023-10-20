using FEPetServices.Form;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

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
                        // Đọc thông tin từ token
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.ReadJwtToken(loginResponse.Token);

                        var claim = token.Claims;
                        var roleNameClaim = claim.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

                        var claims = new List<Claim>
                        {
                           new Claim(ClaimTypes.Email, loginInfo.Email)
                        };

                        // Create claims identity
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        // Sign in the user
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        if (roleNameClaim != null)
                        {
                            var roleName = roleNameClaim.Value;

                            // Chuyển hướng dựa trên vai trò (role) của người dùng
                            if (roleName == "MANAGER")
                            {
                                // Hiển thị toast thông báo thành công và sau đó chuyển hướng
                                TempData["SuccessLoginToast"] = "Đăng nhập thành công";
                                return RedirectToAction("Index", "Information", new { area = "Manager" });
                            }
                            else if (roleName == "CUSTOMER")
                            {
                                // Hiển thị toast thông báo thành công và sau đó chuyển hướng
                                TempData["SuccessLoginToast"] = "Đăng nhập thành công";
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            ViewBag.ErrorToast = "Đăng nhập không thành công. Tài khoản chưa được cung cấp";
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
