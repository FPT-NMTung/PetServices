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
            var rooms = await _context.Rooms.Include(r => r.RoomCategories).ToListAsync();
            return Ok(_mapper.Map<List<RoomDTO>>(rooms));
        }

        [HttpGet("GetRoom/{roomId}")]
        public async Task<ActionResult> GetRoom(int roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.Services)
                .Include(r => r.RoomCategories)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
            {
                return NotFound();
            }

            var roomDto = _mapper.Map<RoomDTO>(room);

            roomDto.ServiceIds = room.Services.Select(s => s.ServiceId).ToList();

            return Ok(roomDto);
        }

        [HttpGet("GetRoomCategory")]
        public async Task<ActionResult> GetRoomCategory()
        {
            var roomCategory = await _context.RoomCategories
                .ToListAsync();

            return Ok(_mapper.Map<List<RoomCategoryDTO>>(roomCategory));
        }

        [HttpGet("GetAllService")]
        public async Task<ActionResult> GetAllService()
        {
            var listService = await _context.Services.ToListAsync();

            return Ok(listService);
        }

        [HttpPost("AddRoom")]
        public async Task<ActionResult> AddRoom(RoomDTO roomDTO)
        {
            try
            {
                var newRoom = new Room
                {
                    RoomId = roomDTO.RoomId,
                    RoomName = roomDTO.RoomName,
                    Desciptions = roomDTO.Desciptions,
                    Picture = roomDTO.Picture,
                    Price = roomDTO.Price,
                    Slot = roomDTO.Slot,
                    Status = roomDTO.Status,
                    RoomCategoriesId = roomDTO.RoomCategoriesId,
                };

                var services = _context.Services.Where(s => roomDTO.ServiceIds.Contains(s.ServiceId)).ToList();

                foreach (var service in services)
                {
                    newRoom.Services.Add(service);
                }

                await _context.Rooms.AddAsync(newRoom);
                await _context.SaveChangesAsync();

                return Ok(_mapper.Map<RoomDTO>(newRoom));
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPut("UpdateRoom")]
        public async Task<ActionResult> UpdateRoom(RoomDTO roomDTO, int roomId)
        {
            try
            {
                var room = await _context.Rooms.Include(r => r.RoomCategories).Include(r => r.Services).FirstOrDefaultAsync(p => p.RoomId == roomId);
                if (room == null)
                {
                    return BadRequest("Không tìm thấy phòng bạn chọn.");
                }

                var servicesToRemove = room.Services.ToList();
                room.Services.Clear();
                _context.SaveChanges();

                room.RoomName = roomDTO.RoomName;
                room.Desciptions = roomDTO.Desciptions;
                room.Picture = roomDTO.Picture;
                room.Price = roomDTO.Price;
                room.Status = roomDTO.Status;
                room.Slot = roomDTO.Slot;
                room.RoomCategoriesId = roomDTO.RoomCategoriesId;

                var services = _context.Services.Where(s => roomDTO.ServiceIds.Contains(s.ServiceId)).ToList();

                foreach (var service in services)
                {
                    room.Services.Add(service);
                }

                _context.Entry(room).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(room);
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPut("ChangeStatusRoom")]
        public async Task<ActionResult> ChangeStatusRoom(int RoomId, bool status)
        {
            try
            {
                var room = await _context.Rooms.FirstOrDefaultAsync(p => p.RoomId == RoomId);
                if (room == null)
                {
                    return BadRequest("Không tìm thấy phòng cần thay đổi.");
                }

                room.Status = status;

                _context.Entry(room).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(room);
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }
    }
}
