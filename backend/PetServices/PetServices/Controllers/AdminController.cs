using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Models;
using System.Data;
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
                            .Where(a => a.RoleId != 1)
                            .ToListAsync();

            var accountsViewModel = _mapper.Map<List<AccountByAdminDTO>>(accounts);

            return Ok(accountsViewModel);
        }
        [HttpGet("{methodName}")]
        public IActionResult GetMethod(string methodName)
        {
            List<Account> acc = _context.Accounts
                .Where(c => c.Email == methodName)
                .ToList();
            return Ok(_mapper.Map<List<UpdateAccountDTO>>(acc));
        }
        [HttpPut("UpdateAccount")]
        public async Task<ActionResult> UpdateAccount(AccountByAdminDTO accountchange)
        {
            try
            {
                var account = await _context.Accounts
                            .Include(a => a.UserInfo)
                            .Include(a => a.PartnerInfo)
                            .FirstOrDefaultAsync(a => a.Email == accountchange.Email);
                if (account == null)
                {
                    return BadRequest("Không tìm thấy tài khoản");
                }

                else if (account.RoleId == accountchange.RoleId && account.Status == accountchange.Status)
                {
                    return BadRequest("Bạn không có gì thay đổi so với ban đầu.");
                }

                else
                {
                    if (account.PartnerInfoId == null && accountchange.RoleId == 4)
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
                            Dob = account.UserInfo?.Dob ?? null,
                        };
                    }

                    if (account.PartnerInfoId != null && accountchange.RoleId == 4)
                    {
                        account.PartnerInfo.FirstName = account.UserInfo?.FirstName ?? null;
                        account.PartnerInfo.LastName = account.UserInfo?.LastName ?? null;
                        account.PartnerInfo.Phone = account.UserInfo?.Phone ?? null;
                        account.PartnerInfo.Province = account.UserInfo?.Province ?? null;
                        account.PartnerInfo.District = account.UserInfo?.District ?? null;
                        account.PartnerInfo.Commune = account.UserInfo?.Commune ?? null;
                        account.PartnerInfo.Address = account.UserInfo?.Address ?? null;
                        account.PartnerInfo.Descriptions = account.UserInfo?.Descriptions ?? null;
                        account.PartnerInfo.ImagePartner = account.UserInfo?.ImageUser ?? null;
                        account.PartnerInfo.Dob = account.UserInfo?.Dob ?? null;
                    }

                    if (account.UserInfoId == null && accountchange.RoleId != 4)
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
                            Dob = account.PartnerInfo?.Dob ?? null,
                        };
                    }


                    if (account.UserInfoId != null && accountchange.RoleId != 4)
                    {
                        account.UserInfo.FirstName = account.PartnerInfo?.FirstName ?? null;
                        account.UserInfo.LastName = account.UserInfo?.LastName ?? null;
                        account.UserInfo.Phone = account.PartnerInfo?.Phone ?? null;
                        account.UserInfo.Province = account.PartnerInfo?.Province ?? null;
                        account.UserInfo.District = account.PartnerInfo?.District ?? null;
                        account.UserInfo.Commune = account.PartnerInfo?.Commune ?? null;
                        account.UserInfo.Address = account.PartnerInfo?.Address ?? null;
                        account.UserInfo.Descriptions = account.PartnerInfo?.Descriptions ?? null;
                        account.UserInfo.ImageUser = account.PartnerInfo?.ImagePartner ?? null;
                        account.UserInfo.Dob = account.PartnerInfo?.Dob ?? null;
                    }

                    account.RoleId = accountchange.RoleId;
                    account.Status = accountchange.Status;

                    _context.Entry(account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.Update(account);
                    await _context.SaveChangesAsync();


                    return Ok(account);
                }
            }
            catch (Exception ex)
            {
                return NotFound($"Đã xảy ra lỗi: {ex.Message}");
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
                ModelState.AddModelError("Mật khẩu không hợp lệ", "Mật khẩu cần tối thiểu 8 ký tự!");
                return BadRequest(ModelState);
            }

            var newAcc = new Account
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
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
        [HttpGet("UpdateOrderStatusReceived")]
        public async Task<IActionResult> UpdateOrderStatusReceived(int orderId)
        {
            try
            {
                Order order = await _context.Orders
                    .Include(b => b.UserInfo)
                    .Include(b => b.BookingServicesDetails)
                    .ThenInclude(bs => bs.Service)
                    .SingleOrDefaultAsync(b => b.OrderId == orderId);
                order.OrderStatus = "Received";
                _context.Update(order);
                await _context.SaveChangesAsync();

                return Ok(_mapper.Map<OrdersDTO>(order));

            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }
    }
}
