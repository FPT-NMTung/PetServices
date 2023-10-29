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
    public class ServiceController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;
        /*[Authorize(Roles = "MANAGER")]*/
        public ServiceController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        

        [HttpGet("GetAllService")]
        public IActionResult Get()
        {
            List<Service> services = _context.Services.Include(s => s.SerCategories)
               .ToList();
            return Ok(_mapper.Map<List<ServiceDTO>>(services));
        }

    

        [HttpGet("ServiceID/{id}")]
        public IActionResult GetById(int id)
        {
            Service  service = _context.Services
                .Include(s => s.SerCategories)
                .FirstOrDefault(c => c.ServiceId == id)
                ;

            return Ok(_mapper.Map<ServiceDTO>(service));
        }

        [HttpGet("GetServicesByCategory/{serviceCategoryID}")]
        public IActionResult GetServicesByCategory(int serviceCategoryID)
        {
            // Find services that belong to the specified serviceCategoryID.
            List<Service> servicesInCategory = _context.Services
                .Where(s => s.SerCategoriesId == serviceCategoryID)
                .ToList();

            if (servicesInCategory.Count == 0)
            {
                return NotFound("No services found for the specified category.");
            }

            // Map the services to ServiceDTO objects.
            List<ServiceDTO> serviceDTOs = _mapper.Map<List<ServiceDTO>>(servicesInCategory);

            return Ok(serviceDTOs);
        }

        [HttpPost("CreateService")]
        public async Task<IActionResult> CreateService(ServiceDTO serviceDTO)
        {
            if (serviceDTO == null)
            {
                return BadRequest("Service data is missing.");
            }

            var newServices = new Service
            {
                ServiceId = serviceDTO.ServiceId,
                ServiceName = serviceDTO.ServiceName,
                Desciptions = serviceDTO.Desciptions,
                Price = serviceDTO.Price,
                Picture = serviceDTO.Picture,
                Status = serviceDTO.Status,
                SerCategoriesId= serviceDTO.SerCategoriesId

            };

            _context.Services.Add(newServices);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(_mapper.Map<ServiceDTO>(newServices));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex.InnerException.Message);
            }
        }

        
        [HttpPut("UpdateServices")]
        public IActionResult Update(ServiceDTO serviceDTO, int serviceId)
        {
            var service = _context.Services
                .Include(a => a.SerCategories)
                .FirstOrDefault(p => p.ServiceId == serviceId);

            if (service == null)
            {
                return NotFound();
            }

            service.ServiceName = serviceDTO.ServiceName;
            service.Desciptions = serviceDTO.Desciptions;
            service.Price = serviceDTO.Price;
            service.Picture = serviceDTO.Picture;
            service.Status = serviceDTO.Status;
            service.SerCategoriesId = serviceDTO.SerCategoriesId;

            try
            {
                _context.Entry(service).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
            return Ok(service);
        }

        
        [HttpDelete]
        public IActionResult Delete(int serviceId)
        {
            var service = _context.Services.FirstOrDefault(p => p.ServiceId == serviceId);
            if (service == null)
            {
                return NotFound();
            }
            try
            {
                _context.Services.Remove(service);
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
