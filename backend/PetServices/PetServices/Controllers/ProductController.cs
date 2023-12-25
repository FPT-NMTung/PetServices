using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Models;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ProductController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpGet("GetAll")]
        public IActionResult GetAllProduct()
        {
            List<Product> products = _context.Products.Include(s => s.ProCategories)
                .OrderByDescending(p => p.Quantity)
                .ToList();

            var productlist = _mapper.Map<List<ProductDTO>>(products);

            foreach (var product in productlist)
            {
                var averageStars = _context.Feedbacks.Where(f => f.ProductId == product.ProductId).Average(f => f.NumberStart);

                if (averageStars.HasValue)
                {
                    averageStars = Math.Round(averageStars.Value, 1);
                }

                product.NumberStar = averageStars ?? 0;
                product.NumberVoter = _context.Feedbacks.Where(f => f.ProductId == product.ProductId).ToList().Count();
            }

            return Ok(productlist);
        }

        [HttpGet("GetAllProductWhenCategoryTrue")]
        public IActionResult GetProductWhenCategoryTrue()
        {
            // Filter services based on the status of their associated service categories
            List<Product> products = _context.Products
                .Include(s => s.ProCategories)
                .OrderByDescending(x => x.ProductId)
                .Where(s => s.ProCategories.Status == true) // Filter based on service category status
                .ToList();

            var productlist = _mapper.Map<List<ProductDTO>>(products);

            foreach (var product in productlist)
            {
                var averageStars = _context.Feedbacks.Where(f => f.ProductId == product.ProductId).Average(f => f.NumberStart);

                if (averageStars.HasValue)
                {
                    averageStars = Math.Round(averageStars.Value, 1);
                }

                product.NumberStar = averageStars ?? 0;
                product.NumberVoter = _context.Feedbacks.Where(f => f.ProductId == product.ProductId).ToList().Count();
            }

            return Ok(productlist);
        }

        [HttpGet("ProductID/{id}")]
        public IActionResult GetById(int id)
        {
            Product product = _context.Products
                .Include(s => s.ProCategories)
                .FirstOrDefault(c => c.ProductId == id);
            return Ok(_mapper.Map<ProductDTO>(product));
        }
        [HttpGet("GetByCategory/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            try
            {
                // Kiểm tra xem categoryId có tồn tại trong danh sách loại sản phẩm hay không
                var category = _context.ProductCategories.FirstOrDefault(c => c.ProCategoriesId == categoryId);
                if (category == null)
                {
                    return BadRequest("Loại sản phẩm không tồn tại.");
                }

                // Lấy danh sách sản phẩm thuộc loại categoryId
                List<Product> products = _context.Products
                    .Include(s => s.ProCategories)
                    .Where(p => p.ProCategoriesId == categoryId)
                    .OrderByDescending(x => x.ProductId)
                    .ToList();

                // Kiểm tra xem có sản phẩm nào thuộc loại đó hay không
                if (products.Count == 0)
                {
                    return NotFound("Không tìm thấy sản phẩm thuộc loại này.");
                }

                return Ok(_mapper.Map<List<ProductDTO>>(products));
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> CreateProduct(ProductDTO productDTO)
        {
            // check tên sản phẩm
            if (string.IsNullOrWhiteSpace(productDTO.ProductName))
            {
                string errorMessage = "Tên sản phẩm không được để trống!";
                return BadRequest(errorMessage);
            }
            if (productDTO.ProductName.Length > 500)
            {
                string errorMessage = "Tên sản phẩm vượt quá số ký tự. Tối đa 500 ký tự!";
                return BadRequest(errorMessage);
            }
            // check mô tả
            if (string.IsNullOrWhiteSpace(productDTO.Desciption))
            {
                string errorMessage = "Mô tả không được để trống!";
                return BadRequest(errorMessage);
            }
            // check ảnh
            if (string.IsNullOrWhiteSpace(productDTO.Picture))
            {
                string errorMessage = "Ảnh sản phẩm không được để trống!";
                return BadRequest(errorMessage);
            }
            else if (productDTO.Picture.Contains(" "))
            {
                string errorMessage = "URL ảnh không chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            // check số lượng Slot
            if (productDTO.Quantity <= 0)
            {
                string errorMessage = "Số lượng phải lớn hơn 0!";
                return BadRequest(errorMessage);
            }
            // check giá
            if (productDTO.Price <= 0)
            {
                string errorMessage = "Giá phải lớn hơn 0!";
                return BadRequest(errorMessage);
            }
            // check loại sản phẩm            
            var proCategoriesId = _context.ProductCategories.FirstOrDefault(p => p.ProCategoriesId == productDTO.ProCategoriesId);
            if (proCategoriesId == null)
            {
                string errorMessage = "Loại sản phẩm không tồn tại!";
                return BadRequest(errorMessage);
            }
            try
            {
                var product = new Product
                {
                    ProductName = productDTO.ProductName,
                    Desciption = productDTO.Desciption,
                    Picture = productDTO.Picture,
                    Status = true,
                    Price = productDTO.Price,
                    Quantity = productDTO.Quantity,
                    CreateDate = DateTime.Now,
                    ProCategoriesId = productDTO.ProCategoriesId
                };

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return Ok("Thêm sản phẩm thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(ProductDTO productDTO, int proId)
        {
            // check tên sản phẩm
            if (string.IsNullOrWhiteSpace(productDTO.ProductName))
            {
                string errorMessage = "Tên sản phẩm không được để trống!";
                return BadRequest(errorMessage);
            }
            if (productDTO.ProductName.Length > 500)
            {
                string errorMessage = "Tên sản phẩm vượt quá số ký tự. Tối đa 500 ký tự!";
                return BadRequest(errorMessage);
            }
            // check mô tả
            if (string.IsNullOrWhiteSpace(productDTO.Desciption))
            {
                string errorMessage = "Mô tả không được để trống!";
                return BadRequest(errorMessage);
            }
            // check ảnh
            if (string.IsNullOrWhiteSpace(productDTO.Picture))
            {
                string errorMessage = "Ảnh sản phẩm không được để trống!";
                return BadRequest(errorMessage);
            }
            else if (productDTO.Picture.Contains(" "))
            {
                string errorMessage = "URL ảnh không chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            // check số lượng Slot
            if (productDTO.Quantity <= 0)
            {
                string errorMessage = "Số lượng phải lớn hơn 0!";
                return BadRequest(errorMessage);
            }
            // check giá
            if (productDTO.Price <= 0)
            {
                string errorMessage = "Giá phải lớn hơn 0!";
                return BadRequest(errorMessage);
            }
            // check loại sản phẩm            
            var proCategoriesId = _context.ProductCategories.FirstOrDefault(p => p.ProCategoriesId == productDTO.ProCategoriesId);
            if (proCategoriesId == null)
            {
                string errorMessage = "Loại sản phẩm không tồn tại!";
                return BadRequest(errorMessage);
            }
            try
            {
                var product = _context.Products
                                .Include(a => a.ProCategories)
                                .FirstOrDefault(p => p.ProductId == proId);
                if (product == null)
                {
                    return BadRequest("Không tìm thấy sản phẩm bạn chọn.");
                }

                product.ProductName = productDTO.ProductName;
                product.Desciption = productDTO.Desciption;
                product.Picture = productDTO.Picture;
                product.Status = productDTO.Status;
                product.Price = productDTO.Price;
                product.Quantity = productDTO.Quantity;
                product.CreateDate = productDTO.CreateDate;
                product.UpdateDate = DateTime.Now;
                product.ProCategoriesId = productDTO.ProCategoriesId;
                _context.Update(product);
                await _context.SaveChangesAsync();

                return Ok("Cập nhật sản phẩm thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPut("ChangeStatusProduct")]
        public async Task<ActionResult> ChangeStatusProduct(int ProductId, bool status)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == ProductId);
                if (product == null)
                {
                    return BadRequest("Không tìm thấy sản phẩm cần thay đổi.");
                }

                product.Status = status;
                product.UpdateDate = DateTime.Now;
                _context.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Cập nhật sản phẩm thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPut("ChangeProduct")]
        public async Task<ActionResult> ChangeProduct(int ProductId, int Quantity)
        {
            try
            {
                if (ProductId <= 0 || Quantity < 0 )
                {
                    return BadRequest("Invalid input parameters.");
                }

                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == ProductId);
                if (product == null)
                {
                    return BadRequest("Không tìm thấy sản phẩm cần thay đổi.");
                }

                _context.Entry(product).OriginalValues["Quantity"] = product.Quantity;

                product.Quantity -= Quantity;
                product.QuantitySold += Quantity;


                await _context.SaveChangesAsync();
                return Ok("Cập nhật sản phẩm thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPut("InChangeProduct")]
        public async Task<ActionResult> InChangeProduct(int ProductId, int Quantity)
        {
            try
            {
                if (ProductId <= 0 || Quantity < 0)
                {
                    return BadRequest("Invalid input parameters.");
                }

                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == ProductId);
                if (product == null)
                {
                    return BadRequest("Không tìm thấy sản phẩm cần thay đổi.");
                }

                _context.Entry(product).OriginalValues["Quantity"] = product.Quantity;

                product.Quantity += Quantity;
                product.QuantitySold -= Quantity;

                await _context.SaveChangesAsync();
                return Ok("Cập nhật sản phẩm thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpDelete]
        public IActionResult DeleteServce(int serviceId)
        {
            var service = _context.Products.FirstOrDefault(p => p.ProductId == serviceId);
            if (service == null)
            {
                return NotFound();
            }
            try
            {
                _context.Products.Remove(service);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
            return Ok(service);
        }
    }
}
