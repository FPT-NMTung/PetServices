using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetServices.DTO;
using PetServices.Models;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReasonController : ControllerBase
    {
        public PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;
        public ReasonController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            List<Reason> reasons = _context.Reasons.ToList();
            return Ok(_mapper.Map<List<ReasonDTO>>(reasons));
        }
        [HttpGet("ReasonId/{id}")]
        public IActionResult GetById(int id)
        {
            List<Reason> reasons = _context.Reasons
                .Where(c => c.ReasonId == id)
                .ToList();
            return Ok(_mapper.Map<List<ReasonDTO>>(reasons));
        }
    }
}
