using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Models;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;
        /*[Authorize(Roles = "MANAGER")]*/
        public BlogController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }



        [HttpGet("GetAllBlog")]
        public IActionResult Get()
        {
            List<Blog> blogs = _context.Blogs.ToList();
            return Ok(_mapper.Map<List<BlogDTO>>(blogs));
        }


        [HttpGet("BlogID/{id}")]
        public IActionResult GetById(int id)
        {
            Blog blog = _context.Blogs
                .FirstOrDefault(c => c.BlogId == id);
            return Ok(_mapper.Map<BlogDTO>(blog));
        }

        [HttpPost("CreateBlog")]
        public async Task<IActionResult> CreateBlog(BlogDTO blog)
        {
            if (blog == null)
            {
                return BadRequest("Blog data is missing.");
            }

            var newBlog = new Blog
            {
                BlogId = blog.BlogId,
                PageTile = blog.PageTile,
                Heading = blog.Heading,
                Description = blog.Description,
                PublisheDate = blog.PublisheDate,
                Content = blog.Content,
                Status=blog.Status,
                ImageUrl = blog.ImageUrl

            };

            _context.Blogs.Add(newBlog);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(_mapper.Map<BlogDTO>(newBlog));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex.InnerException.Message);
            }
        }


        [HttpPut("UpdateBlog")]
        public IActionResult Update(BlogDTO blogDTO, int blogId)
        {
            var blog = _context.Blogs
                .FirstOrDefault(p => p.BlogId == blogId);

            if (blog == null)
            {
                return NotFound();
            }

            blog.PageTile = blogDTO.PageTile;
            blog.Description = blogDTO.Description;
            blog.ImageUrl = blogDTO.ImageUrl;
            blog.PublisheDate = blogDTO.PublisheDate;
            blog.Content = blogDTO.Content;
            blog.Status = blogDTO.Status;
            blog.Heading = blogDTO.Heading;

            try
            {
                _context.Entry(blog).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
            return Ok(blog);
        }


        [HttpDelete]
        public IActionResult Delete(int blogId)
        {
            var blog = _context.Blogs.FirstOrDefault(p => p.BlogId == blogId);
            if (blog == null)
            {
                return NotFound();
            }
            try
            {
                _context.Blogs.Remove(blog);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
            return Ok(blog);
        }
    }
}

