using FEPetServices.Controllers;
using FEPetServices.Form;
using FEPetServices.Form.BookingForm;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        private string DefaultApiUrlOrderListOfPetTraining = "";
        private string DefaultApiUrlOrderListOfPetTrainingSpecial = "";
        public HomePartnerController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "https://localhost:7255/api/Partner";
            DefaultApiUrlOrderListOfPetTraining = "https://localhost:7255/api/OrderPartner/ListOrderPetTraining?serCategoriesId=4";
            DefaultApiUrlOrderListOfPetTrainingSpecial = "https://localhost:7255/api/OrderPartner/ListOrderPetTrainingSpecial";
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> ListOrderPetTraining()
        {
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining);
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
        public async Task<IActionResult> ListOrderPetTrainingSpecial()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            HttpResponseMessage repId = await client.GetAsync(DefaultApiUrl + "/" + email);
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
            //https://localhost:7255/api/OrderPartner/ListOrderPetTrainingSpecial?serCategoriesId=4&partnerInfoId=1
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);

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


        //private async Task<List<OrderForm>> GetPetTrainingOrdersAsync()
        //{
        //    List<OrderForm> petTrainingOrders = new List<OrderForm>();

        //    try
        //    {
        //        // Gọi API để lấy danh sách đơn đặt hàng dựa trên pet training
        //        HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            // Chuyển đổi nội dung response thành danh sách đơn đặt hàng
        //            petTrainingOrders = await response.Content.ReadFromJsonAsync<List<OrderForm>>();
        //        }
        //        else
        //        {
        //            // Xử lý lỗi nếu cần
        //            ModelState.AddModelError(string.Empty, "Error retrieving pet training orders.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý lỗi nếu cần
        //        ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
        //    }

        //    return petTrainingOrders;
        //}
        //public List<OrderForm> GetOrdersByServiceCategory(string serviceCategory)
        //{
        //    var orders = new List<OrderForm>();
        //    // Lấy danh sách đơn hàng
        //    // ...
        //    // Lọc danh sách đơn hàng theo danh mục dịch vụ
        //    var filteredOrders = orders.Where(order => order.BookingServicesDetails.Any(service => service.Service.SerCategoriesName == serviceCategory)).ToList();
        //    return filteredOrders;
        //}
    }
}
