using DocumentFormat.OpenXml.Office2010.Excel;
using FEPetServices.Areas.Admin.DTO;
using FEPetServices.Controllers;
using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Areas.Partner.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class HomePartnerController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public HomePartnerController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Partner";

        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        //Waiting
        public async Task<IActionResult> ListOrderPartner()
        {
            ViewBag.Title = "Danh sách đơn hàng";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            HttpResponseMessage repId = await client.GetAsync(DefaultApiUrl + "Partner/" + email);
            AccountInfo account = null; // Initialize with null or a default value

            if (repId.IsSuccessStatusCode)
            {
                string responseAccContent = await repId.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                account = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseAccContent, options);
            }
            ViewBag.PartnerId = account.PartnerInfoId;
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPartner");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                List<OrderForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);
                orderLists = orderLists
                    .Where(order => order.BookingServicesDetails.Any(x => x.PartnerInfoId == null)).ToList();
                return View(orderLists);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
        public async Task<IActionResult> ListOrderPartnerSpecial()
        {
            ViewBag.Title = "Danh sách đơn hàng";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            HttpResponseMessage repId = await client.GetAsync(DefaultApiUrl + "Partner/" + email);
            AccountInfo account = null; // Initialize with null or a default value

            if (repId.IsSuccessStatusCode)
            {
                string responseAccContent = await repId.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                account = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseAccContent, options);
            }
            int partnerInfoId = account?.PartnerInfoId ?? 0; // Use the null-conditional operator to provide a default value

            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPartnerSpecial?partnerInfoId=" + partnerInfoId);
            
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                List<OrderForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);
                return View(orderLists);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
        //Complete
        public async Task<IActionResult> ListOrderPartnerComplete()
        {
            ViewBag.Title = "Danh sách đơn hàng";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            HttpResponseMessage repId = await client.GetAsync(DefaultApiUrl + "Partner/" + email);
            AccountInfo account = null; // Initialize with null or a default value

            if (repId.IsSuccessStatusCode)
            {
                string responseAccContent = await repId.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                account = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseAccContent, options);
            }
            int partnerInfoId = account?.PartnerInfoId ?? 0; // Use the null-conditional operator to provide a default value

            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPartnerSpecial?partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId + "&orderStatus" + orderStatus);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                List<OrderForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);
                return View(orderLists);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
        //Received
        public async Task<IActionResult> ListOrderPartnerReceived()
        {
            ViewBag.Title = "Danh sách đơn hàng";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            HttpResponseMessage repId = await client.GetAsync(DefaultApiUrl + "Partner/" + email);
            //HttpResponseMessage repId = await client.GetAsync(DefaultApiUrl + "/" + email);
            AccountInfo account = null; // Initialize with null or a default value

            if (repId.IsSuccessStatusCode)
            {
                string responseAccContent = await repId.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                account = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseAccContent, options);
            }
            int partnerInfoId = account?.PartnerInfoId ?? 0; // Use the null-conditional operator to provide a default value

            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPartnerSpecial?partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId + "&orderStatus" + orderStatus);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                List<OrderForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);
                return View(orderLists);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
        //Processing
        public async Task<IActionResult> ListOrderPartnerProcessing()
        {
            ViewBag.Title = "Danh sách đơn hàng";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            HttpResponseMessage repId = await client.GetAsync(DefaultApiUrl + "Partner/" + email);
            //HttpResponseMessage repId = await client.GetAsync(DefaultApiUrl + "/" + email);
            AccountInfo account = null; // Initialize with null or a default value

            if (repId.IsSuccessStatusCode)
            {
                string responseAccContent = await repId.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                account = System.Text.Json.JsonSerializer.Deserialize<AccountInfo>(responseAccContent, options);
            }
            int partnerInfoId = account?.PartnerInfoId ?? 0; // Use the null-conditional operator to provide a default value

            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPartnerSpecial?partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId + "&orderStatus" + orderStatus);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                List<OrderForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);
                return View(orderLists);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> OrderPartnerDetail(int orderId)
        {
            ViewBag.Title = "Chi tiết đơn hàng";
            //HttpResponseMessage reasonResponse = await client.GetAsync("https://localhost:7255/api/Reason/GetAll");
            //HttpResponseMessage reasonResponse = await client.GetAsync(DefaultApiUrl + "Reason/GetAll");
            //if (reasonResponse.IsSuccessStatusCode)
            //{
            //    var reaCategories = await reasonResponse.Content.ReadFromJsonAsync<List<ReasonDTO>>();
            //    ViewBag.Reasons = new SelectList(reaCategories, "ReasonId", "ReasonTitle");
            //}
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/" + orderId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderPartner + "/" + orderId);
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

                //TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
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
        public async Task<IActionResult> OrderPartnerDetail(int orderId, [FromForm] Status status, [FromForm] ReasonOrdersForm reasonOrders)
        {
            ViewBag.Title = "Chi tiết đơn hàng";
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            if (status.newStatus == "Waiting")
            {
                if (string.IsNullOrWhiteSpace(reasonOrders.ReasonOrderTitle) || string.IsNullOrWhiteSpace(reasonOrders.ReasonOrderDescription))
                {
                    TempData["ErrorToast"] = "Vui lòng nhập tiêu đề và mô tả trước khi cập nhật.";
                    return RedirectToAction("OrderPartnerDetail", new { orderId = orderId });
                }
                status.newStatusProduct = "";
                status.newStatusService = "Waiting";
            }
            if (status.newStatus == "Confirmed")
            {
                status.newStatusProduct = "";
                status.newStatusService = "Received";
            }
            if (status.newStatus == "Processing")
            {
                status.newStatusProduct = "";
                status.newStatusService = "Processing";
            }
            if (status.newStatus == "Completed")
            {
                status.newStatusProduct = "";
                status.newStatus = "Processing";
                status.newStatusService = "Completed";
            }
            HttpResponseMessage response = await client.PutAsJsonAsync(DefaultApiUrl+ "OrderPartner/ChangeStatus/" + email + "?orderId=" + orderId, status);
            reasonOrders.OrderId = orderId;
            
            reasonOrders.EmailReject = email;
            
            HttpResponseMessage responseReject = await client.PostAsJsonAsync(DefaultApiUrl + "ReasonOrder", reasonOrders);


            if (response.IsSuccessStatusCode || responseReject.IsSuccessStatusCode)
            {
                TempData["SuccessToast"] = "Cập nhật thành công";
                return RedirectToAction("OrderPartnerDetail", new { orderId = orderId });
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
    }
}
