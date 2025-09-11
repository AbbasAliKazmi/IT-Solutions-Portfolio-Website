using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] //  Sirf Admin access
    public class ResearchPublicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResearchPublicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //  Get All
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.ResearchPublications.ToList());
        }

        //  Get by ID
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var publication = _context.ResearchPublications.Find(id);
            if (publication == null) return NotFound();
            return Ok(publication);
        }

        //  Create
        [HttpPost]
        public IActionResult Create([FromBody] ResearchPublication pub)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.ResearchPublications.Add(pub);
            _context.SaveChanges();
            return Ok(pub);
        }

        //  Update
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ResearchPublication pub)
        {
            var existing = _context.ResearchPublications.Find(id);
            if (existing == null) return NotFound();

            existing.Title = pub.Title;
            existing.Abstract = pub.Abstract;
            existing.Author = pub.Author;
            existing.Category = pub.Category;
            existing.Year = pub.Year;
            existing.PublishedAt = pub.PublishedAt;
            existing.PdfUrl = pub.PdfUrl;

            _context.SaveChanges();
            return Ok(existing);
        }

        //  Delete
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
