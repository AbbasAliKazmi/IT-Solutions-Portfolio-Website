using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestAuthController : ControllerBase
    {
        // Accessible to anyone with valid token
        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var userName = User.Identity?.Name;
            return Ok(new { Message = $"Hello {userName}, you are authenticated!" });
        }

        // Accessible only to Student
        [HttpGet("student")]
        [Authorize(Roles = "Student")]
        public IActionResult StudentOnly()
        {
            return Ok(new { Message = "Welcome Student 🎓" });
        }

        // Accessible only to Researcher
        [HttpGet("researcher")]
        [Authorize(Roles = "Researcher")]
        public IActionResult ResearcherOnly()
        {
            return Ok(new { Message = "Welcome Researcher 🔬" });
        }

        // Accessible only to Admin
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return Ok(new { Message = "Welcome Admin 🛠️" });
        }
    }
}
