using DocumentFormat.OpenXml.EMMA;
using FEPetServices.Areas.DTO;
using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetServices.DTO;
using PetServices.Form;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using static FEPetServices.Areas.Manager.Controllers.DashBoardController;

namespace FEPetServices.Areas.Partner.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "PartnerOnly")]
    public class DashboardPartnerController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;
        public DashboardPartnerController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");
            //DefaultApiUrl = "https://localhost:7255/api/";
        }
        public class DashBoardPartner
        {
            public List<FeedbackForm>? FeedbackCustomer { get; set; }
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Thống kê doanh số";
            DashBoardPartner dashBoardPartner = new DashBoardPartner()
            {
                FeedbackCustomer = new List<FeedbackForm>()
            };
            PartnerFeedbackModel model = new PartnerFeedbackModel();
            try
            {
                //lay thong tin cua partner
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

                //so don hang trong thang
                HttpResponseMessage NumberOrderInMonthResponse = await client.GetAsync(DefaultApiUrl + "DashboardPartner/OrderInMonth/" + partnerInfoId);
                if (NumberOrderInMonthResponse.IsSuccessStatusCode)
                {
                    var OrderInMonth = await NumberOrderInMonthResponse.Content.ReadFromJsonAsync<int>();
                    ViewBag.OrderInMonth = OrderInMonth;
                }
                //% so don hang trong thang so vs thang trc
                HttpResponseMessage PercentOrderInMonthAndInPreMonthResponse = await client.GetAsync(DefaultApiUrl + "DashboardPartner/GetPercentOrderInMonth/" + partnerInfoId);
                if (PercentOrderInMonthAndInPreMonthResponse.IsSuccessStatusCode)
                {
                    var PercentOrderInMonthAndInPreMonth = await PercentOrderInMonthAndInPreMonthResponse.Content.ReadFromJsonAsync<double>();
                    ViewBag.PercentOrderInMonthAndInPreMonth = PercentOrderInMonthAndInPreMonth;
                }

                //thu nhap trong thang
                HttpResponseMessage TotalPriceInMonthResponse = await client.GetAsync(DefaultApiUrl + "DashboardPartner/GetTotalPriceInMonth/" + partnerInfoId);
                if (TotalPriceInMonthResponse.IsSuccessStatusCode)
                {
                    var TotalPriceInMonth = await TotalPriceInMonthResponse.Content.ReadFromJsonAsync<int>();
                    ViewBag.TotalPriceInMonth = TotalPriceInMonth;
                }
                //% thu nhap trong thang so vs thang trc
                HttpResponseMessage PercentTotalPriceInMonthAndInPreMonthResponse = await client.GetAsync(DefaultApiUrl + "DashboardPartner/GetPercentTotalPriceInMonthAndInPreMonth/" + partnerInfoId);
                if (PercentTotalPriceInMonthAndInPreMonthResponse.IsSuccessStatusCode)
                {
                    var PercentTotalPriceInMonthAndInPreMonth = await PercentTotalPriceInMonthAndInPreMonthResponse.Content.ReadFromJsonAsync<double>();
                    ViewBag.PercentTotalPriceInMonthAndInPreMonth = PercentTotalPriceInMonthAndInPreMonth;
                }

                //tổng đánh giá
                HttpResponseMessage StarInPartnerResponse = await client.GetAsync(DefaultApiUrl + "DashboardPartner/GetStarInPartner/" + partnerInfoId);
                if (StarInPartnerResponse.IsSuccessStatusCode)
                {
                    var StarInPartner = await StarInPartnerResponse.Content.ReadFromJsonAsync<int>();
                    ViewBag.StarInPartner = StarInPartner;
                }
                //lấy số lượng đánh giá
                HttpResponseMessage voteNumberResponse = await client.GetAsync(DefaultApiUrl + "DashboardPartner/GetPartnerVoteNumber/" + partnerInfoId);

                if (voteNumberResponse.IsSuccessStatusCode)
                {
                    var voteNumber = await voteNumberResponse.Content.ReadFromJsonAsync<VoteNumberDTO>();

                    model.VoteNumberas = voteNumber;
                }
                //lấy số sao
                HttpResponseMessage PartnerStarResponse = await client.GetAsync(DefaultApiUrl + "DashboardPartner/GetPartnerStar/" + partnerInfoId);

                if (PartnerStarResponse.IsSuccessStatusCode)
                {
                    var content = await PartnerStarResponse.Content.ReadAsStringAsync();

                    if (double.TryParse(content, out double PartnerStar))
                    {
                        ViewBag.PartnerStar = PartnerStar;
                    }
                }

                //doanh thu theo ngày
                HttpResponseMessage TotalPriceIn7DayResponse = await client.GetAsync(DefaultApiUrl + "DashboardPartner/GetTotalPriceIn7Day/" + partnerInfoId);

                if (TotalPriceIn7DayResponse.IsSuccessStatusCode)
                {
                    var TotalPriceIn7Day = await TotalPriceIn7DayResponse.Content.ReadFromJsonAsync<List<ReceiveInDayForm>>();
                    ViewBag.TotalPriceIn7Day = new SelectList(TotalPriceIn7Day, "Date", "Receive");
                }

                // Số đơn hàng hoàn thành trong tháng 
                HttpResponseMessage NumberOrderCompleteInMonthResponse = await client.GetAsync(DefaultApiUrl + "DashboardPartner/GetNumberOrderCompleteInMonth/" + partnerInfoId);

                if (NumberOrderCompleteInMonthResponse.IsSuccessStatusCode)
                {
                    var NumberOrderCompleteInMonth = await NumberOrderCompleteInMonthResponse.Content.ReadFromJsonAsync<List<Quantity_RatioForm>>();
                    ViewBag.NumberOrderCompleteInMonth = new SelectList(NumberOrderCompleteInMonth, "date", "quantity");
                    ViewBag.NumberOrderCompleteInMonth1 = new SelectList(NumberOrderCompleteInMonth, "quantity", "Ratio");
                }

                // đánh giá của khách hàng về các nhân viên
                HttpResponseMessage FeedbackOfPartnerResponse = await client.GetAsync(DefaultApiUrl + "DashboardPartner/GetFeedbackOfPartner/" + partnerInfoId);

                if (FeedbackOfPartnerResponse.IsSuccessStatusCode)
                {
                    var FeedbackOfPartner = await FeedbackOfPartnerResponse.Content.ReadFromJsonAsync<List<FeedbackForm>>();

                    dashBoardPartner.FeedbackCustomer = FeedbackOfPartner;
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View(dashBoardPartner);
        }

        public class PartnerFeedbackModel
        {
            public List<FeedbackDTO> Feedback { get; set; }
            public VoteNumberDTO VoteNumberas { get; set; }
            public PartnerInfo Partner { get; set; }
            public List<PartnerInfo> partners { get; set; }
        }
    }
}
