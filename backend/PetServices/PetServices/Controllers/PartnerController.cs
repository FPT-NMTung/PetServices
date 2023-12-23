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
        [HttpGet("GetAllPartner")]
        public IActionResult GetAllPartner()
        {
            List<PartnerInfo> PartnerInfo = _context.PartnerInfos.ToList();
            return Ok(_mapper.Map<List<PartnerInfoDTO>>(PartnerInfo));
        }
        [HttpGet("PartnerInfoId")]
        public IActionResult GetPartner(int PartnerInfoId)
        {
            PartnerInfo PartnerInfo = _context.PartnerInfos.FirstOrDefault(p => p.PartnerInfoId == PartnerInfoId);
            return Ok(_mapper.Map<PartnerInfoDTO>(PartnerInfo));
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
            Account account = _context.Accounts.Include(a => a.PartnerInfo).Include(a => a.Role).FirstOrDefault(a => a.Email == email && a.Role.RoleName == "PARTNER");
            if (account == null)
            {
                return NotFound("Tài khoản không tồn tài");
            }
            return Ok(_mapper.Map<AccountInfo>(account));
        }

        [HttpGet("GetOrderPartner")]
        public IActionResult GetOrderPartner(int month, int year)
        {
            var partners = _context.Accounts.Where(a => a.RoleId == 4).ToList();
            var orderPartnersList = new List<OrderPartnerDTO>();

            int stt = 1;

            foreach (var partner in partners)
            {
                var partnerInfo = _context.PartnerInfos.Where(p => p.PartnerInfoId == partner.PartnerInfoId).FirstOrDefault();

                if (partnerInfo != null)
                {
                    var orders = _context.BookingServicesDetails.Where(p => p.PartnerInfoId == partner.PartnerInfoId && p.StartTime.Value.Month == month && p.StartTime.Value.Year == year).ToList();

                    var orderPartner = new OrderPartnerDTO
                    {
                        Stt = stt,
                        PartnerId = partner.PartnerInfoId ?? 0,
                        ImagePartner = partnerInfo.ImagePartner,
                        NamePartner = partnerInfo.FirstName + " " + partnerInfo.LastName,
                        TotalOrder = orders.Count().ToString(),
                        TotalSalary = orders.Sum(p => p.PriceService),
                    };

                    orderPartnersList.Add(orderPartner);
                    stt++;
                }
            }

            return Ok(orderPartnersList);
        }

        [HttpGet("GetOrderDetailPartner")]
        public IActionResult GetOrderDetailPartner(int PartnerId, int month, int year)
        {
            var orders = _context.BookingServicesDetails.Where(p => p.PartnerInfoId == PartnerId && p.StartTime.Value.Month == month && p.StartTime.Value.Year == year).ToList();
            var orderPartnersList = new List<OrderPartnerDTO>();

            int stt = 1;

            foreach (var order in orders)
            {
                var service = _context.Services.FirstOrDefault(s => s.ServiceId == order.ServiceId);

                var orderPartner = new OrderPartnerDTO
                {
                    Stt = stt,
                    PartnerId = PartnerId,
                    ImagePartner = service.Picture,
                    NamePartner = service.ServiceName,
                    TotalOrder = order.StartTime.ToString(),
                    TotalSalary = order.PriceService,
                };

                orderPartnersList.Add(orderPartner);
                stt++;
            }

            return Ok(orderPartnersList);
        }

        [HttpPut("updateAccount")]
        public async Task<IActionResult> UpdateAccountStatus(string email)
        {
            try
            {
                var existingAccount = await _context.Accounts
                    .Include(a => a.PartnerInfo)
                    .SingleOrDefaultAsync(a => a.Email == email);

                if (existingAccount == null)
                {
                    return NotFound("Tài khoản không tồn tại");
                }
                existingAccount.Status = !existingAccount.Status;
                _context.SaveChanges();
                return Ok(existingAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        
        [HttpPut("UpdateLocation")]
        public IActionResult UpdateLocation([FromBody] PartnerLocationDTO partnerDTO, string email)
        {
            try
            {
                var partnerAccount = _context.Accounts
                    .Include(a => a.PartnerInfo)
                    .Include(a => a.Role)
                    .FirstOrDefault(a => a.Email == email && a.Role.RoleName == "PARTNER");

                if (partnerAccount == null)
                {
                    return NotFound("Tài khoản không tồn tại hoặc không phải là đối tác");
                }

                partnerAccount.PartnerInfo.Lat = partnerDTO.Lat; 
                partnerAccount.PartnerInfo.Lng = partnerDTO.Lng;

                _context.SaveChanges();

                return Ok(_mapper.Map<PartnerLocationDTO>(partnerAccount.PartnerInfo));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("updateInfo")]
        public async Task<IActionResult> EditProfile(string email, [FromBody] EditPartnerInfo updateInfo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingAccount = await _context.Accounts.Include(a => a.PartnerInfo).SingleOrDefaultAsync(a => a.Email == email);
                if (existingAccount == null)
                {
                    return NotFound("Tài khoản không tồn tài");
                }
                existingAccount.PartnerInfo.LastName = updateInfo.LastName;
                existingAccount.PartnerInfo.FirstName = updateInfo.FirstName;
                existingAccount.PartnerInfo.Phone = updateInfo.Phone;
                existingAccount.PartnerInfo.Province = updateInfo.Province;
                existingAccount.PartnerInfo.District = updateInfo.District;
                existingAccount.PartnerInfo.Commune = updateInfo.Commune;
                existingAccount.PartnerInfo.CardNumber = updateInfo.CardNumber;
                existingAccount.PartnerInfo.CardName = updateInfo.CardName;
                existingAccount.PartnerInfo.Address = updateInfo.Address;
                existingAccount.PartnerInfo.Descriptions = updateInfo.Descriptions;
                existingAccount.PartnerInfo.ImagePartner = updateInfo.ImagePartner;
                existingAccount.PartnerInfo.ImageCertificate = updateInfo.ImageCertificate;
                existingAccount.PartnerInfo.Lat = updateInfo.Lat;
                existingAccount.PartnerInfo.Lng = updateInfo.Lng;
                existingAccount.PartnerInfo.Dob = updateInfo.Dob;

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
