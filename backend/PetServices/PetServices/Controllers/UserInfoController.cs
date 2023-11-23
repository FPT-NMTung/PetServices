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
    public class UserInfoController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserInfoController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<UserInfo> userInfo = _context.UserInfos.ToList();
            return Ok(_mapper.Map<List<UserInfoDTO>>(userInfo));
        }

        [HttpGet("{email}")]
        public IActionResult Get(string email)
        {
            //Account roles admin, manager, customer 
            Account account = _context.Accounts.Include(a => a.UserInfo).Include(a => a.Role).FirstOrDefault(a => a.Email == email);
            if (account == null)
            {
                return NotFound("Tài khoản không tồn tài");
            }
            // Account role partner 
            if (account.Role.RoleName == "PARTNER")
            {
                account = _context.Accounts.Include(a => a.PartnerInfo).Include(a => a.Role).FirstOrDefault(a => a.Email == email);
            }
            return Ok(_mapper.Map<AccountInfo>(account));
        }

        [HttpPut("updateInfo")]
        public async Task<IActionResult> EditProfile(string email, [FromBody] EditUserInfo updateInfo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingAccount = await _context.Accounts.Include(a => a.UserInfo).SingleOrDefaultAsync(a => a.Email == email);
                if (existingAccount == null)
                {
                    return NotFound("Tài khoản không tồn tài");
                }
                existingAccount.UserInfo.LastName = updateInfo.LastName;
                existingAccount.UserInfo.FirstName = updateInfo.FirstName;
                existingAccount.UserInfo.Phone = updateInfo.Phone;
                existingAccount.UserInfo.Province = updateInfo.Province;
                existingAccount.UserInfo.District = updateInfo.District;
                existingAccount.UserInfo.Commune = updateInfo.Commune;
                existingAccount.UserInfo.Address = updateInfo.Address;
                existingAccount.UserInfo.Descriptions = updateInfo.Descriptions;
                existingAccount.UserInfo.ImageUser = updateInfo.ImageUser;
                existingAccount.UserInfo.Dob = updateInfo.Dob;

                _context.Accounts.Update(existingAccount);
                await _context.SaveChangesAsync();

                return Ok("Profile updated successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
