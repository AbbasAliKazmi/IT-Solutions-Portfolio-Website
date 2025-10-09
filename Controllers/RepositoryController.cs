using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;
using System.Linq;

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

        // ✅ Get all repositories (Role-based filtering)
        [HttpGet]
        [Authorize(Roles = "Admin,Researcher,Student")]
        public IActionResult GetAll()
        {
            var repos = _context.Repositories.ToList();

            // Agar Student login hai to sirf Free wali repos dikhao
            if (User.IsInRole("Student"))
            {
                repos = repos
                    .Where(r => !string.IsNullOrEmpty(r.Type) && r.Type.ToLower() == "free")
                    .ToList();
            }

            return Ok(repos);
        }

        // ✅ Get single repository by id
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Researcher,Student")]
        public IActionResult Get(int id)
        {
            var repo = _context.Repositories.Find(id);
            if (repo == null)
                return NotFound(new { message = "Repository not found." });

            // Agar Student hai to sirf Free repos ka access
            if (User.IsInRole("Student") && repo.Type?.ToLower() != "free")
                return Forbid();

            return Ok(repo);
        }

        // ✅ Create new repository (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] Repository repo)
        {
            if (repo == null)
                return BadRequest(new { message = "Invalid repository data." });

            _context.Repositories.Add(repo);
            _context.SaveChanges();
            return Ok(repo);
        }

        // ✅ Update existing repository (Admin only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] Repository repo)
        {
            var existing = _context.Repositories.Find(id);
            if (existing == null)
                return NotFound(new { message = "Repository not found." });

            existing.Title = repo.Title;
            existing.Description = repo.Description;
            existing.Type = repo.Type;
            existing.GitHubLink = repo.GitHubLink;

            _context.SaveChanges();
            return Ok(existing);
        }

        // ✅ Delete repository (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var repo = _context.Repositories.Find(id);
            if (repo == null)
                return NotFound(new { message = "Repository not found." });

            _context.Repositories.Remove(repo);
            _context.SaveChanges();
            return Ok(new { message = "Repository deleted successfully." });
        }
    }
}
