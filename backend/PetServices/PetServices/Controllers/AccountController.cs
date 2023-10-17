using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetServices.DTO;
using PetServices.Form;
using PetServices.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AccountController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }


        [Authorize(Roles = "MANAGER")]
        [HttpGet]
        public IActionResult Get()
        {
            List<Account> account = _context.Accounts.ToList();
            return Ok(_mapper.Map<List<AccountDTO>>(account));
        }

        [HttpGet("{email}")]
        public IActionResult Get(string email)
        {
            List<Account> accounts = _context.Accounts.Include(a => a.Role).Where(a => a.Email == email).ToList();
            return Ok(_mapper.Map<List<AccountInfo>>(accounts));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginForm login)
        {
            var result = await _context.Accounts
                .Include(a => a.Role) 
                .SingleOrDefaultAsync(x => x.Email == login.Email && x.Password == login.Password);

            if (result != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, login.Email),
            new Claim(ClaimTypes.Role, result.Role?.RoleName) // Lưu tên vai trò (RoleName) vào mã thông báo
        };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["Jwt:ExpiryInDays"]));
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: expiry,
                    signingCredentials: creds
                );

                return Ok(new LoginResponse { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            else
            {
                return BadRequest("Đăng nhập không hợp lệ.");
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem tài khoản đã tồn tại dựa trên UserName
            if (_context.Accounts.Any(a => a.Email == registerDto.Email))
            {
                return Conflict("Email đã được đăng ký");
            }

            // Tạo một đối tượng Account từ thông tin đăng ký (email và password)
            var newAccount = new Account
            {
                Email = registerDto.Email,
                Password = registerDto.Password,
                Status = false // Hoặc giá trị mặc định khác tùy thuộc vào yêu cầu
            };

            // Thêm Account vào cơ sở dữ liệu
            await _context.Accounts.AddAsync(newAccount);
            await _context.SaveChangesAsync();

            // Gán thông tin UserInfo cho Account
            newAccount.UserInfo = new UserInfo
            {
                // Thiết lập thông tin UserInfo tại đây
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Phone = registerDto.Phone,
                Address = registerDto.Address
            };

            // Lưu thông tin UserInfo vào cơ sở dữ liệu
            _context.Update(newAccount);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký thành công! Đăng nhập để trải nghiệm hệ thống");
        }


        [HttpPut("newpassword")]
        public async Task<IActionResult> ChangePassword(string email, string newpassword)
        {
            Account account = await  _context.Accounts.Include(a => a.UserInfo).SingleOrDefaultAsync(a => a.Email == email);
            if (account == null)
            {
                return NotFound("Tài khoản không tồn tài");
            }
            account.Password = newpassword;

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            return null;
        }
    }
}
