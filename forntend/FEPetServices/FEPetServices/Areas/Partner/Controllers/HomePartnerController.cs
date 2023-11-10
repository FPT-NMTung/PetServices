using FEPetServices.Controllers;
using FEPetServices.Form.BookingForm;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

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
            DefaultApiUrl = "";
            DefaultApiUrlOrderListOfPetTraining = "";
            DefaultApiUrlOrderListOfPetTrainingSpecial = "";
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        //public async Task<IActionResult> ListOrderPetTraining()
        //{
        //    // Gọi API để lấy danh sách đơn đặt hàng pet training
        //    List<OrderForm> petTrainingOrders = await GetPetTrainingOrdersAsync();

        //    // Truyền danh sách đơn đặt hàng vào view
        //    return View(petTrainingOrders);
        //}

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
