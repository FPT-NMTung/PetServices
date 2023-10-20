using FEPetServices.Areas.Admin.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PetServices.DTO;
using PetServices.Models;
using System;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace FEPetServices.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private string ApiUrlAccountList = "";
        private string ApiUrlAddAccount = "";
        private string ApiUrlUpdateAccount = "";

        public AccountController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "";
            ApiUrlAccountList = "https://localhost:7255/api/Admin/GetAllAccountByAdmin";
            ApiUrlAddAccount = "https://localhost:7255/api/Admin/AddAccount";
            ApiUrlUpdateAccount = "https://localhost:7255/api/Admin/UpdateAccount";
        }

        public async Task<IActionResult> Index()
        {
            var accountList = await client.GetAsync(ApiUrlAccountList);
            if (accountList.IsSuccessStatusCode)
            {
                var responseContent = await accountList.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseContent))
                {
                    var listAccount = JsonConvert.DeserializeObject<List<AccountByAdminDTO>>(responseContent);

                    return View(listAccount);
                }
            }

            return View();
        }

        public async Task<IActionResult> AddAccount([FromForm] AddAccountDTO addAccount)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(addAccount);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    string password = HttpUtility.UrlEncode(addAccount.Password);

                    ApiUrlAddAccount = ApiUrlAddAccount + "?email=" + addAccount.Email + "&password=" + password + "&roleId=" + addAccount.RoleId;

                    HttpResponseMessage response = await client.PostAsync(ApiUrlAddAccount, content);

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Success = "Thêm tài khoản thành công!";

                        return View(addAccount);
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        errorMessage = RemoveUnwantedCharacters(errorMessage);
                        ViewBag.ErrorMessage = errorMessage;
                        return View(addAccount);
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ViewBag.ErrorMessage = "Thêm tài khoản thất bại!";
                        return View(addAccount);
                    }
                }
                else
                {
                    return View(addAccount);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return View(addAccount);
            }
        }

        public async Task<IActionResult> UpdateAccount(string email, int roleId, bool status)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string apiUrl = ApiUrlUpdateAccount + "?Email=" + email + "&RoleId=" + roleId + "&Status=" + status;
                    var acc = new UpdateAccountDTO
                    {
                        Email = email,
                        RoleId = roleId,
                        Status = status
                    };

                    var json = JsonConvert.SerializeObject(acc);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsJsonAsync(apiUrl, content);


                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Success = "Cập nhật tài khoản thành công!";
                        Debug.WriteLine(response.Content);
                        return Json(new
                        {
                            Success = true
                        }); 
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ViewBag.ErrorMessage = errorMessage;
                        return Json(new
                        {
                            Success = false
                        });
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ViewBag.ErrorMessage = "Cập nhật tài khoản thất bại!";
                        return Json(new
                        {
                            Success = false
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Success = false
                    });
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return View();
            }
        }

        private string RemoveUnwantedCharacters(string input)
        {
            string[] unwantedCharacters = { "[", "{", "\"", "}", "]" };
            foreach (var character in unwantedCharacters)
            {
                input = input.Replace(character, string.Empty);
            }

            return input;
        }
    }
}
