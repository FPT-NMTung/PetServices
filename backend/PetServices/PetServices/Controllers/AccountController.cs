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
using System.Security.Cryptography;
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

        // hash password
        private static string MD5Hash(string text)
        {
            MD5 md5 = MD5.Create();
            /*Chuyen string ve byte*/
            byte[] inputBytes = Encoding.ASCII.GetBytes(text);
            /*ma hoa byte*/
            byte[] result = md5.ComputeHash(inputBytes);
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
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

        /* [HttpPost("Login")]
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
                 .Include(a => a.UserInfo)
                 .Include(a => a.PartnerInfo)
                 .SingleOrDefaultAsync(x => x.Email == login.Email && x.Password == MD5Hash(login.Password));

             if (result != null)
             {
                 var claims = new List<Claim>
                 {
                     new Claim(ClaimTypes.Name, login.Email),
                     new Claim(ClaimTypes.Role, result.Role?.RoleName),
                     new Claim("RoleId", result.Role?.RoleId.ToString()),
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

                 return Ok(new LoginResponse { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token), RoleName = result.Role?.RoleName, Status= result.Status,
                 UserName = result.Role?.RoleName != "PARTNER" ? ( result?.UserInfo.FirstName +" " + result?.UserInfo.LastName) : (result?.PartnerInfo.FirstName + " " + result?.PartnerInfo.LastName), 
                     UserImage = result.Role?.RoleName != "PARTNER" ? result?.UserInfo.ImageUser : result?.PartnerInfo.ImagePartner
                 });
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
         }*/

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
                .Include(a => a.UserInfo)
                .Include(a => a.PartnerInfo)
                .SingleOrDefaultAsync(x => x.Email == login.Email);

            if (result != null && BCrypt.Net.BCrypt.Verify(login.Password, result.Password))
            {
                if (!result.Status)
                {
                    string errorMessage = "Tài khoản chưa được kích hoạt.";
                    return BadRequest(errorMessage);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, login.Email),
                    new Claim(ClaimTypes.Role, result.Role?.RoleName),
                    new Claim("RoleId", result.Role?.RoleId.ToString()),
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

                return Ok(new LoginResponse
                {
                    Successful = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RoleName = result.Role?.RoleName,
                    Status = result.Status,
                    UserName = result.Role?.RoleName != "PARTNER" ? (result?.UserInfo.FirstName + " " + result?.UserInfo.LastName) : (result?.PartnerInfo.FirstName + " " + result?.PartnerInfo.LastName),
                    UserImage = result.Role?.RoleName != "PARTNER" ? result?.UserInfo.ImageUser : result?.PartnerInfo.ImagePartner
                });
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

            // check Họ
            if (string.IsNullOrWhiteSpace(registerDto.FirstName))
            {
                string errorMessage = "Họ không được để trống!";
                return BadRequest(errorMessage);
            }
            string firstName = registerDto.FirstName;
            if (!Regex.IsMatch(firstName, "^[a-zA-ZÀ-Ỹà-ỹ ]+$"))
            {
                string errorMessage = "Họ chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.";
                return BadRequest(errorMessage);
            }
            // check Tên
            if (string.IsNullOrWhiteSpace(registerDto.LastName))
            {
                string errorMessage = "Tên không được để trống!";
                return BadRequest(errorMessage);
            }
            string lastName = registerDto.LastName;
            if (!Regex.IsMatch(lastName, "^[a-zA-ZÀ-Ỹà-ỹ ]+$"))
            {
                string errorMessage = "Tên chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.";
                return BadRequest(errorMessage);
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
                CreateDate = DateTime.Now,
                //Password = registerDto.Password,
                //Password = MD5Hash(registerDto.Password),
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password), 
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
        public async Task<IActionResult> RegisterPartner([FromBody] RegisterPartnerDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // check Email
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
            // check Phone
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
            // check Password
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
            // check ImageCertificate
            if (string.IsNullOrWhiteSpace(registerDto.ImageCertificate))
            {
                string errorMessage = "Bạn cần cung cấp hình ảnh chứng chỉ!";
                return BadRequest(errorMessage);
            }
            else if (registerDto.ImageCertificate.Contains(" "))
            {
                string errorMessage = "URL ảnh không chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            // check Họ
            if (string.IsNullOrWhiteSpace(registerDto.FirstName))
            {
                string errorMessage = "Họ không được để trống!";
                return BadRequest(errorMessage);
            }
            string firstName = registerDto.FirstName;
            if (!Regex.IsMatch(firstName, "^[a-zA-ZÀ-Ỹà-ỹ ]+$"))
            {
                string errorMessage = "Họ chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.";
                return BadRequest(errorMessage);
            }
            // check Tên
            if (string.IsNullOrWhiteSpace(registerDto.LastName))
            {
                string errorMessage = "Tên không được để trống!";
                return BadRequest(errorMessage);
            }
            string lastName = registerDto.LastName;
            if (!Regex.IsMatch(lastName, "^[a-zA-ZÀ-Ỹà-ỹ ]+$"))
            {
                string errorMessage = "Tên chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.";
                return BadRequest(errorMessage);
            }
            // check Tỉnh
            if (string.IsNullOrWhiteSpace(registerDto.Province))
            {
                string errorMessage = "Tỉnh không được để trống!";
                return BadRequest(errorMessage);
            }
            string province = registerDto.Province;
            if (!Regex.IsMatch(province, "^[a-zA-ZÀ-Ỹà-ỹ ]+$"))
            {
                string errorMessage = "Tỉnh chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.";
                return BadRequest(errorMessage);
            }
            // check Huyện
            if (string.IsNullOrWhiteSpace(registerDto.District))
            {
                string errorMessage = "Huyện không được để trống!";
                return BadRequest(errorMessage);
            }
            string district = registerDto.District;
            if (!Regex.IsMatch(district, "^[a-zA-ZÀ-Ỹà-ỹ ]+$"))
            {
                string errorMessage = "Huyện chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.";
                return BadRequest(errorMessage);
            }
            // check Xã
            if (string.IsNullOrWhiteSpace(registerDto.Commune))
            {
                string errorMessage = "Xã không được để trống!";
                return BadRequest(errorMessage);
            }
            string commune = registerDto.Commune;
            if (!Regex.IsMatch(commune, "^[a-zA-ZÀ-Ỹà-ỹ ]+$"))
            {
                string errorMessage = "Xã chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.";
                return BadRequest(errorMessage);
            }

            // trùng email
            if (_context.Accounts.Any(a => a.Email == registerDto.Email))
            {
                return Conflict("Email đã được đăng ký");
            }

            var newAccount = new Account
            {
                Email = registerDto.Email,
                //Password = registerDto.Password,
                //Password = MD5Hash(registerDto.Password),
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password), 
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
                Province = registerDto.Province,
                District = registerDto.District,
                Commune = registerDto.Commune,
                Address = registerDto.Address,
                CardNumber = registerDto.CardNumber,
                ImageCertificate = registerDto.ImageCertificate

            };

            _context.Update(newAccount);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký thành công! Vui lòng chờ đợi quản lý xác nhận tài khoản của bạn trước khi đăng nhập");
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
                    string newPass = convert.Substring(0, 15);

                    Account account = await _context.Accounts.SingleAsync(i => i.AccountId == result.AccountId);
                    account.Password = BCrypt.Net.BCrypt.HashPassword(newPass);

                    _context.Entry(account).State = EntityState.Modified;
                    _context.SaveChanges();

                    var json = new { Email = account.Email, NewPassword = newPass };
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

            if (BCrypt.Net.BCrypt.Verify(oldpassword, account.Password))
            {
                // Hash the new password before storing it
                string hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(newpassword);

                account.Password = hashedNewPassword;

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
