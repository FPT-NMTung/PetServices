﻿using FEPetServices.Areas.DTO;
using FEPetServices.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PetServices.DTO;
using PetServices.Form;
using PetServices.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
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
            try
            {
                // số khách hàng mới trong tháng 
                HttpResponseMessage NumberCustomerInMonthResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetNumberCustomerInMonth");

                if (NumberCustomerInMonthResponse.IsSuccessStatusCode)
                {
                    var NumberCustomerInMonth = await NumberCustomerInMonthResponse.Content.ReadFromJsonAsync<int>();
                    ViewBag.NumberCustomerInMonth = NumberCustomerInMonth;
                }

                // % khách hàng mới trong tháng so với tháng trước 
                HttpResponseMessage PercentCustomerPreviousMonthResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetPercentCustomerPreviousMonth");

                if (PercentCustomerPreviousMonthResponse.IsSuccessStatusCode)
                {
                    var PercentCustomerPreviousMonth = await PercentCustomerPreviousMonthResponse.Content.ReadFromJsonAsync<double>();
                    ViewBag.PercentCustomerPreviousMonth = PercentCustomerPreviousMonth;
                }

                // số đơn hàng trong tháng
                HttpResponseMessage NumberOrderInMonthResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetNumberOrderInMonth");

                if (NumberOrderInMonthResponse.IsSuccessStatusCode)
                {
                    var NumberOrderInMonth = await NumberOrderInMonthResponse.Content.ReadFromJsonAsync<int>();
                    ViewBag.NumberOrderInMonth = NumberOrderInMonth;
                }

                // % số đơn hàng trong tháng so với tháng trước
                HttpResponseMessage PercentOrderPreviousMonthResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetPercentOrderPreviousMonth");

                if (PercentOrderPreviousMonthResponse.IsSuccessStatusCode)
                {
                    var PercentOrderPreviousMonth = await PercentOrderPreviousMonthResponse.Content.ReadFromJsonAsync<double>();
                    ViewBag.PercentOrderPreviousMonth = PercentOrderPreviousMonth;
                }

                // tổng thu nhập trong tháng
                HttpResponseMessage IncomeInMonthResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetIncomeInMonth");

                if (IncomeInMonthResponse.IsSuccessStatusCode)
                {
                    var IncomeInMonth = await IncomeInMonthResponse.Content.ReadFromJsonAsync<int>();

                    ViewBag.IncomeInMonth = IncomeInMonth;
                }

                // % tổng thu nhập trong tháng so với tháng trước
                HttpResponseMessage PercentIncomePreviousMonthResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetPercentIncomePreviousMonth");

                if (PercentIncomePreviousMonthResponse.IsSuccessStatusCode)
                {
                    var PercentIncomePreviousMonth = await PercentIncomePreviousMonthResponse.Content.ReadFromJsonAsync<double>();
                    ViewBag.PercentIncomePreviousMonth = PercentIncomePreviousMonth;
                }

                // số lượng sản phẩm bán trong tháng
                HttpResponseMessage NumberProductInMonthResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetNumberProductInMonth");

                if (NumberProductInMonthResponse.IsSuccessStatusCode)
                {
                    var NumberProductInMonth = await NumberProductInMonthResponse.Content.ReadFromJsonAsync<int>();
                    ViewBag.NumberProductInMonth = NumberProductInMonth;
                }

                // % số lượng sản phẩm bán trong tháng so với tháng trước
                HttpResponseMessage PercentNumberProductPreviousMonthResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetPercentNumberProductPreviousMonth");

                if (PercentNumberProductPreviousMonthResponse.IsSuccessStatusCode)
                {
                    var PercentNumberProductPreviousMonth = await PercentNumberProductPreviousMonthResponse.Content.ReadFromJsonAsync<double>();
                    ViewBag.PercentNumberProductPreviousMonth = PercentNumberProductPreviousMonth;
                }

                // Doanh số service theo ngày
                HttpResponseMessage TotalPriceServiceIn7DayResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetTotalPriceServiceIn7Day");

                if (TotalPriceServiceIn7DayResponse.IsSuccessStatusCode)
                {
                    var TotalPriceServiceIn7Day = await TotalPriceServiceIn7DayResponse.Content.ReadFromJsonAsync<List<ReceiveInDayForm>>();
                    ViewBag.TotalPriceServiceIn7Day = new SelectList(TotalPriceServiceIn7Day, "Date", "Receive");
                }

                // Doanh số product theo ngày
                HttpResponseMessage TotalPriceProductIn7DayResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetTotalPriceProductIn7Day");

                if (TotalPriceServiceIn7DayResponse.IsSuccessStatusCode)
                {
                    var TotalPriceProductIn7Day = await TotalPriceProductIn7DayResponse.Content.ReadFromJsonAsync<List<ReceiveInDayForm>>();
                    ViewBag.TotalPriceProductIn7Day = new SelectList(TotalPriceProductIn7Day, "Date", "Receive");
                }

                // Doanh số room theo ngày
                HttpResponseMessage TotalPriceRoomIn7DayResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetTotalPriceRoomIn7Day");

                if (TotalPriceRoomIn7DayResponse.IsSuccessStatusCode)
                {
                    var TotalPriceRoomIn7Day = await TotalPriceRoomIn7DayResponse.Content.ReadFromJsonAsync<List<ReceiveInDayForm>>();
                    ViewBag.TotalPriceRoomIn7Day = new SelectList(TotalPriceRoomIn7Day, "Date", "Receive");
                }

                // Số đơn hàng hoàn thành trong tháng 
                HttpResponseMessage NumberOrderCompleteInMonthResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetNumberOrderCompleteInMonth");

                if (NumberOrderCompleteInMonthResponse.IsSuccessStatusCode)
                {
                    var NumberOrderCompleteInMonth = await NumberOrderCompleteInMonthResponse.Content.ReadFromJsonAsync<List<Quantity_RatioForm>>();
                    ViewBag.NumberOrderCompleteInMonth = new SelectList(NumberOrderCompleteInMonth, "date", "quantity");
                    ViewBag.NumberOrderCompleteInMonth1 = new SelectList(NumberOrderCompleteInMonth,"quantity", "Ratio");
                }

                // Số đơn hàng bị hủy trong tháng
                HttpResponseMessage NumberOrderRejectedInMonthResponse = await client.GetAsync("https://localhost:7255/api/Dashboard/GetNumberOrderRejectedInMonth");

                if (NumberOrderRejectedInMonthResponse.IsSuccessStatusCode)
                {
                    var NumberOrderRejectedInMonth = await NumberOrderRejectedInMonthResponse.Content.ReadFromJsonAsync<List<Quantity_RatioForm>>();
                    ViewBag.NumberOrderRejectedInMonth = new SelectList(NumberOrderRejectedInMonth, "date", "quantity");
                    ViewBag.NumberOrderRejectedInMonth1 = new SelectList(NumberOrderRejectedInMonth, "quantity", "Ratio");
                }


            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }

            return View();
        }

        public class DashBoard
        {
            
        }
    }
}
