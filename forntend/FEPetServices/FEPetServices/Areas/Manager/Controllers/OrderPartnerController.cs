using DocumentFormat.OpenXml.Drawing.Charts;
using FEPetServices.Areas.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PetServices.DTO;
using PetServices.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Policy = "ManaOnly")]
    public class OrderPartnerController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        public OrderPartnerController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");

        }

        public async Task<ActionResult> Index(OrderPartnerDTO order, int month, int year)
        {
            try
            {
                ViewBag.Title = "Doanh thu nhân viên";

                var json = JsonConvert.SerializeObject(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //https://localhost:7255/api/Partner/GetOrderPartner?month=12&year=2023

                if (month < 1 && year < 1)
                {
                    month = DateTime.Now.Month;
                    year = DateTime.Now.Year;
                }

                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Partner/GetOrderPartner?month=" + month + "&year=" + year);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var OrderPartnerList = JsonConvert.DeserializeObject<List<OrderPartnerDTO>>(responseContent);

                        ViewBag.month = month;
                        ViewBag.year = year;

                        return View(OrderPartnerList);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "API trả về dữ liệu rỗng.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Tải dữ liệu lên thất bại. Vui lòng tải lại trang.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }


            return View();
        }

        public async Task<ActionResult> OrderPartnerDetail(OrderPartnerDTO order, int partnerId, int month, int year)
        {
            try
            {
                ViewBag.Title = "Chi tiết doanh thu nhân viên";

                var json = JsonConvert.SerializeObject(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Partner/GetOrderDetailPartner?PartnerId=" + partnerId + "&month=" + month + "&year=" + year);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var OrderPartnerList = JsonConvert.DeserializeObject<List<OrderPartnerDTO>>(responseContent);

                        //ViewBag.month = month;
                        //ViewBag.year = year;

                        return View(OrderPartnerList);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "API trả về dữ liệu rỗng.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Tải dữ liệu lên thất bại. Vui lòng tải lại trang.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
            }
            return View();
        }
    }
}

