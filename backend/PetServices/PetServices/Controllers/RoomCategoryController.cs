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
    public class RoomCategoryController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public RoomCategoryController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("GetAllRoomCategory")]
        public async Task<ActionResult> GetAllRoomCategory()
        {
            var roomCategories = await _context.RoomCategories
                .OrderByDescending(o => o.RoomCategoriesId)
                .ToListAsync();

            return Ok(_mapper.Map<List<RoomCategoryDTO>>(roomCategories));
        }

        [HttpGet("GetRoomCategory/{roomCategoryId}")]
        public async Task<ActionResult> GetRoomCategory(int roomCategoryId)
        {
            var roomCategory = await _context.RoomCategories.FirstOrDefaultAsync(r => r.RoomCategoriesId == roomCategoryId);
            return Ok(_mapper.Map<RoomCategoryDTO>(roomCategory));
        }

        [HttpPost("AddRoomCategory")]
        public async Task<IActionResult> AddRoomCategory(RoomCategoryDTO roomCategoryDTO)
        {
            try
            {
                var newRoomCategory = new RoomCategory
                {
                    RoomCategoriesId = roomCategoryDTO.RoomCategoriesId,
                    RoomCategoriesName = roomCategoryDTO.RoomCategoriesName,
                    Desciptions = roomCategoryDTO.Desciptions,
                    Picture = roomCategoryDTO.Picture,
                    Status = roomCategoryDTO.Status,
                };

                await _context.RoomCategories.AddAsync(newRoomCategory);
                await _context.SaveChangesAsync();

                return Ok(_mapper.Map<RoomCategoryDTO>(newRoomCategory));
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPut("UpdateRoomCategory")]
        public async Task<ActionResult> UpdateRoomCategory(RoomCategoryDTO roomCategoryDTO, int roomCategoryId)
        {
            try
            {
                var roomCategory = await _context.RoomCategories.FirstOrDefaultAsync(p => p.RoomCategoriesId == roomCategoryId);
                if (roomCategory == null)
                {
                    return BadRequest("Không tìm thấy loại phòng cần thay đổi.");
                }

                roomCategory.RoomCategoriesName = roomCategoryDTO.RoomCategoriesName;
                roomCategory.Desciptions = roomCategoryDTO.Desciptions;
                roomCategory.Picture = roomCategoryDTO.Picture;
                roomCategory.Status = roomCategoryDTO.Status;

                _context.Entry(roomCategory).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(roomCategory);

            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPut("ChangeStatusRoomCategory")]
        public async Task<ActionResult> ChangeStatusRoomCategory(int RoomCategoryId, bool status)
        {
            try
            {
                var roomCategory = await _context.RoomCategories.FirstOrDefaultAsync(p => p.RoomCategoriesId == RoomCategoryId);
                if (roomCategory == null)
                {
                    return BadRequest("Không tìm thấy loại phòng cần thay đổi.");
                }

                roomCategory.Status = status;

                _context.Entry(roomCategory).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(roomCategory);
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }
    }
}
