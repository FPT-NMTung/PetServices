using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetServices.Form;
using System.Net.Http.Headers;

namespace FEPetServices.Areas.Manager.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "ManaOnly")]
    public class DashBoardController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public DashBoardController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);

            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");

        }

        public async Task<IActionResult> Index()
        {

            DashBoard dashboard = new DashBoard
            {
                FeedbackRoom = new List<FeedbackForm>(),
                FeedbackService = new List<FeedbackForm>(),
                FeedbackProduct = new List<FeedbackForm>()
            };
            try
            {
                ViewBag.Title = "Thống kê doanh số";
                // số khách hàng mới trong tháng 
                HttpResponseMessage NumberCustomerInMonthResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetNumberCustomerInMonth");

                if (NumberCustomerInMonthResponse.IsSuccessStatusCode)
                {
                    var NumberCustomerInMonth = await NumberCustomerInMonthResponse.Content.ReadFromJsonAsync<int>();
                    ViewBag.NumberCustomerInMonth = NumberCustomerInMonth;
                }

                // % khách hàng mới trong tháng so với tháng trước 
                HttpResponseMessage PercentCustomerPreviousMonthResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetPercentCustomerPreviousMonth");

                if (PercentCustomerPreviousMonthResponse.IsSuccessStatusCode)
                {
                    var PercentCustomerPreviousMonth = await PercentCustomerPreviousMonthResponse.Content.ReadFromJsonAsync<double>();
                    ViewBag.PercentCustomerPreviousMonth = PercentCustomerPreviousMonth;
                }

                // số đơn hàng trong tháng
                HttpResponseMessage NumberOrderInMonthResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetNumberOrderInMonth");

                if (NumberOrderInMonthResponse.IsSuccessStatusCode)
                {
                    var NumberOrderInMonth = await NumberOrderInMonthResponse.Content.ReadFromJsonAsync<int>();
                    ViewBag.NumberOrderInMonth = NumberOrderInMonth;
                }

                // % số đơn hàng trong tháng so với tháng trước
                HttpResponseMessage PercentOrderPreviousMonthResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetPercentOrderPreviousMonth");

                if (PercentOrderPreviousMonthResponse.IsSuccessStatusCode)
                {
                    var PercentOrderPreviousMonth = await PercentOrderPreviousMonthResponse.Content.ReadFromJsonAsync<double>();
                    ViewBag.PercentOrderPreviousMonth = PercentOrderPreviousMonth;
                }

                // tổng thu nhập trong tháng
                HttpResponseMessage IncomeInMonthResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetIncomeInMonth");

                if (IncomeInMonthResponse.IsSuccessStatusCode)
                {
                    var IncomeInMonth = await IncomeInMonthResponse.Content.ReadFromJsonAsync<int>();

                    ViewBag.IncomeInMonth = IncomeInMonth;
                }

                // % tổng thu nhập trong tháng so với tháng trước
                HttpResponseMessage PercentIncomePreviousMonthResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetPercentIncomePreviousMonth");

                if (PercentIncomePreviousMonthResponse.IsSuccessStatusCode)
                {
                    var PercentIncomePreviousMonth = await PercentIncomePreviousMonthResponse.Content.ReadFromJsonAsync<double>();
                    ViewBag.PercentIncomePreviousMonth = PercentIncomePreviousMonth;
                }

                // số lượng sản phẩm bán trong tháng
                HttpResponseMessage NumberProductInMonthResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetNumberProductInMonth");

                if (NumberProductInMonthResponse.IsSuccessStatusCode)
                {
                    var NumberProductInMonth = await NumberProductInMonthResponse.Content.ReadFromJsonAsync<int>();
                    ViewBag.NumberProductInMonth = NumberProductInMonth;
                }

                // % số lượng sản phẩm bán trong tháng so với tháng trước
                HttpResponseMessage PercentNumberProductPreviousMonthResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetPercentNumberProductPreviousMonth");

                if (PercentNumberProductPreviousMonthResponse.IsSuccessStatusCode)
                {
                    var PercentNumberProductPreviousMonth = await PercentNumberProductPreviousMonthResponse.Content.ReadFromJsonAsync<double>();
                    ViewBag.PercentNumberProductPreviousMonth = PercentNumberProductPreviousMonth;
                }

                // Doanh số service theo ngày
                HttpResponseMessage TotalPriceServiceIn7DayResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetTotalPriceServiceIn7Day");

                if (TotalPriceServiceIn7DayResponse.IsSuccessStatusCode)
                {
                    var TotalPriceServiceIn7Day = await TotalPriceServiceIn7DayResponse.Content.ReadFromJsonAsync<List<ReceiveInDayForm>>();
                    ViewBag.TotalPriceServiceIn7Day = new SelectList(TotalPriceServiceIn7Day, "Date", "Receive");
                }

                // Doanh số product theo ngày
                HttpResponseMessage TotalPriceProductIn7DayResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetTotalPriceProductIn7Day");

                if (TotalPriceServiceIn7DayResponse.IsSuccessStatusCode)
                {
                    var TotalPriceProductIn7Day = await TotalPriceProductIn7DayResponse.Content.ReadFromJsonAsync<List<ReceiveInDayForm>>();
                    ViewBag.TotalPriceProductIn7Day = new SelectList(TotalPriceProductIn7Day, "Date", "Receive");
                }

                // Doanh số room theo ngày
                HttpResponseMessage TotalPriceRoomIn7DayResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetTotalPriceRoomIn7Day");

                if (TotalPriceRoomIn7DayResponse.IsSuccessStatusCode)
                {
                    var TotalPriceRoomIn7Day = await TotalPriceRoomIn7DayResponse.Content.ReadFromJsonAsync<List<ReceiveInDayForm>>();
                    ViewBag.TotalPriceRoomIn7Day = new SelectList(TotalPriceRoomIn7Day, "Date", "Receive");
                }

                // Số đơn hàng hoàn thành trong tháng 
                HttpResponseMessage NumberOrderCompleteInMonthResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetNumberOrderCompleteInMonth");

                if (NumberOrderCompleteInMonthResponse.IsSuccessStatusCode)
                {
                    var NumberOrderCompleteInMonth = await NumberOrderCompleteInMonthResponse.Content.ReadFromJsonAsync<List<Quantity_RatioForm>>();
                    ViewBag.NumberOrderCompleteInMonth = new SelectList(NumberOrderCompleteInMonth, "date", "quantity");
                    ViewBag.NumberOrderCompleteInMonth1 = new SelectList(NumberOrderCompleteInMonth,"quantity", "Ratio");
                }

                // Số đơn hàng bị hủy trong tháng
                HttpResponseMessage NumberOrderRejectedInMonthResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetNumberOrderCancelledInMonth");

                if (NumberOrderRejectedInMonthResponse.IsSuccessStatusCode)
                {
                    var NumberOrderRejectedInMonth = await NumberOrderRejectedInMonthResponse.Content.ReadFromJsonAsync<List<Quantity_RatioForm>>();
                    ViewBag.NumberOrderRejectedInMonth = new SelectList(NumberOrderRejectedInMonth, "date", "quantity");
                    ViewBag.NumberOrderRejectedInMonth1 = new SelectList(NumberOrderRejectedInMonth, "quantity", "Ratio");
                }

                // Top 5 phòng được book nhiều nhất trong tháng 
                HttpResponseMessage Top5RoomBookingResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetTop5RoomBooking");

                if (Top5RoomBookingResponse.IsSuccessStatusCode)
                {
                    var Top5RoomBooking = await Top5RoomBookingResponse.Content.ReadFromJsonAsync<List<Quantity_RatioForm>>();
                    ViewBag.Top5RoomBooking = new SelectList(Top5RoomBooking, "date", "quantity");
                }

                // Top 5 dịch vụ được book nhiều nhất trong tháng 
                HttpResponseMessage Top5ServiceBookingResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetTop5ServiceBooking");

                if (Top5ServiceBookingResponse.IsSuccessStatusCode)
                {
                    var Top5ServiceBooking = await Top5ServiceBookingResponse.Content.ReadFromJsonAsync<List<Quantity_RatioForm>>();
                    ViewBag.Top5ServiceBooking = new SelectList(Top5ServiceBooking, "date", "quantity");
                }

                // Top 5 sản phẩm bán chạy nhất trong tháng 
                HttpResponseMessage Top5ProductOrderResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetTop5ProductOrder");

                if (Top5ProductOrderResponse.IsSuccessStatusCode)
                {
                    var Top5ProductOrder = await Top5ProductOrderResponse.Content.ReadFromJsonAsync<List<Quantity_RatioForm>>();
                    ViewBag.Top5ProductOrder = new SelectList(Top5ProductOrder, "date", "quantity");
                }

                // Top 5 khu vực đông khách hàng nhất  
                HttpResponseMessage Top5CustomerAreaResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetTop5CustomerArea");

                if (Top5CustomerAreaResponse.IsSuccessStatusCode)
                {
                    var Top5CustomerArea = await Top5CustomerAreaResponse.Content.ReadFromJsonAsync<List<Quantity_RatioForm>>();
                    ViewBag.Top5CustomerArea = new SelectList(Top5CustomerArea, "date", "quantity");
                }


                // đánh giá của khách hàng về các phòng
                HttpResponseMessage FeedbackOfRoomResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetFeedbackOfRoom");

                if (FeedbackOfRoomResponse.IsSuccessStatusCode)
                {
                    var FeedbackOfRoom = await FeedbackOfRoomResponse.Content.ReadFromJsonAsync<List<FeedbackForm>>();

                    dashboard.FeedbackRoom = FeedbackOfRoom;
                }

                // đánh giá của khách hàng về các sản phẩm
                HttpResponseMessage FeedbackOfProductResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetFeedbackOfProduct");

                if (FeedbackOfProductResponse.IsSuccessStatusCode)
                {
                    var FeedbackOfProduct = await FeedbackOfProductResponse.Content.ReadFromJsonAsync<List<FeedbackForm>>();

                    dashboard.FeedbackProduct = FeedbackOfProduct;
                }

                // đánh giá của khách hàng về các dịch vụ
                HttpResponseMessage FeedbackOfServiceResponse = await client.GetAsync(DefaultApiUrl + "Dashboard/GetFeedbackOfService");

                if (FeedbackOfServiceResponse.IsSuccessStatusCode)
                {
                    var FeedbackOfService = await FeedbackOfServiceResponse.Content.ReadFromJsonAsync<List<FeedbackForm>>();

                    dashboard.FeedbackService = FeedbackOfService;
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }

            return View(dashboard);
        }

        public class DashBoard
        {
            public List<FeedbackForm>? FeedbackRoom { get; set; }
            public List<FeedbackForm>? FeedbackService { get; set; }
            public List<FeedbackForm>? FeedbackProduct { get; set; }
        }
    }
}
