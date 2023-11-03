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

        [HttpGet("GetAllRoomCustomer")]
        public async Task<ActionResult> GetAllRoomCustomer()
        {
            var rooms = await _context.Rooms.Include(r => r.RoomCategories).Where(r => r.Status == true).ToListAsync();
            return Ok(_mapper.Map<List<RoomDTO>>(rooms));
        }

        [HttpGet("GetServiceInRoom")]
        public async Task<ActionResult> GetServiceInRoom(int roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.Services)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            var services = _mapper.Map<List<ServiceDTO>>(room.Services.Where(s => s.Status == true).ToList());

            return Ok(services);
        }

        [HttpGet("GetServiceOutRoom")]
        public async Task<ActionResult> GetServiceOutRoom(int roomId)
        {
            var allServices = await _context.Services.ToListAsync();

            var room = await _context.Rooms
                .Include(r => r.Services)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            var servicesInRoom = room?.Services.Select(s => s.ServiceId).ToList();

            var remainingServices = allServices.Where(s => !servicesInRoom.Contains(s.ServiceId))
                                               .Select(service => _mapper.Map<ServiceDTO>(service))
                                               .ToList();

            var services = _mapper.Map<List<ServiceDTO>>(remainingServices.Where(s => s.Status == true).ToList());

            return Ok(services);
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
            var roomCategory = await _context.RoomCategories.Where(r => r.Status == true)
                .ToListAsync();

            return Ok(_mapper.Map<List<RoomCategoryDTO>>(roomCategory));
        }

        [HttpGet("GetAllService")]
        public async Task<ActionResult> GetAllService()
        {
            var listService = await _context.Services.Where(r => r.Status == true).ToListAsync();

            return Ok(listService);
        }

        [HttpPost("AddRoom")]
        public async Task<ActionResult> AddRoom(RoomDTO roomDTO)
        {
                // check tên phòng
            if (string.IsNullOrWhiteSpace(roomDTO.RoomName))
            {
                string errorMessage = "Tên phòng không được để trống!";
                return BadRequest(errorMessage);
            }
            if (roomDTO.RoomName.Length > 500)
            {
                string errorMessage = "Tên phòng vượt quá số ký tự. Tối đa 500 ký tự!";
                return BadRequest(errorMessage);
            }
            // check mô tả
            if (string.IsNullOrWhiteSpace(roomDTO.Desciptions))
            {
                string errorMessage = "Mô tả không được để trống!";
                return BadRequest(errorMessage);
            }
            // check ảnh
            if (string.IsNullOrWhiteSpace(roomDTO.Picture))
            {
                string errorMessage = "Ảnh phòng không được để trống!";
                return BadRequest(errorMessage);
            }
            else if (roomDTO.Picture.Contains(" "))
            {
                string errorMessage = "URL ảnh không chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }                       
            // check loại phòng           
            var roomcategory = _context.RoomCategories.FirstOrDefault(r => r.RoomCategoriesId == roomDTO.RoomCategoriesId);

            if (roomcategory == null)
            {
                string errorMessage = "Loại phòng không tồn tại!";
                return BadRequest(errorMessage);
            }          


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
                    Status = true,
                    RoomCategoriesId = roomDTO.RoomCategoriesId,
                };

                var services = _context.Services.Where(s => roomDTO.ServiceIds.Contains(s.ServiceId)).ToList();

                foreach (var service in services)
                {
                    newRoom.Services.Add(service);
                }

                await _context.Rooms.AddAsync(newRoom);
                await _context.SaveChangesAsync();

                /*return Ok(_mapper.Map<RoomDTO>(newRoom));*/
                return Ok("Thêm phòng thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPut("UpdateRoom")]
        public async Task<ActionResult> UpdateRoom(RoomDTO roomDTO, int roomId)
        {
            if (string.IsNullOrWhiteSpace(roomDTO.RoomName))
            {
                string errorMessage = "Tên phòng không được để trống!";
                return BadRequest(errorMessage);
            }
            if (roomDTO.RoomName.Length > 500)
            {
                string errorMessage = "Tên phòng vượt quá số ký tự. Tối đa 500 ký tự!";
                return BadRequest(errorMessage);
            }
            // check mô tả
            if (string.IsNullOrWhiteSpace(roomDTO.Desciptions))
            {
                string errorMessage = "Mô tả không được để trống!";
                return BadRequest(errorMessage);
            }
            // check ảnh
            if (string.IsNullOrWhiteSpace(roomDTO.Picture))
            {
                string errorMessage = "Ảnh phòng không được để trống!";
                return BadRequest(errorMessage);
            }
            else if (roomDTO.Picture.Contains(" "))
            {
                string errorMessage = "URL ảnh không chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            // check giá
            if (roomDTO.Price == null)
            {
                string errorMessage = "Giá phòng không được để trống!";
                return BadRequest(errorMessage);
            }
            if (roomDTO.RoomCategoriesId == null)
            {
                string errorMessage = "Loại phòng không được để trống!";
                return BadRequest(errorMessage);
            }

            var roomcategory = _context.RoomCategories.FirstOrDefault(r => r.RoomCategoriesId == roomDTO.RoomCategoriesId);

            if (roomcategory == null)
            {
                string errorMessage = "Loại phòng không tồn tại!";
                return BadRequest(errorMessage);
            }
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
