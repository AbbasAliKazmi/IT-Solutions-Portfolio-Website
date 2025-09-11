using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductAPI.Data;


namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepositoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RepositoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        //  Get all repositories
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Repositories.ToList());
        }

        //  Get repository by id
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var repo = _context.Repositories.Find(id);
            if (repo == null) return NotFound();
            return Ok(repo);
        }

        //  Add new repository (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create([FromBody] Repository repo)
        {
            _context.Repositories.Add(repo);
            _context.SaveChanges();
            return Ok(repo);
        }

        //  Update repository (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Repository repo)
        {
            var existing = _context.Repositories.Find(id);
            if (existing == null) return NotFound();

            existing.Title = repo.Title;
            existing.Description = repo.Description;
            existing.Type = repo.Type;
            existing.GitHubLink = repo.GitHubLink;

            _context.SaveChanges();
            return Ok(existing);
        }

        //  Delete repository (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var repo = _context.Repositories.Find(id);
            if (repo == null) return NotFound();

            _context.Repositories.Remove(repo);
            _context.SaveChanges();
            return Ok();
        }
    }
}
