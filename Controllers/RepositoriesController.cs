using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // ❗ Sirf Admin hi in APIs ko use kar sakta hai
    public class RepositoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RepositoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //  GET: api/repositories (sab repos list karega)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Repository>>> GetRepositories()
        {
            return await _context.Repositories.ToListAsync();
        }

        //  GET: api/repositories/5 (id se repo nikalna)
        [HttpGet("{id}")]
        public async Task<ActionResult<Repository>> GetRepository(int id)
        {
            var repo = await _context.Repositories.FindAsync(id);
            if (repo == null) return NotFound();
            return repo;
        }

        //  POST: api/repositories (naya repo add karna)
        [HttpPost]
        public async Task<ActionResult<Repository>> CreateRepository(Repository repo)
        {
            _context.Repositories.Add(repo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRepository), new { id = repo.Id }, repo);
        }

        //  PUT: api/repositories/5 (update repo)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRepository(int id, Repository repo)
        {
            if (id != repo.Id) return BadRequest();

            _context.Entry(repo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //  DELETE: api/repositories/5 (repo delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRepository(int id)
        {
            var repo = await _context.Repositories.FindAsync(id);
            if (repo == null) return NotFound();

            _context.Repositories.Remove(repo);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
