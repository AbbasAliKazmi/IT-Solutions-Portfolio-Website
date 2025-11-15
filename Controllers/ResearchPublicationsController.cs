using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Data;
using ProductAPI.Models;
using System.Linq;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Sirf Admin access
    public class ResearchPublicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResearchPublicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Get all
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _context.ResearchPublications.ToList();
            return Ok(list);
        }

        // ✅ Get by ID
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var publication = _context.ResearchPublications.Find(id);
            if (publication == null)
                return NotFound();
            return Ok(publication);
        }

        // ✅ Create
// ✅ Create
[HttpPost]
public async Task<IActionResult> Create([FromBody] ResearchPublication pub)
{
    try
    {
        // Fix: Ensure PublishedAt always has a value
        pub.PublishedAt ??= DateTime.UtcNow;

        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid data", details = ModelState });

        _context.ResearchPublications.Add(pub);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Publication created", data = pub });
    }
    catch (Exception ex)
    {
        // IMPORTANT: Return JSON instead of HTML error page
        return StatusCode(500, new { error = ex.Message });
    }
}


        // ✅ Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ResearchPublication pub)
        {
            var existing = _context.ResearchPublications.Find(id);
            if (existing == null)
                return NotFound();

            existing.Title = pub.Title;
            existing.Abstract = pub.Abstract;
            existing.Author = pub.Author;
            existing.Category = pub.Category;
            existing.Year = pub.Year;
            existing.PublishedAt = pub.PublishedAt;
            existing.PdfUrl = pub.PdfUrl;

            await _context.SaveChangesAsync();   // 🔥 async save
            return Ok(existing);
        }

        // ✅ Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pub = _context.ResearchPublications.Find(id);
            if (pub == null)
                return NotFound();

            _context.ResearchPublications.Remove(pub);
            await _context.SaveChangesAsync();   // 🔥 async save
            return Ok();
        }
    }
}
