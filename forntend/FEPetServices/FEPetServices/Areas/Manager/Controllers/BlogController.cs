using FEPetServices.Areas.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FEPetServices.Areas.Manager.Controllers
{
    public class BlogController : Controller
    {
        private readonly HttpClient client = null;
        private string DefaultApiUrl = "";
        private readonly IConfiguration configuration;

        private string DefaultApiUrlBlogList = "";
        private string DefaultApiUrlBlogDetail = "";
        private string DefaultApiUrlBlogAdd = "";
        private string DefaultApiUrlBlogUpdate = "";

        public BlogController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            DefaultApiUrl = configuration.GetValue<string>("DefaultApiUrl");

            DefaultApiUrlBlogList = "https://localhost:7255/api/Blog";
            DefaultApiUrlBlogDetail = "https://localhost:7255/api/Blog/BlogID";
            DefaultApiUrlBlogAdd = "https://localhost:7255/api/Blog/CreateBlog";
            DefaultApiUrlBlogUpdate = "https://localhost:7255/api/Blog/UpdateBlog?blogId=";

        }

        public async Task<IActionResult> Index(BlogDTO blog)
        {
            try
            {
                var json = JsonConvert.SerializeObject(blog);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Blog/GetAllBlog");
                //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlBlogList + "/GetAllBlog");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var servicecategoryList = JsonConvert.DeserializeObject<List<BlogDTO>>(responseContent);
                        return View(servicecategoryList);
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

        public async Task<IActionResult> AddBlog([FromForm] BlogDTO blog, List<IFormFile> image)
        {
            try
            {
                if (ModelState.IsValid) // Kiểm tra xem biểu mẫu có hợp lệ không
                {
                    if (blog.PageTile == null) { return View(); }
                    foreach (var file in image)
                    {
                        string filename = GenerateRandomNumber(5) + file.FileName;
                        filename = Path.GetFileName(filename);
                        string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Blog/", filename);
                        var stream = new FileStream(uploadfile, FileMode.Create);
                        file.CopyToAsync(stream);
                        blog.ImageUrl = "/img/Blog/" + filename;
                    }

                    // mặc định status là true
                    blog.Status = true;

                    var json = JsonConvert.SerializeObject(blog);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(DefaultApiUrl + "Blog/CreateBlog", content);
                    //HttpResponseMessage response = await client.PostAsync(DefaultApiUrlBlogAdd, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessToast"] = "Thêm bài viết thành công!";
                        return View(blog); 
                    }
                    else
                    {
                        TempData["ErrorToast"] = "Thêm bài viết thất bại. Vui lòng thử lại sau.";
                        return View(blog); 
                    }
                }
                else
                {
                    return View(blog);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(blog); 
            }
        }

        public static string GenerateRandomNumber(int length)
        {
            Random random = new Random();
            const string chars = "0123456789"; 
            char[] randomChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomChars);
        }

        [HttpGet]
        public async Task<IActionResult> EditBlog(int blogId)
        {
            try
            {
                // Gọi API để lấy thông tin ServiceCategory cần chỉnh sửa
                HttpResponseMessage response = await client.GetAsync(DefaultApiUrl + "Blog/BlogID/" + blogId);
                //HttpResponseMessage response = await client.GetAsync(DefaultApiUrlBlogDetail + "/" + blogId);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    BlogDTO managerInfos = System.Text.Json.JsonSerializer.Deserialize<BlogDTO>(responseContent, options);

                    return View(managerInfos);
                }
                else
                {
                    TempData["ErrorToast"] = "Tải dữ liệu lên thất bại. Vui lòng tải lại trang.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
            }

            // Return the view with or without an error message
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> EditBlog([FromForm] BlogDTO blog, IFormFile image, int blogId)
        {
            try
            {
                if (image != null && image.Length > 0)
                {
                    // An image file has been uploaded, so update the image path.
                    Console.WriteLine(image);
                    // Save the image to a location (e.g., a folder in your application)
                    var imagePath = "/img/Blog/" + image.FileName;
                    blog.ImageUrl = imagePath;

                    // Save the image file to a folder on your server
                    var physicalImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img","Blog", image.FileName);
                    using (var stream = new FileStream(physicalImagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                }
                else
                {

                    HttpResponseMessage responseForImage = await client.GetAsync(DefaultApiUrl + "Blog/BlogID/" + blogId);
                    //HttpResponseMessage responseForImage = await client.GetAsync(DefaultApiUrlBlogDetail + "/" + blogId);

                    if (responseForImage.IsSuccessStatusCode)
                    {
                        var responseContent = await responseForImage.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var existingServiceCategory = JsonConvert.DeserializeObject<BlogDTO>(responseContent);

                            if (existingServiceCategory != null)
                            {
                                // Assign the existing image path to serviceCategory.Prictue.
                                blog.ImageUrl = existingServiceCategory.ImageUrl;
                            }
                        }
                    }
                }
                if (Request.Form["Status"] == "on")
                {
                    blog.Status = true;
                }
                else
                {
                    blog.Status = false;
                }

                var json = JsonConvert.SerializeObject(blog);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi dữ liệu lên máy chủ
                HttpResponseMessage response = await client.PutAsync(DefaultApiUrl + "Blog/UpdateBlog?blogId=" + blogId, content);
                //HttpResponseMessage response = await client.PutAsync(DefaultApiUrlBlogUpdate + blogId, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessToast"] = "Chỉnh sửa bài viết thành công!";
                    return View(blog); // Chuyển hướng đến trang thành công hoặc trang danh sách
                }
                else
                {
                    TempData["ErrorToast"] = "Chỉnh sửa bài viết thất bại. Vui lòng thử lại sau.";
                    return View(blog); // Hiển thị lại biểu mẫu với dữ liệu đã điền
                }
            }

            catch (Exception ex)
            {
                TempData["ErrorToast"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(blog); // Hiển thị lại biểu mẫu với dữ liệu đã điền
            }
        }
    }
}

