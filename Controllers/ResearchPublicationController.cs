using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResearchPublicationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResearchPublicationController(ApplicationDbContext context)
        {
            _context = context;
        }

        //  Get all publications
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.ResearchPublications.ToList());
        }

        //  Get publication by id
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var pub = _context.ResearchPublications.Find(id);
            if (pub == null) return NotFound();
            return Ok(pub);
        }

        //  Add new publication (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create([FromBody] ResearchPublication pub)
        {
            _context.ResearchPublications.Add(pub);
            _context.SaveChanges();
            return Ok(pub);
        }

        //  Update publication (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ResearchPublication pub)
        {
            var existing = _context.ResearchPublications.Find(id);
            if (existing == null) return NotFound();

            existing.Title = pub.Title;
            existing.Abstract = pub.Abstract;
            existing.Author = pub.Author;
            existing.Category = pub.Category;
            existing.PdfUrl = pub.PdfUrl;
            existing.PublishedAt = pub.PublishedAt;

            _context.SaveChanges();
            return Ok(existing);
        }

        //  Delete publication (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var pub = _context.ResearchPublications.Find(id);
            if (pub == null) return NotFound();

            _context.ResearchPublications.Remove(pub);
            _context.SaveChanges();
            return Ok();
        }
    }
}
