using FEPetServices.Areas.Admin.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PetServices.DTO;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

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

                    ApiUrlAddAccount = ApiUrlAddAccount + "?email=" + addAccount.Email + "&password=" + addAccount.Password + "&roleId=" + addAccount.RoleId;

                    HttpResponseMessage response = await client.PostAsync(ApiUrlAddAccount, content);

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Success = "thêm tài khoản thành công!";
                        return View(addAccount);
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ViewBag.ErrorMessage = "Thêm tài khoản thất bại: " + errorMessage;
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

        public async Task<IActionResult> UpdateAccount([FromForm] AddAccountDTO addAccount)
        {
            return View();
        }

    }
}
