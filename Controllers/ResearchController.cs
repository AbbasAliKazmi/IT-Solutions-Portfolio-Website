using Microsoft.AspNetCore.Mvc;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ResearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        //  Add new research publication
        [HttpPost]
        public async Task<IActionResult> CreateResearch([FromBody] ResearchPublication research)
        {
            _context.ResearchPublications.Add(research);
            await _context.SaveChangesAsync();
            return Ok(research);
        }

        //  Search & Filter
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string? keyword, [FromQuery] string? author, [FromQuery] int? year)
        {
            var query = _context.ResearchPublications.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(r => r.Title.Contains(keyword) || r.Abstract.Contains(keyword));

            if (!string.IsNullOrEmpty(author))
                query = query.Where(r => r.Author.Contains(author));

            if (year.HasValue)
                query = query.Where(r => r.Year == year.Value);

            return Ok(query.ToList());
        }
    }
}
