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
            //Account roles admin, manager, customer 
            Account account = _context.Accounts.Include(a => a.UserInfo).ThenInclude(u => u.PetInfos).FirstOrDefault(a => a.Email == email);
            if (account == null)
            {
                return NotFound("Tài khoản không tồn tài");
            }
            return Ok(_mapper.Map<AccountInfo>(account));
        }

            [HttpPost("CreatePet")]
            public async Task<IActionResult> AddPet([FromBody] PetInfoDTO petInfoForm)
            {
                if (petInfoForm == null)
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                var newPet = new PetInfo
                {
                    PetInfoId = petInfoForm.PetInfoId,
                    PetName = petInfoForm.PetName,
                    ImagePet = petInfoForm.ImagePet,
                    Species = petInfoForm.Species,
                    Gender = petInfoForm.Gender,
                    Descriptions = petInfoForm.Descriptions,
                    UserInfoId = petInfoForm.UserInfoId,
                    Weight = petInfoForm.Weight,
                    Dob = petInfoForm.Dob
                };

                _context.PetInfos.Add(newPet);

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(_mapper.Map<PetInfoDTO>(newPet));
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(500, ex.InnerException.Message);
                }
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
