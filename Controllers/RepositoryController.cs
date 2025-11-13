using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;
using ProductAPI.Services; // 👈 For IEmailSender
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepositoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public RepositoryController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // ✅ Get all repositories
        [HttpGet]
        [AllowAnonymous] // temporarily allow all users
        public async Task<IActionResult> GetRepos()
        {
            var repos = await _context.Repositories.ToListAsync();
            return Ok(repos);
        }

        // ✅ Get repository by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRepo(int id)
        {
            var repo = await _context.Repositories.FindAsync(id);
            if (repo == null) return NotFound();
            return Ok(repo);
        }

        // ✅ Create repository (Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Repository repo)
        {
            if (repo == null) return BadRequest("Invalid data");

            _context.Repositories.Add(repo);
            await _context.SaveChangesAsync();
            return Ok(repo);
        }

        // ✅ Update repository (Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Repository repo)
        {
            var existing = await _context.Repositories.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = repo.Title;
            existing.Description = repo.Description;
            existing.GitHubURL = repo.GitHubURL;
            existing.License = repo.License;
            existing.Version = repo.Version;
            existing.Domain = repo.Domain;
            existing.PreviewURL = repo.PreviewURL;
            existing.IsPremium = repo.IsPremium;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // ✅ Delete repository
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var repo = await _context.Repositories.FindAsync(id);
            if (repo == null) return NotFound();

            _context.Repositories.Remove(repo);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Deleted successfully" });
        }

        // ✅ POST: Premium Access Request (User → Email Admin)
        [HttpPost("RequestPremium")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPremium([FromBody] PremiumRequest req)
        {
            if (req == null) return BadRequest();

            var message = $@"
                <h3>Premium Access Request</h3>
                <p>User: {req.UserName} ({req.UserEmail})</p>
                <p>Requested Repo: {req.RepoTitle}</p>
            ";

            await _emailSender.SendEmailAsync("admin@yourdomain.com", "Premium Access Request", message);
            return Ok(new { success = true });
        }
    }

    // ✅ Model for Premium Request
    public class PremiumRequest
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string RepoTitle { get; set; }
    }
}
