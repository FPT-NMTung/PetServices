using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Form;
using PetServices.Models;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetInfoController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PetInfoController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Lấy danh sách PetInfo
            List<PetInfo> petInfos = _context.PetInfos.ToList();
            return Ok(_mapper.Map<List<PetInfoDTO>>(petInfos));
        }

        [HttpGet("{email}")]
        public IActionResult Get(string email)
        {
            List<Account> accounts = _context.Accounts
    .Include(a => a.UserInfo)
        .ThenInclude(u => u.PetInfos)
    .Where(a => a.Email == email)
    .ToList();

            if (accounts == null)
            {
                return NotFound("Tài khoản không tồn tại");
            }

            List<AccountInfo> accountInfos = _mapper.Map<List<AccountInfo>>(accounts);
            return Ok(accountInfos);

        }

        [HttpPost]
        public IActionResult AddPet([FromBody] PetInfoForm petInfoForm)
        {
            if (petInfoForm == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }

            // Sử dụng AutoMapper để chuyển đổi từ PetInfoForm thành PetInfo
            var petInfo = _mapper.Map<PetInfo>(petInfoForm);

            // Thêm petInfo mới vào cơ sở dữ liệu
            _context.PetInfos.Add(petInfo);
            _context.SaveChanges();

            // Trả về kết quả thành công và thông tin pet đã được thêm
            return CreatedAtAction("Get", new { id = petInfo.PetInfoId }, petInfo);
        }

        [HttpPut("{id}")]
        public IActionResult EditInfoPet(int id, [FromBody] PetInfoForm petInfoForm)
        {
            if (petInfoForm == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }

            // Tìm thông tin pet cần chỉnh sửa theo PetInfoId
            var existingPetInfo = _context.PetInfos.FirstOrDefault(p => p.PetInfoId == id);

            if (existingPetInfo == null)
            {
                return NotFound("Không tìm thấy thông tin pet");
            }

            // Sử dụng AutoMapper để cập nhật thông tin pet từ petInfoForm
            _mapper.Map(petInfoForm, existingPetInfo);

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            // Trả về kết quả thành công và thông tin pet đã được chỉnh sửa
            return Ok(existingPetInfo);
        }


    }
}
