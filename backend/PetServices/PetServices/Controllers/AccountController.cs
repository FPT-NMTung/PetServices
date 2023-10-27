using AutoMapper;
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
using System.Text.RegularExpressions;

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
            if (string.IsNullOrWhiteSpace(login.Email))
            {
                string errorMessage = "Email không được để trống!";
                return BadRequest(errorMessage);
            }
            else if (login.Email.Contains(" "))
            {
                string errorMessage = "Email không chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            if (!IsValidEmail(login.Email))
            {
                ModelState.AddModelError("Email không hợp lệ", "Email cần có @");
                return BadRequest(ModelState);
            }

            var result = await _context.Accounts
                .Include(a => a.Role)
                .SingleOrDefaultAsync(x => x.Email == login.Email && x.Password == login.Password);

            if (result != null)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login.Email),
                new Claim(ClaimTypes.Role, result.Role?.RoleName),
                new Claim("RoleId", result.Role?.RoleId.ToString())
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

                return Ok(new LoginResponse { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token), RoleName = result.Role?.RoleName });
            }
            else
            {
                string errorMessage = "Đăng nhập không hợp lệ.";
                if (string.IsNullOrWhiteSpace(login.Password))
                {
                    errorMessage = "Mật khẩu không được để trống!";
                }
                else if (login.Password.Length < 8)
                {
                    errorMessage = "Mật khẩu phải có ít nhất 8 ký tự.";
                }
                else if (login.Password.Contains(" "))
                {
                    errorMessage = "Mật khẩu không được chứa khoảng trắng.";
                }
                else if (Regex.IsMatch(login.Password, @"[^a-zA-Z0-9]"))
                {
                    errorMessage = "Mật khẩu không được chứa ký tự đặc biệt.";
                }
                return BadRequest(errorMessage);
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(registerDto.Email))
            {
                string errorMessage = "Email không được để trống!";
                return BadRequest(errorMessage);
            }
            else if (registerDto.Email.Contains(" "))
            {
                string errorMessage = "Email không chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            if (!IsValidEmail(registerDto.Email))
            {
                ModelState.AddModelError("Email không hợp lệ", "Email cần có @");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(registerDto.Phone))
            {
                string errorMessage = "Số điện thoại không được để trống!";
                return BadRequest(errorMessage);
            }
            if (registerDto.Phone.Length != 10)
            {
                string errorMessage = "Số điện thoại phải có 10 ký tự!";
                return BadRequest(errorMessage);
            }
            if (registerDto.Phone.Contains(" "))
            {
                string errorMessage = "Số điện thoại không được chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            if (!registerDto.Phone.StartsWith("0"))
            {
                string errorMessage = "Số điện thoại phải bắt đầu bằng số 0!";
                return BadRequest(errorMessage);
            }
            if (!int.TryParse(registerDto.Phone, out int phoneNumber))
            {
                string errorMessage = "Số điện thoại không phải là số! Bạn cần nhập số điện thoại ở dạng số!";
                return BadRequest(errorMessage);
            }

            if (string.IsNullOrWhiteSpace(registerDto.Password))
            {
                string errorMessage = "Mật khẩu không được để trống!";
                return BadRequest(errorMessage);
            }
            if (registerDto.Password.Length < 8)
            {
                string errorMessage = "Mật khẩu phải có ít nhất 8 ký tự!";
                return BadRequest(errorMessage);
            }
            if (registerDto.Password.Contains(" "))
            {
                string errorMessage = "Mật khẩu không được chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            if (Regex.IsMatch(registerDto.Password, @"[^a-zA-Z0-9]"))
            {
                string errorMessage = "Mật khẩu không được chứa ký tự đặc biệt!";
                return BadRequest(errorMessage);
            }

            if (_context.Accounts.Any(a => a.Email == registerDto.Email))
            {
                return Conflict("Email đã được đăng ký");
            }

            var newAccount = new Account
            {
                Email = registerDto.Email,
                Password = registerDto.Password,
                Status = false,
                RoleId = 2
            };

            await _context.Accounts.AddAsync(newAccount);
            await _context.SaveChangesAsync();

            newAccount.UserInfo = new UserInfo
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Phone = registerDto.Phone,
                Address = registerDto.Address
            };

            _context.Update(newAccount);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký thành công! Đăng nhập để trải nghiệm hệ thống");
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return Regex.IsMatch(email, emailPattern);
        }


        /*private bool IsValidPassword(string password)
        {            
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 8 && !password.Contains(" "); 
        }*/

        private bool IsValidPhone(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone) && phone.Length == 10 && phone.StartsWith("0") && phone.All(char.IsDigit);
        }

        [HttpPost("RegisterPartner")]
        public async Task<IActionResult> RegisterPartner([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra Email
            if (!IsValidEmail(registerDto.Email))
            {
                ModelState.AddModelError("Email không hợp lệ", "Email cần có @");
                return BadRequest(ModelState);
            }

            // Kiểm tra Phone
            if (!IsValidPhone(registerDto.Phone))
            {
                ModelState.AddModelError("Số điện thoại không hợp lệ", "Số điện thoại bắt đầu bằng số 0 và có tổng 10 số");
                return BadRequest(ModelState);
            }

            if (_context.Accounts.Any(a => a.Email == registerDto.Email))
            {
                return Conflict("Email đã được đăng ký");
            }

            var newAccount = new Account
            {
                Email = registerDto.Email,
                Password = registerDto.Password,
                Status = false,
                RoleId = 4
            };

            await _context.Accounts.AddAsync(newAccount);
            await _context.SaveChangesAsync();

            newAccount.PartnerInfo = new PartnerInfo
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                CardNumber = registerDto.CardNumber,
                ImageCertificate = registerDto.ImageCertificate

            };

            _context.Update(newAccount);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký thành công! Đăng nhập để trải nghiệm hệ thống");
        }


        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email is required.");
                }

                var result = await _context.Accounts
                    .Where(i => i.Email == email)
                    .Select(i => new { i.AccountId })
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    string guid = Guid.NewGuid().ToString();
                    string convert = Convert.ToBase64String(Encoding.UTF8.GetBytes(guid));
                    string pass = convert.Substring(0, 15);

                    Account account = await _context.Accounts.SingleAsync(i => i.AccountId == result.AccountId);
                    account.Password = pass;

                    _context.Entry(account).State = EntityState.Modified;
                    _context.SaveChanges();
                    var json = new { Gmail = account.Email, NewPass = pass };
                    return Ok(json);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred.");
            }
        }

        [HttpPost("SendOTP")]
        public async Task<IActionResult> SendOTP([FromBody] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Cần nhập thông tin Email.");
                }

                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);

                if (account == null)
                {
                    return NotFound("Email không tồn tại.");
                }

                Random random = new Random();
                int otp = random.Next(100000, 999999);

                var newOTP = new Otp
                {
                    Code = otp.ToString()
                };

                _context.Otps.Add(newOTP);
                await _context.SaveChangesAsync();

                account.Otpid = newOTP.Otpid;
                _context.Update(account);
                await _context.SaveChangesAsync();

                var response = new { Email = email, OTP = otp };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi.");
            }
        }

        [HttpPost("VerifyOTPAndActivateAccount")]
        public async Task<IActionResult> VerifyOTPAndActivateAccount([FromBody] VerifyOTPModel verifyOTPModel)
        {
            try
            {
                if (string.IsNullOrEmpty(verifyOTPModel.Email) || verifyOTPModel.OTP <= 0)
                {
                    return BadRequest("Email and OTP cần được nhập chính xác.");
                }

                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == verifyOTPModel.Email);

                if (account == null)
                {
                    return NotFound("Email không tồn tại.");
                }

                var otp = await _context.Otps.FirstOrDefaultAsync(o => o.Otpid == account.Otpid && o.Code == verifyOTPModel.OTP.ToString());

                if (otp == null)
                {
                    return BadRequest("Sai OTP.");
                }

                account.Status = true;
                _context.Update(account);
                await _context.SaveChangesAsync();

                return Ok("Tài khoản đã được kích hoạt");
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi.");
            }
        }

        [HttpPut("newpassword")]
        public async Task<IActionResult> ChangePassword(string email, string oldpassword, string newpassword, string confirmnewpassword)
        {
            Account account = await _context.Accounts.Include(a => a.UserInfo).SingleOrDefaultAsync(a => a.Email == email);
            if (account == null)
            {
                return NotFound("Tài khoản không tồn tài");
            }

            if (newpassword != confirmnewpassword)
            {
                return BadRequest("Mật khẩu xác nhận không chính xác");
            }

            if (account.Password == oldpassword)
            {
                account.Password = newpassword;

                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                return Ok("Đổi mật khẩu thành công");
            }
            else
            {
                return BadRequest("Mật khẩu cũ không chính xác");
            }
        }
    }
}
