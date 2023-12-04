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
        [HttpGet("PetID/{id}")]
        public IActionResult GetById(int id)
        {
            PetInfo pet = _context.PetInfos
                .FirstOrDefault(c => c.PetInfoId == id);
            return Ok(_mapper.Map<PetInfoDTO>(pet));
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

        [HttpPut("UpdatePet")]
        public IActionResult EditInfoPet(int id, [FromBody] PetInfoDTO petInfoForm)
        {
            var pet = _context.PetInfos
               .FirstOrDefault(p => p.PetInfoId == id);

            if (pet == null)
            {
                return NotFound();
            }

            pet.PetInfoId = petInfoForm.PetInfoId;
            pet.PetName = petInfoForm.PetName;
            pet.ImagePet = petInfoForm.ImagePet;
            pet.Species = petInfoForm.Species;
            pet.Gender = petInfoForm.Gender;
            pet.Descriptions = petInfoForm.Descriptions;
            pet.Weight = petInfoForm.Weight;
            pet.UserInfo = pet.UserInfo;
            pet.Dob = petInfoForm.Dob;

            try
            {
                _context.Entry(pet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
            return Ok(pet);
        }
        [HttpDelete]
        public IActionResult DeleteServce(int petId)
        {
            var pet = _context.PetInfos.FirstOrDefault(p => p.PetInfoId == petId);
            if (petId == null)
            {
                return NotFound();
            }
            try
            {
                _context.PetInfos.Remove(pet);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
            return Ok(petId);
        }


    }
}
