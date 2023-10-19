using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
            List<Service> service = _context.Services.ToList();
            return Ok(_mapper.Map<List<ServiceDTO>>(service));
        }

        
        [HttpGet("{ServicesName}")]
        public IActionResult Get(string ServicesName)
        {
            List<Service> service = _context.Services.Where(c => c.ServiceName == ServicesName).ToList();
            return Ok(_mapper.Map<List<ServiceDTO>>(service));
        }


        [HttpGet("ServiceID/{id}")]
        public IActionResult GetById(int id)
        {
            List<Service> service = _context.Services
                .Where(c => c.ServiceId == id)
                .ToList();
            return Ok(_mapper.Map<List<ServiceDTO>>(service));
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
            var service = _context.Services.FirstOrDefault(p => p.ServiceId == serviceId);
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
