using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using FEPetServices.Form;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Http.Headers;

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

        [HttpPost]
        public async Task<IActionResult> Index([FromForm, Bind("Email", "Password")] LoginForm loginInfo)
        {
            try
            {
                var json = JsonConvert.SerializeObject(loginInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(DefaultApiUrl + "/Login", content); // Đảm bảo URL của API đúng

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

                    if (loginResponse.Successful)
                    {
                        // Đọc thông tin từ token
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.ReadJwtToken(loginResponse.Token);

                        var claims = token.Claims;
                        var roleNameClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

                        if (roleNameClaim != null)
                        {
                            var roleName = roleNameClaim.Value;

                            // Chuyển hướng dựa trên vai trò (role) của người dùng
                            if (roleName == "MANAGER")
                            {
                                return RedirectToAction("Index", "Information", new { area = "Manager" }); // Chuyển trang đến trang quản lý
                            }
                            else if (roleName == "CUSTOMER")
                            {
                                return RedirectToAction("Index", "Home"); // Chuyển trang đến trang chính của người dùng
                            }
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Token không chứa thông tin vai trò (role).";
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Đăng nhập không thành công.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Đăng nhập không thành công. Mã lỗi HTTP: " + (int)response.StatusCode;
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return View();
            }
        }
    }
}
