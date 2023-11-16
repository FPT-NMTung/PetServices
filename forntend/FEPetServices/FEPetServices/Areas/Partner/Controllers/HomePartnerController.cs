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
        private string DefaultApiUrlOrderPartner = "";
        private string DefaultApiUrlOrderListOfPetTraining = "";
        private string DefaultApiUrlOrderListOfPetTrainingSpecial = "";
        public HomePartnerController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = "https://pet-service-api.azurewebsites.net/api/Partner";
            DefaultApiUrlOrderPartner = "https://pet-service-api.azurewebsites.net/api/OrderPartner";
            DefaultApiUrlOrderListOfPetTraining = "https://pet-service-api.azurewebsites.net/api/OrderPartner/ListOrderPetTraining?serCategoriesId=4";
            DefaultApiUrlOrderListOfPetTrainingSpecial = "https://pet-service-api.azurewebsites.net/api/OrderPartner/ListOrderPetTrainingSpecial";
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> ListOrderPetTraining()
        {
            //orderStatus = "Waiting";
            //https://pet-service-api.azurewebsites.net/api/OrderPartner/ListOrderPetTraining?serCategoriesId=4&orderStatus=Waiting
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining);
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
           
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
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
        public async Task<IActionResult> ListOrderPetTrainingComplete()
        {
            //orderStatus = "Waiting";
            //https://pet-service-api.azurewebsites.net/api/OrderPartner/ListOrderPetTraining?serCategoriesId=4&orderStatus=Waiting
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining);
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
        public async Task<IActionResult> ListOrderPetTrainingSpecialComplete()
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
           
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
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
            //orderStatus = "Waiting";
            //https://pet-service-api.azurewebsites.net/api/OrderPartner/ListOrderPetTraining?serCategoriesId=4&orderStatus=Waiting
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining);
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
           
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
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
        public async Task<IActionResult> ListOrderPetTrainingReceived()
        {
            //orderStatus = "Waiting";
            //https://pet-service-api.azurewebsites.net/api/OrderPartner/ListOrderPetTraining?serCategoriesId=4&orderStatus=Waiting
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTraining);
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
        public async Task<IActionResult> ListOrderPetTrainingSpecialReceived()
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
           
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderListOfPetTrainingSpecial + "?serCategoriesId=" + serCategoriesId + "&partnerInfoId=" + partnerInfoId);
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
            HttpResponseMessage response = await client.GetAsync(DefaultApiUrlOrderPartner + "/" + orderId);
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
        //[HttpPut]
        //public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            string apiUrl = DefaultApiUrlOrderListOfPetTraining + "&OrderId=" + orderId + "&OrderStatus=" + status;
                    
        //            var ord = new OrderForm
        //            {
        //                OrderId = orderId,
        //                OrderStatus = status
        //            };

        //            var json = JsonConvert.SerializeObject(ord);
        //            var content = new StringContent(json, Encoding.UTF8, "application/json");

        //            HttpResponseMessage response = await client.PutAsJsonAsync(apiUrl, content);


        //            if (response.IsSuccessStatusCode)
        //            {
        //                return Json(new
        //                {
        //                    Success = true
        //                });
        //            }
        //            else if (response.StatusCode == HttpStatusCode.BadRequest)
        //            {
        //                var errorMessage = await response.Content.ReadAsStringAsync();
        //                ViewBag.ErrorMessage = errorMessage;
        //                return Json(new
        //                {
        //                    Success = false
        //                });
        //            }
        //            else
        //            {
        //                var errorMessage = await response.Content.ReadAsStringAsync();
        //                return Json(new
        //                {
        //                    Success = false
        //                });
        //            }
        //        }
        //        else
        //        {
        //            return Json(new
        //            {
        //                Success = false
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
        //        return View();
        //    }
        //    //// Tìm đơn hàng trong cơ sở dữ liệu theo orderId
        //    //var order = await client.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

        //    //// Kiểm tra nếu đơn hàng tồn tại
        //    //if (order != null)
        //    //{
        //    //    // Cập nhật trạng thái đơn hàng
        //    //    order.OrderStatus = status;

        //    //    // Lưu thay đổi vào cơ sở dữ liệu
        //    //    await dbContext.SaveChangesAsync();

        //    //    // Trả về một phản hồi thành công
        //    //    return Json(new { success = true });
        //    //}

        //    //// Trả về một phản hồi thất bại nếu không tìm thấy đơn hàng
        //    //return Json(new { success = false });
        //}
    }
}
