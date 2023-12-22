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
    public class ProductCategoryController : ControllerBase
    {
        public PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ProductCategoryController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        //[Authorize(Roles = "MANAGER")]
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            List<ProductCategory> productCategories = _context.ProductCategories
                .OrderByDescending(x => x.ProCategoriesId)
                .ToList();
            return Ok(_mapper.Map<List<ProductCategoryDTO>>(productCategories));
        }
        [HttpGet("ProductCategorysID/{id}")]
        public IActionResult GetById(int id)
        {
            List<ProductCategory> productCategories = _context.ProductCategories
                .Where(c => c.ProCategoriesId == id)
                .ToList();
            return Ok(_mapper.Map<List<ProductCategoryDTO>>(productCategories));
        }

        [HttpPost("CreateNewProductCategory")]
        public async Task<IActionResult> CreateProCategory(ProductCategoryDTO productCategoryDTO)
        {
            if (productCategoryDTO == null)
            {
                return BadRequest("Product Category date is missing");
            }
            var proCate = new ProductCategory
            {
                ProCategoriesId = productCategoryDTO.ProCategoriesId,
                ProCategoriesName = productCategoryDTO.ProCategoriesName,
                Desciptions = productCategoryDTO.Desciptions,
                Picture = productCategoryDTO.Picture,
                Status = true
            };
            _context.ProductCategories.Add(proCate);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(_mapper.Map<ProductCategoryDTO>(proCate));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        [HttpPut("Update")]
        public IActionResult Update(ProductCategoryDTO productCategoryDTO, int procateId)
        {
            var productCate = _context.ProductCategories.FirstOrDefault(p => p.ProCategoriesId == procateId);
            if (productCate == null)
            {
                return NotFound();
            }
            productCate.ProCategoriesName = productCategoryDTO.ProCategoriesName;
            productCate.Desciptions = productCategoryDTO.Desciptions;
            productCate.Picture = productCategoryDTO.Picture;
            productCate.Status = productCategoryDTO.Status;
            try
            {
                _context.Entry(productCate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
            return Ok();
        }
    }
}
