using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Form;
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
        public IActionResult GetAllServiceCategory()
        {
            List<ServiceCategory> serviceCategories = _context.ServiceCategories.ToList();

            var serviceCategorylist = _mapper.Map<List<ServiceCategoryDTO>>(serviceCategories);

            foreach (var serviceCategory in serviceCategorylist)
            {
                var services = _context.Services.Where(s => s.SerCategoriesId == serviceCategory.SerCategoriesId).ToList();
                int count = 0;
                double totalStar = 0;
                int totalFeedbackCount = 0;

                foreach (var service in services)
                {
                    var feedback = _context.Feedbacks.Where(f => f.ServiceId == service.ServiceId).ToList();

                    if (feedback.Any())
                    {
                        totalStar += feedback.Average(f => f.NumberStart) ?? 0;
                        totalFeedbackCount += feedback.Count;
                        count++;
                    }
                }

                serviceCategory.NumberStar = totalFeedbackCount > 0 ? Math.Round(totalStar / count, 1) : 0;
                serviceCategory.NumberVoter = totalFeedbackCount;
            }

            return Ok(serviceCategorylist);
        }


        [HttpGet("{ServiceCategorysName}")]
        public IActionResult GetByNameServiceCategory(string ServiceCategorysName)
        {
            List<ServiceCategory> serviceCategories = _context.ServiceCategories
                .Where(c => c.SerCategoriesName == ServiceCategorysName)
                .ToList();
            return Ok(_mapper.Map<List<ServiceCategoryDTO>>(serviceCategories));
        }

        /*[Authorize(Roles = "MANAGER")]*/
        [HttpGet("ServiceCategorysID/{id}")]
        public IActionResult GetByIdServiceCategory(int id)
        {
            ServiceCategory serviceCategories = _context.ServiceCategories.Include(s => s.Services)
                .FirstOrDefault(c => c.SerCategoriesId == id);
            return Ok(_mapper.Map<ServiceCategoryDTO>(serviceCategories));
        }


        [HttpGet("GetServiceByServiceCategoryAndServiceID")]
        public IActionResult GetServiceByServiceCategoryAndServiceID(int serviceCategoryId, int serviceId)
        {
            // Find the service that matches the specified serviceId and serviceCategoryId.
            Service service = _context.Services
                .Include(s => s.SerCategories)
                .FirstOrDefault(s => s.ServiceId == serviceId && s.SerCategoriesId == serviceCategoryId);

            if (service == null)
            {
                return NotFound("Service not found for the specified service ID and service category ID.");
            }

            // Map the service to a ServiceDTO object.
            ServiceDTO serviceDTO = _mapper.Map<ServiceDTO>(service);

            return Ok(serviceDTO);
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
                Status = serviceCategoryDTO.Status
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
        public IActionResult UpdateServiceCategory(ServiceCategoryDTO serviceCategoryDTO, int serCategoriesId)
        {
            var servicecategorie = _context.ServiceCategories.FirstOrDefault(p => p.SerCategoriesId == serCategoriesId);
            if (servicecategorie == null)
            {
                return NotFound();
            }

            servicecategorie.SerCategoriesName = serviceCategoryDTO.SerCategoriesName;
            servicecategorie.Desciptions = serviceCategoryDTO.Desciptions;
            servicecategorie.Picture = serviceCategoryDTO.Picture;
            servicecategorie.Status = serviceCategoryDTO.Status;

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

        [HttpGet("GetServicesByCategory/{serviceCategoryID}")]
        public IActionResult GetServicesByCategory(int serviceCategoryID)
        {
            // Find the service category by ID
            var serviceCategory = _context.ServiceCategories.FirstOrDefault(sc => sc.SerCategoriesId == serviceCategoryID);

            if (serviceCategory == null)
            {
                return NotFound("Service category not found.");
            }

            if (serviceCategory.Status == false)
            {
                return BadRequest("Service category status is currently inactive.");
            }

            // Fetch services belonging to the specified serviceCategoryID
            List<Service> servicesInCategory = _context.Services.Include(s => s.SerCategories)
                .Where(s => s.SerCategoriesId == serviceCategoryID)
                .ToList();

            if (servicesInCategory.Count == 0)
            {
                return NotFound("No services found for the specified category.");
            }

            // Map the services to ServiceDTO objects
            List<ServiceDTO> serviceDTOs = _mapper.Map<List<ServiceDTO>>(servicesInCategory);

            return Ok(serviceDTOs);
        }



        [Authorize(Roles = "MANAGER")]
        [HttpDelete]
        public IActionResult DeleteServiceCategory(int serCategoriesId)
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
