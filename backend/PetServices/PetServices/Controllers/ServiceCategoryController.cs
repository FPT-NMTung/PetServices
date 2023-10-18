using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCategoryController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;
        
        /*[Authorize(Roles = "MANAGER")]*/
        public ServiceCategoryController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        
        [HttpGet("GetAllServiceCategory")]
        public IActionResult GetAll()
        {
            List<ServiceCategory> serviceCategories = _context.ServiceCategories.ToList();
            return Ok(_mapper.Map<List<ServiceCategoryDTO>>(serviceCategories));
        }


        [HttpGet("{ServiceCategorysName}")]
        public IActionResult GetByName(string ServiceCategorysName)
        {
            List<ServiceCategory> serviceCategories = _context.ServiceCategories
                .Where(c => c.SerCategoriesName == ServiceCategorysName)
                .ToList();
            return Ok(_mapper.Map<List<ServiceCategoryDTO>>(serviceCategories));
        }

        /*[Authorize(Roles = "MANAGER")]*/
        [HttpGet("ServiceCategorysID/{id}")]
        public IActionResult GetById(int id)
        {
            List<ServiceCategory> serviceCategories = _context.ServiceCategories
                .Where(c => c.SerCategoriesId == id)
                .ToList();
            return Ok(_mapper.Map<List<ServiceCategoryDTO>>(serviceCategories));
        }

        
        [HttpPost("AddServiceCategory")]
        public async Task<IActionResult> CreateSerCategories(ServiceCategoryDTO serviceCategoryDTO)
        {
            if (serviceCategoryDTO == null)
            {
                return BadRequest("ServiceCategory data is missing.");
            }

            var newServiceCategory = new ServiceCategory
            {
                SerCategoriesId = serviceCategoryDTO.SerCategoriesId,
                SerCategoriesName = serviceCategoryDTO.SerCategoriesName,
                Desciptions = serviceCategoryDTO.Desciptions,
                Picture = serviceCategoryDTO.Picture,
            };

            _context.ServiceCategories.Add(newServiceCategory);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(_mapper.Map<ServiceCategoryDTO>(newServiceCategory));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        
        [HttpPut("EditServiceCategory")]
        public IActionResult Update(ServiceCategoryDTO serviceCategoryDTO, int serCategoriesId)
        {
            var servicecategorie = _context.ServiceCategories.FirstOrDefault(p => p.SerCategoriesId == serCategoriesId);
            if (servicecategorie == null)
            {
                return NotFound();
            }

            servicecategorie.SerCategoriesName = serviceCategoryDTO.SerCategoriesName;
            servicecategorie.Desciptions = serviceCategoryDTO.Desciptions;
            servicecategorie.Picture = serviceCategoryDTO.Picture;

            try
            {
                _context.Entry(servicecategorie).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
            return Ok(servicecategorie);
        }

        [HttpDelete]
        public IActionResult Delete(int serCategoriesId)
        {
            var servicecategorie = _context.ServiceCategories.FirstOrDefault(p => p.SerCategoriesId == serCategoriesId);
            if (servicecategorie == null)
            {
                return NotFound();
            }
            try
            {
                _context.ServiceCategories.Remove(servicecategorie);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
            return Ok(servicecategorie);
        }
    }
}
