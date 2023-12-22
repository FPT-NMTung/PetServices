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
        public async Task<ActionResult> GetAllBlog()
        {
            var blog = await _context.Blogs.Include(r => r.Tag).ToListAsync();
            return Ok(_mapper.Map<List<BlogDTO>>(blog));
        }


        [HttpGet("BlogID/{id}")]
        public IActionResult GetById(int id)
        {
            Blog blog = _context.Blogs
                .Include(s => s.Tag)
                .FirstOrDefault(c => c.BlogId == id) ;

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
                ImageUrl = blog.ImageUrl,
                TagId=blog.TagId,
                
                

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
            var blog = _context.Blogs.Include(a => a.Tag)
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
            blog.TagId = blogDTO.TagId;
            



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


        [HttpGet("GetBlogsByTagId/{tagId}")]
        public IActionResult GetBlogsByTagId(int tagId)
        {
            var blogs = _context.Blogs
                .Where(b => b.TagId == tagId)
                .Include(b => b.Tag)
                .ToList();

            if (blogs == null || !blogs.Any())
            {
                return NotFound("Không có bài viết nào cho tag này.");
            }

            return Ok(_mapper.Map<List<BlogDTO>>(blogs));
        }
    }
}

