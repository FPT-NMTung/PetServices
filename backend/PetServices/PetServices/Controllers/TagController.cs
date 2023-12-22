using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Models;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        /*[Authorize(Roles = "MANAGER")]*/
        public TagController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }


        [HttpGet("GetAllTag")]
        public IActionResult GetTags()
        {
            List<Tag> services = _context.Tags
               .ToList();
            return Ok(_mapper.Map<List<TagDTO>>(services));
        }

        [HttpGet("TagsID/{id}")]
        public IActionResult GetByIdTags(int id)
        {
            Tag tag = _context.Tags
                .FirstOrDefault(c => c.TagId == id);
            return Ok(_mapper.Map<TagDTO>(tag));
        }

        
        [HttpPost("AddTag")]
        public async Task<IActionResult> CreateTag(TagDTO tagDTO)
        {
            if (tagDTO == null)
            {
                return BadRequest("Tag data is missing.");
            }

            var newTag = new Tag
            {
                TagName = tagDTO.TagName,
                Status=tagDTO.Status
            };

            _context.Tags.Add(newTag);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(_mapper.Map<TagDTO>(newTag));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex.InnerException.Message);
            }
        }


        [HttpPut("EditTag")]
        public IActionResult UpdateTag(TagDTO tagDTO, int tagID)
        {
            var existingTag = _context.Tags.FirstOrDefault(p => p.TagId == tagID);
            if (existingTag == null)
            {
                return NotFound();
            }
            existingTag.TagName = tagDTO.TagName;
            existingTag.Status = tagDTO.Status; 

            try
            {
                _context.Tags.Update(existingTag); // Attach the entity and mark it as modified
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }

            return Ok(existingTag);
        }


    }
}
