using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AdminController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("GetAllAccount")]
        public async Task<ActionResult> GetAllAccountTest()
        {
            var acc = await _context.Accounts.ToListAsync();

            return Ok(acc);
        }

        [HttpGet("GetAllUser")]
        public async Task<ActionResult> GetAllUser()
        {
            var user = await _context.UserInfos.ToListAsync();

            return Ok(user);
        }

        [HttpGet("GetAllPartner")]
        public async Task<ActionResult> GetAllPartner()
        {
            var partner = await _context.PartnerInfos.ToListAsync();

            return Ok(partner);
        }

        [HttpGet]
        public async Task<ActionResult> GetRole()
        {
            var roles = await _context.Roles.ToListAsync();

            return Ok(roles);
        }

        [HttpGet("GetAllAccountByAdmin")]
        public async Task<ActionResult> GetAllAccount()
        {
            var accounts = await _context.Accounts
                            .Include(a => a.UserInfo)
                            .Include(a => a.PartnerInfo)
                            .Include(a => a.Role)
                            .ToListAsync();

            var accountsViewModel = _mapper.Map<List<AccountByAdminDTO>>(accounts);

            return Ok(accountsViewModel);
        }

        /*[HttpGet("{ServiceCategorysName}")]
        public IActionResult GetByName(string ServiceCategorysName)
        {
            List<ServiceCategory> serviceCategories = _context.ServiceCategories
                .Where(c => c.SerCategoriesName == ServiceCategorysName)
                .ToList();
            return Ok(_mapper.Map<List<ServiceCategoryDTO>>(serviceCategories));
        }*/
        
        [HttpGet("{methodName}")]
        public IActionResult GetMethod(string methodName)
        {
            List<Account> acc = _context.Accounts
                .Where(c => c.Email == methodName)
                .ToList();
            return Ok(_mapper.Map<List<UpdateAccountDTO>>(acc));
        }

        [HttpPut("UpdateAccount")]
        public async Task<ActionResult> UpdateAccount(string Email, int RoleId, bool Status)
        {
            try
            {
                var account = await _context.Accounts
                            .Include(a => a.UserInfo)
                            .Include(a => a.PartnerInfo)
                            .FirstOrDefaultAsync(a => a.Email == Email);
                if (account == null)
                {
                    return BadRequest("Không tìm thấy tài khoản");
                }

                else if (account.RoleId == RoleId && account.Status == Status)
                {
                    return BadRequest("Bạn không có gì thay đổi so với ban đầu.");
                }

                else
                {
                    if (account.PartnerInfoId == null && RoleId == 4)
                    {
                        account.PartnerInfo = new PartnerInfo
                        {
                            FirstName = account.UserInfo?.FirstName ?? null,
                            LastName = account.UserInfo?.LastName ?? null,
                            Phone = account.UserInfo?.Phone ?? null,
                            Province = account.UserInfo?.Province ?? null,
                            District = account.UserInfo?.District ?? null,
                            Commune = account.UserInfo?.Commune ?? null,
                            Address = account.UserInfo?.Address ?? null,
                            Descriptions = account.UserInfo?.Descriptions ?? null,
                            ImagePartner = account.UserInfo?.ImageUser ?? null,
                        };
                    }

                    else if (account.UserInfoId == null && RoleId != 4)
                    {
                        account.UserInfo = new UserInfo
                        {
                            FirstName = account.PartnerInfo?.FirstName ?? null,
                            LastName = account.PartnerInfo?.LastName ?? null,
                            Phone = account.PartnerInfo?.Phone ?? null,
                            Province = account.PartnerInfo?.Province ?? null,
                            District = account.PartnerInfo?.District ?? null,
                            Commune = account.PartnerInfo?.Commune ?? null,
                            Address = account.PartnerInfo?.Address ?? null,
                            Descriptions = account.PartnerInfo?.Descriptions ?? null,
                            ImageUser = account.PartnerInfo?.ImagePartner ?? null,
                        };
                    }

                    account.RoleId = RoleId;
                    account.Status = Status;

                    _context.Entry(account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.Update(account);
                    await _context.SaveChangesAsync();


                    return Ok(account);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPost("AddAccount")]
        public async Task<ActionResult> AddAccount(string email, string password, int roleId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Accounts.Where(a => a.Email == email).FirstOrDefault() != null)
            {
                return BadRequest("Email đã tồn tại. Vui lòng nhập email khác!");
            }

            if (!IsValidEmail(email))
            {
                return BadRequest("Email không hợp lệ");
            }

            if (!IsValidPassword(password))
            {
                ModelState.AddModelError("Mật khẩu không hợp lệ", "Mật khẩu cần tối thiểu 8 ký tự và không chứa ký tự đặc biệt!");
                return BadRequest(ModelState);
            }

            var newAcc = new Account
            {
                Email = email,
                Password = password,
                Status = true,
                RoleId = roleId
            };

            await _context.Accounts.AddAsync(newAcc);
            await _context.SaveChangesAsync();

            if (roleId == 4)
            {
                newAcc.PartnerInfo = new PartnerInfo();
            }
            else
            {
                newAcc.UserInfo = new UserInfo();
            }

            _context.Update(newAcc);
            await _context.SaveChangesAsync();

            return Ok(newAcc);
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 8 && !password.Contains(" ");
        }
    }
}
