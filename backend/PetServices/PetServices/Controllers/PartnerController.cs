using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Form;
using PetServices.Models;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PartnerController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<Account> account = _context.Accounts.Include(a => a.Role).Include(a => a.PartnerInfo).Where(a => a.Role.RoleName == "PARTNER").ToList();
            return Ok(_mapper.Map<List<AccountInfo>>(account));
        }

        [HttpGet("accountNotActive")]
        public IActionResult GetAcountNotActive()
        {
            List<Account> account = _context.Accounts.Include(a => a.Role).Include(a => a.PartnerInfo).Where(a => a.Role.RoleName == "PARTNER" && a.Status == false).ToList();
            return Ok(_mapper.Map<List<AccountInfo>>(account));
        }

        [HttpGet("{email}")]
        public IActionResult Get(string email)
        {
            //Account roles partner
            Account account = _context.Accounts.Include(a => a.PartnerInfo).Include(a => a.Role).FirstOrDefault(a => a.Email == email && a.Role.RoleName == "PARTNER");
            if (account == null)
            {
                return NotFound("Tài khoản không tồn tài");
            }
            return Ok(_mapper.Map<AccountInfo>(account));
        }

        [HttpPut("updateAccount")]
        public async Task<IActionResult> UpdateAccountStatus(string email)
        {
            try
            {
                // Tìm tài khoản dựa trên địa chỉ email
                var existingAccount = await _context.Accounts
                    .Include(a => a.PartnerInfo)
                    .SingleOrDefaultAsync(a => a.Email == email);

                if (existingAccount == null)
                {
                    // Trả về lỗi 404 nếu tài khoản không tồn tại
                    return NotFound("Tài khoản không tồn tại");
                }

                // Đảm bảo thay đổi trạng thái tài khoản (kích hoạt/bỏ kích hoạt)
                existingAccount.Status = !existingAccount.Status;

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();

                // Trả về thông tin tài khoản đã được cập nhật
                return Ok(existingAccount);
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu xảy ra lỗi trong quá trình xử lý
                return StatusCode(500, ex.Message);
            }
        }
    }
}
