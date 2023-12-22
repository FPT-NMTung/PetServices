using FEPetServices.Areas.Admin.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetServices.DTO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace FEPetServices.Areas.Admin.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "AdminOnly")]
    public class AccountController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        //private string ApiUrlAccountList = "";
        //private string ApiUrlAddAccount = "";
        //private string ApiUrlUpdateAccount = "";
        private readonly IConfiguration configuration;

        public AccountController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //ApiUrlAccountList = "https://pet-service-api.azurewebsites.net/api/Admin/GetAllAccountByAdmin";
            //ApiUrlAddAccount = "https://pet-service-api.azurewebsites.net/api/Admin/AddAccount";
            //ApiUrlUpdateAccount = "https://pet-service-api.azurewebsites.net/api/UpdateAccount";
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Danh sách tài khoản";
            var accountList = await client.GetAsync(DefaultApiUrl + "Admin/GetAllAccountByAdmin");
            //var accountList = await client.GetAsync(ApiUrlAccountList);
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
                ViewBag.Title = "Thêm tài khoản mới";
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(addAccount);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    string password = HttpUtility.UrlEncode(addAccount.Password);

                    DefaultApiUrl = DefaultApiUrl + "Admin/AddAccount" + "?email=" + addAccount.Email + "&password=" + password + "&roleId=" + addAccount.RoleId;
                    //ApiUrlAddAccount = ApiUrlAddAccount + "?email=" + addAccount.Email + "&password=" + password + "&roleId=" + addAccount.RoleId;

                    HttpResponseMessage response = await client.PostAsync(DefaultApiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessToast"] = "Thêm tài khoản thành công!";

                        return View(addAccount);
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        errorMessage = RemoveUnwantedCharacters(errorMessage);
                        TempData["WatingToast"] = errorMessage;

                        ViewBag.Email = addAccount.Email;
                        ViewBag.Password = addAccount.Password;
                        ViewBag.RoleId = addAccount.RoleId;

                        return View(addAccount);
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        TempData["ErrorToast"] = "Thêm tài khoản thất bại!";

                        ViewBag.Email = addAccount.Email;
                        ViewBag.Password = addAccount.Password;
                        ViewBag.RoleId = addAccount.RoleId;

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
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(addAccount);
            }
        }

        public async Task<IActionResult> UpdateAccount(string email, int roleId, bool status)
        {
            try
            {
                ViewBag.Title = "";
                if (ModelState.IsValid)
                {
                    string apiUrl = DefaultApiUrl + "Admin/UpdateAccount";
                    //string apiUrl = ApiUrlUpdateAccount + "?Email=" + email + "&RoleId=" + roleId + "&Status=" + status;
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
                        TempData["SuccessToast"] = response.Content.ReadAsStringAsync();

                        return Json(new
                        {
                            Success = true
                        });
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        TempData["WatingToast"] = errorMessage;
                        return Json(new
                        {
                            Success = false
                        });
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        TempData["ErrorToast"] = errorMessage;
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
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
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
