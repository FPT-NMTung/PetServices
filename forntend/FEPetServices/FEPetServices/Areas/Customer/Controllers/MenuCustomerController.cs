using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FEPetServices.Areas.Customer.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "CusOnly")]
    public class MenuCustomerController : Controller
    {
        private readonly HttpClient _client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public MenuCustomerController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/UserInfo";
            //DefaultApiUrlPet = "https://pet-service-api.azurewebsites.net/api/PetInfo";
            //DefaultApiUrlOrders = "https://localhost:7255/api/Order";
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Information()
        {
            ViewBag.Title = "Thông tin người dùng";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "UserInfo/" + email);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                AccountInfo userInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);

                return View(userInfos);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Information([FromForm] UserInfo userInfo, IFormFile image)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            if (userInfo.Phone == null)
            {
                TempData["ErrorToast"] = "Số điện thoại không được để trống";
                return RedirectToAction("Information");
            }
            if (userInfo.Phone.Length != 10 && !userInfo.Phone.StartsWith("0"))
            {
                TempData["ErrorToast"] = "Số điện thoại phải bắt đầu bằng số 0 và có 10 chữ số";
                return RedirectToAction("Information");
            }
            if (userInfo.Dob.HasValue && userInfo.Dob.Value > DateTime.Now)
            {
                TempData["ErrorToast"] = "Ngày sinh không thể lớn hơn ngày hiện tại";
                return RedirectToAction("Information");
            }
            if (userInfo.Address == null)
            {
                TempData["ErrorToast"] = "Địa chỉ cụ thể không được để trống";
                return RedirectToAction("Information");
            }
            if (userInfo.Address.Length <= 10)
            {
                TempData["ErrorToast"] = "Địa chỉ cụ thể phải ít nhât 10 ký tự";
                return RedirectToAction("Information");
            }

            if (image != null)
            {
                string filename = GenerateRandomNumber(5) + image.FileName;
                filename = Path.GetFileName(filename);
                string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/", filename);
                using (var stream = new FileStream(uploadfile, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                userInfo.ImageUser = "/img/" + filename;
            }
            else
            {
                HttpResponseMessage responseUser = await _client.GetAsync(DefaultApiUrl + "UserInfo/" + email);
                if (responseUser.IsSuccessStatusCode)
                {
                    string responseContent = await responseUser.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    AccountInfo managerInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                    userInfo.ImageUser = managerInfos.UserInfo.ImageUser;
                }
            }

            if (userInfo.Province == null ||
                userInfo.District == null || userInfo.Commune == null)
            {
                HttpResponseMessage responseUser = await _client.GetAsync(DefaultApiUrl + "UserInfo/" + email);
                if (responseUser.IsSuccessStatusCode)
                {
                    string responseContent = await responseUser.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    AccountInfo managerInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                    userInfo.Province = managerInfos.UserInfo.Province;
                    userInfo.District = managerInfos.UserInfo.District;
                    userInfo.Commune = managerInfos.UserInfo.Province;
                }
            }

            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrl + "UserInfo/updateInfo?email=" + email, userInfo);
            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.SetString("UserImage", userInfo.ImageUser);
                TempData["SuccessToast"] = "Cập nhật thông tin thành công";
                return RedirectToAction("Information");
            }
            else
            {
                TempData["ErrorToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return RedirectToAction("Information");
            }
        }

        public static string GenerateRandomNumber(int length)
        {
            Random random = new Random();
            const string chars = "0123456789"; // Chuỗi chứa các chữ số từ 0 đến 9
            char[] randomChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomChars);
        }

        public async Task<IActionResult> ChangePassword([FromForm] ChangePassword changePassword)
        {
            ViewBag.Title = "Đổi mật khẩu";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            if (changePassword.OldPassword == null || changePassword.NewPassword == null)
            {
                return View();
            }

            if (changePassword.NewPassword != changePassword.ConfirmNewPassword)
            {
                ViewBag.ErrorToast = "Mật khẩu mới và xác nhận lại mật khẩu không trùng khớp";
                return View();
            }

            else if (changePassword.NewPassword.Length < 8)
            {
                ViewBag.ErrorToast = "Mật khẩu mới phải có ít nhất 8 ký tự";
                return View();
            }

            string apiUrl = $"https://pet-service-api.azurewebsites.net/api/Account/newpassword?email={email}&oldpassword={changePassword.OldPassword}&newpassword={changePassword.NewPassword}&confirmnewpassword={changePassword.ConfirmNewPassword}";

            HttpResponseMessage response = await _client.PutAsync(apiUrl, null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessToast"] = "Đổi mật khẩu thành công";
                return View();
            }
            else
            {
                TempData["ErrorToast"] = "Mật khẩu cũ không chính xác";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> PetInfo( int petId)
        {
            
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            ViewBag.Title = "Thông tin thú cưng";
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl+ "PetInfo/" + email);
         
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                AccountInfo petInfos = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseContent, options);
                return View(petInfos);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int id)
        {
            ViewBag.Title = "Chi tiết đơn hàng";
            HttpResponseMessage response = await _client.GetAsync(DefaultApiUrl + "Order/" + id);
            //HttpResponseMessage response = await _client.GetAsync(DefaultApiUrlOrders + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                OrderForm orderDetail = System.Text.Json.JsonSerializer.Deserialize<OrderForm>(responseContent, options);
                double totalPrice = 0;
                foreach (var od in orderDetail.OrderProductDetails)
                {
                    totalPrice = (double)(totalPrice + od.Price * od.Quantity);
                }

                ViewBag.TotalPrice = totalPrice;
                return View(orderDetail);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> OrderDetail(int id, [FromForm] Status status)
        {
            //https://pet-service-api.azurewebsites.net/api/Order/changeStatus?Id=1
            HttpResponseMessage response = await _client.PutAsJsonAsync(DefaultApiUrl + "UserInfo/changeStatus?Id=" + id, status);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessToast"] = "Cập nhật thành công";
                return RedirectToAction("OrderDetail", new { id = id });
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePet(int petId)
        {
            try
            {
                // Send a DELETE request to the API endpoint with the petId
                HttpResponseMessage response = await _client.DeleteAsync("https://localhost:7255/api/PetInfo/Delete?petId=" + petId);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Xóa thông tin thú cưng thành công!";
                    return RedirectToAction("PetInfo");
                }
                else
                {
                    TempData["ErrorToast"] = "Xóa thông tin thú cưng thất bại. Vui lòng thử lại sau.";
                    return RedirectToAction("PetInfo");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
            }

            // Redirect to the PetInfo page after deletion
            return RedirectToAction("PetInfo");
        }

    }
}
