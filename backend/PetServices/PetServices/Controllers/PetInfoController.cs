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
    public class PetInfoController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PetInfoController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<PetInfo> petInfos = _context.PetInfos.ToList();
            return Ok(_mapper.Map<List<PetInfoDTO>>(petInfos));
        }



    }
}
