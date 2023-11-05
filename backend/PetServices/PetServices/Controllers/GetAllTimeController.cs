using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetServices.Models;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetAllTimeController : ControllerBase
    {
        private readonly PetServicesContext _context;
        public GetAllTimeController(PetServicesContext context)
        {
            _context = context;
        }

        [HttpGet("uptime-auto")]
        public IActionResult Get()
        {
            return Ok("Chào mừng bạn đến với hệ thống");
        }
    }
}
