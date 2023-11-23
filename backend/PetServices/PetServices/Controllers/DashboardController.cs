using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.Models;
using System.Globalization;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public DashboardController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("GetAllCustomerByAll")]
        public async Task<ActionResult> GetAllCustomerByAll(int? day, int? month, int? year)
        {
            try
            {
                var query = _context.Accounts.AsQueryable();

                if (year.HasValue)
                {
                    if (day.HasValue && month.HasValue)
                    {
                        query = query.Where(n => n.CreateDate.Value.Day == day && n.CreateDate.Value.Month == month && n.CreateDate.Value.Year == year);
                    }
                    else if (month.HasValue)
                    {
                        query = query.Where(n => n.CreateDate.Value.Month == month && n.CreateDate.Value.Year == year);
                    }
                    else
                    {
                        query = query.Where(n => n.CreateDate.Value.Year == year);
                    }
                }
                else
                {
                    return BadRequest("Hãy cung cấp tham số: year.");
                }

                var newCustomer = await query.ToListAsync();

                return Ok(newCustomer);

            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

        /*[HttpGet("GetAllAccount")]
        public async Task<ActionResult> GetPercentIncreaseCustomerByMonth()
        {
            try
            {
                var accountsInMonth = await _context.Accounts
                    .Where(a => a.CreateDate.Value.Month == month)
                    .ToListAsync();

                return Ok(accountsInMonth);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }*/
    }
}
