using AutoMapper;
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
                .ToList();
            return Ok(_mapper.Map<List<ProductDTO>>(products));
        }
        [HttpGet("ProductID/{id}")]
        public IActionResult GetById(int id)
        {
            Product product = _context.Products
                .Include(s => s.ProCategories)
                .FirstOrDefault(c => c.ProductId == id);
            return Ok(_mapper.Map<ProductDTO>(product));
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
                string errorMessage = "Ảnh phòng không được để trống!";
                return BadRequest(errorMessage);
            }
            else if (productDTO.Picture.Contains(" "))
            {
                string errorMessage = "URL ảnh không chứa khoảng trắng!";
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
                string errorMessage = "Ảnh phòng không được để trống!";
                return BadRequest(errorMessage);
            }
            else if (productDTO.Picture.Contains(" "))
            {
                string errorMessage = "URL ảnh không chứa khoảng trắng!";
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
                product.CreateDate = DateTime.Now;
                product.ProCategoriesId = productDTO.ProCategoriesId;
                _context.Update(product);
                await _context.SaveChangesAsync();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }
    }
}
