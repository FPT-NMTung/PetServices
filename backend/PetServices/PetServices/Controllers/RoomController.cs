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
    public class RoomController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public RoomController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("GetAllRoom")]
        public async Task<ActionResult> GetAllRoom()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return Ok(_mapper.Map<List<RoomDTO>>(rooms));
        }

        [HttpPost("AddRoom")]
        public async Task<IActionResult> AddRoom(RoomDTO roomDTO)
        {
            try
            {
                var newRoom = new Room
                {
                    RoomName = roomDTO.RoomName,
                    Desciptions = roomDTO.Desciptions,
                    Picture = roomDTO.Picture,
                    Price = roomDTO.Price,

                };

                _context.Services.Add(newServices);

                await _context.SaveChangesAsync();
                return Ok(_mapper.Map<ServiceDTO>(newServices));
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
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
