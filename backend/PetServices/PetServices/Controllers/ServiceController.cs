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
        public IActionResult GetServce()
        {
            List<Service> services = _context.Services.Include(s => s.SerCategories)
               .ToList();
            return Ok(_mapper.Map<List<ServiceDTO>>(services));
        }

    

        [HttpGet("ServiceID/{id}")]
        public IActionResult GetByIdServce(int id)
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
            List<Service> servicesInCategory = _context.Services.Include(s => s.SerCategories)
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

        [HttpGet("GetServiceByServiceCategoryAndServiceID")]
        public IActionResult GetServiceByServiceCategoryAndServiceID(int serviceCategoryID, int serviceID)
        {
            // Find the service that matches the specified serviceID and serviceCategoryID.
            Service service = _context.Services
                .Include(s => s.SerCategories)
                .FirstOrDefault(s => s.ServiceId == serviceID && s.SerCategoriesId == serviceCategoryID);

            if (service == null)
            {
                return NotFound("Service not found for the specified service ID and service category ID.");
            }

            // Map the service to a ServiceDTO object.
            ServiceDTO serviceDTO = _mapper.Map<ServiceDTO>(service);

            return Ok(serviceDTO);
        }

        [HttpPost("CreateService")]
        public async Task<IActionResult> CreateService(ServiceDTO serviceDTO)
        {
            if (serviceDTO == null)
            {
                return BadRequest("Dịch vụ không có sẵn");
            }
            if (string.IsNullOrWhiteSpace(serviceDTO.ServiceName))
            {
                string errorMessage = "Tên dịch vụ không được để trống!";
                return BadRequest(errorMessage);
            }
            if (serviceDTO.ServiceName.Length > 500)
            {
                string errorMessage = "Tên dịch vụ vượt quá số ký tự. Tối đa 500 ký tự!";
                return BadRequest(errorMessage);
            }
            if (string.IsNullOrWhiteSpace(serviceDTO.Desciptions))
            {
                string errorMessage = "Mô tả không được để trống!";
                return BadRequest(errorMessage);
            }
            if (string.IsNullOrWhiteSpace(serviceDTO.Picture))
            {
                string errorMessage = "Ảnh phòng không được để trống!";
                return BadRequest(errorMessage);
            }
            else if (serviceDTO.Picture.Contains(" "))
            {
                string errorMessage = "URL ảnh không chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            // check giá
            if (serviceDTO.Price <= 0)
            {
                string errorMessage = "Giá phải lớn hơn 0!";
                return BadRequest(errorMessage);
            }
            // Check thời gian
            if (serviceDTO.Time <= 0)
            {
                string errorMessage = "Thời gian phải lớn hơn 0!";
                return BadRequest(errorMessage);
            }

            // Check thời gian lớn hơn 1 giờ
            if (serviceDTO.Time < 1)
            {
                string errorMessage = "Thời gian thực hiện dịch vụ phải lớn hơn hoặc bằng 1 giờ!";
                return BadRequest(errorMessage);
            }
            var serviceCategory = _context.ServiceCategories.FirstOrDefault(s => s.SerCategoriesId == serviceDTO.SerCategoriesId);
            if (serviceCategory == null)
            {
                string errorMessage = "Loại dịch vụ không tồn tại!";
                return BadRequest(errorMessage);
            }
            try
            {
                var newServices = new Service
                {
                    ServiceId = serviceDTO.ServiceId,
                    ServiceName = serviceDTO.ServiceName,
                    Desciptions = serviceDTO.Desciptions,
                    Price = serviceDTO.Price,
                    Picture = serviceDTO.Picture,
                    Status = serviceDTO.Status,
                    Time= serviceDTO.Time,  
                    SerCategoriesId = serviceDTO.SerCategoriesId
                };
                
                await _context.Services.AddAsync(newServices);
                await _context.SaveChangesAsync();
                return Ok("Thêm dịch vụ thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
  
        }

        [HttpPut("UpdateServices")]
        public async Task<IActionResult> UpdateServce(ServiceDTO serviceDTO, int serviceId)
        {
            if (string.IsNullOrWhiteSpace(serviceDTO.ServiceName))
            {
                string errorMessage = "Tên dịch vụ không được để trống!";
                return BadRequest(errorMessage);
            }
            if (serviceDTO.ServiceName.Length > 500)
            {
                string errorMessage = "Tên dịch vụ vượt quá số ký tự. Tối đa 500 ký tự!";
                return BadRequest(errorMessage);
            }
            if (string.IsNullOrWhiteSpace(serviceDTO.Desciptions))
            {
                string errorMessage = "Mô tả không được để trống!";
                return BadRequest(errorMessage);
            }
            if (string.IsNullOrWhiteSpace(serviceDTO.Picture))
            {
                string errorMessage = "Ảnh dịch vụ không được để trống!";
                return BadRequest(errorMessage);
            }
            else if (serviceDTO.Picture.Contains(" "))
            {
                string errorMessage = "URL ảnh không chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            // check giá
            if (serviceDTO.Price <= 0)
            {
                string errorMessage = "Giá phải lớn hơn 0!";
                return BadRequest(errorMessage);
            }
            // Check thời gian
            if (serviceDTO.Time <= 0)
            {
                string errorMessage = "Thời gian phải lớn hơn 0!";
                return BadRequest(errorMessage);
            }
           
            var serviceCategory = _context.ServiceCategories.FirstOrDefault(s => s.SerCategoriesId == serviceDTO.SerCategoriesId);
            if (serviceCategory == null)
            {
                string errorMessage = "Loại dịch vụ không tồn tại!";
                return BadRequest(errorMessage);
            }
            try
            {
                var service = _context.Services
                .Include(a => a.SerCategories)
                .FirstOrDefault(p => p.ServiceId == serviceId);

                if (service == null)
                {
                    return BadRequest("Không tìm thấy dịch vụ bạn chọn.");
                }

                service.ServiceName = serviceDTO.ServiceName;
                service.Desciptions = serviceDTO.Desciptions;
                service.Price = serviceDTO.Price;
                service.Picture = serviceDTO.Picture;
                service.Status = serviceDTO.Status;
                service.Time = serviceDTO.Time;
                service.SerCategoriesId = serviceDTO.SerCategoriesId;

                _context.Update(service);
                await _context.SaveChangesAsync();
                return Ok("Cập nhât dịch vụ thành công!");
            }
            
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
            
        }

        [HttpPut("ChangeStatusService")]
        public async Task<ActionResult> ChangeStatusService(int ServiceId, bool status)
        {
            try
            {
                var service = await _context.Services.FirstOrDefaultAsync(p => p.ServiceId == ServiceId);
                if (service == null)
                {
                    return BadRequest("Không tìm thấy dịch vụ cần thay đổi.");
                }

                service.Status = status;

                _context.Entry(service).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Cập nhật dịch vụ thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpGet("GetAllServiceWhenCategoryTrue")]
        public IActionResult GetServceWhenCategoryTrue()
        {
            // Filter services based on the status of their associated service categories
            List<Service> services = _context.Services
                .Include(s => s.SerCategories)
                .Where(s => s.SerCategories.Status == true) // Filter based on service category status
                .ToList();

            return Ok(_mapper.Map<List<ServiceDTO>>(services));
        }

        [HttpDelete]
        public IActionResult DeleteServce(int serviceId)
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
