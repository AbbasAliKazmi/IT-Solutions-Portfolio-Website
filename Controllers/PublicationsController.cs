using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Sirf Admin manage kar sake
    public class PublicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PublicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ 1. Get All Publications
        [HttpGet]
        [AllowAnonymous] // Publicly accessible (frontend users ko dikhana hai)
        public async Task<ActionResult<IEnumerable<ResearchPublication>>> GetPublications()
        {
            return await _context.ResearchPublications.ToListAsync();
        }

        // ✅ 2. Get Publication by Id
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResearchPublication>> GetPublication(int id)
        {
            var publication = await _context.ResearchPublications.FindAsync(id);

            if (publication == null)
            {
                return NotFound();
            }

            return publication;
        }

        // ✅ 3. Create Publication
        [HttpPost]
        public async Task<ActionResult<ResearchPublication>> CreatePublication(ResearchPublication publication)
        {
            _context.ResearchPublications.Add(publication);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPublication), new { id = publication.Id }, publication);
        }

        // ✅ 4. Update Publication
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublication(int id, ResearchPublication publication)
        {
            if (id != publication.Id)
            {
                return BadRequest();
            }

            _context.Entry(publication).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublicationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // ✅ 5. Delete Publication
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublication(int id)
        {
            var publication = await _context.ResearchPublications.FindAsync(id);
            if (publication == null)
            {
                return NotFound();
            }

            _context.ResearchPublications.Remove(publication);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PublicationExists(int id)
        {
            return _context.ResearchPublications.Any(e => e.Id == id);
        }
    }
}
