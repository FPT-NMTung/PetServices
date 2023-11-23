using FEPetServices.Areas.Admin.DTO;
using FEPetServices.Controllers;
using FEPetServices.Form;
using FEPetServices.Form.OrdersForm;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Areas.Partner.Controllers
{
    public class HomePartnerController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        //private string DefaultApiUrlOrderPartner = "";
        //private string DefaultApiUrlOrderListOfPetTraining = "";
        private string DefaultApiUrlOrderListOfPetTrainingSpecial = "";
        private readonly IConfiguration configuration;

        public HomePartnerController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Partner";
            //DefaultApiUrlOrderPartner = "https://pet-service-api.azurewebsites.net/api/OrderPartner";
            //DefaultApiUrlOrderListOfPetTraining = "https://pet-service-api.azurewebsites.net/api/OrderPartner/ListOrderPetTraining?serCategoriesId=4";
            //DefaultApiUrlOrderListOfPetTrainingSpecial = "https://pet-service-api.azurewebsites.net/api/OrderPartner/ListOrderPetTrainingSpecial";
            
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> ListOrderPetTraining()
        {
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
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPetTraining?serCategoriesId=4");
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining + "&orderStatus" + orderStatus);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
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
        public async Task<IActionResult> ListOrderPetTrainingSpecial()
        {
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
            int serCategoriesId = 4;
            int partnerInfoId = account?.PartnerInfoId ?? 0; // Use the null-conditional operator to provide a default value

            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPetTrainingSpecial?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId + "&orderStatus" + orderStatus);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
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
        //public async Task<IActionResult> ListOrderPetTrainingComplete()
        //{
        //    //orderStatus = "Waiting";
        //    //https://pet-service-api.azurewebsites.net/api/OrderPartner/ListOrderPetTraining?serCategoriesId=4&orderStatus=Waiting
        //    HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPetTraining?serCategoriesId=4");
        //    //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining);
        //    //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining + "&orderStatus" + orderStatus);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string responseContent = await response.Content.ReadAsStringAsync();

        //        var options = new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true
        //        };

        //        TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
        //        List<OrderForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);
        //        return View(orderLists);
        //    }
        //    else
        //    {
        //        TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
        //        return View();
        //    }
        //}
        public async Task<IActionResult> ListOrderPetTrainingSpecialComplete()
        {
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
            int serCategoriesId = 4;
            int partnerInfoId = account?.PartnerInfoId ?? 0; // Use the null-conditional operator to provide a default value

            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPetTrainingSpecial?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId + "&orderStatus" + orderStatus);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                List<OrderForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);
                return View(orderLists);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
        //Reject
        public async Task<IActionResult> ListOrderPetTrainingReject()
        {
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPetTraining?serCategoriesId=4");
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining + "&orderStatus" + orderStatus);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                List<OrderForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);
                return View(orderLists);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
        public async Task<IActionResult> ListOrderPetTrainingSpecialReject()
        {
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
            int serCategoriesId = 4;
            int partnerInfoId = account?.PartnerInfoId ?? 0; // Use the null-conditional operator to provide a default value

            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPetTrainingSpecial?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId + "&orderStatus" + orderStatus);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
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
        //public async Task<IActionResult> ListOrderPetTrainingReceived()
        //{
        //    HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPetTraining?serCategoriesId=4");
        //    //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining);
        //    //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining + "&orderStatus" + orderStatus);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string responseContent = await response.Content.ReadAsStringAsync();

        //        var options = new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true
        //        };

        //        TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
        //        List<OrderForm> orderLists = System.Text.Json.JsonSerializer.Deserialize<List<OrderForm>>(responseContent, options);
        //        return View(orderLists);
        //    }
        //    else
        //    {
        //        TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
        //        return View();
        //    }
        //}
        public async Task<IActionResult> ListOrderPetTrainingSpecialReceived()
        {
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
            int serCategoriesId = 4;
            int partnerInfoId = account?.PartnerInfoId ?? 0; // Use the null-conditional operator to provide a default value

            HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "OrderPartner/ListOrderPetTrainingSpecial?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
            //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId + "&orderStatus" + orderStatus);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
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

                TempData["SuccessLoadingDataToast"] = "Lấy dữ liệu thành công";
                ViewBag.TotalPrice = totalPrice;
                return View(orderDetail);
            }
            else
            {
                TempData["ErrorLoadingDataToast"] = "Lỗi hệ thống vui lòng thử lại sau";
                return View();
            }
        }
    }
}
