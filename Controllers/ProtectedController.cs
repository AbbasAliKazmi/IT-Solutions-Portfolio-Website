using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        [HttpGet("data")]
        [Authorize]  // Yeh line ensure karti hai k token valid ho
        public IActionResult GetProtectedData()
        {
            return Ok(new
            {
                message = "Ye protected data hai (sirf valid JWT ke saath milega).",
                time = DateTime.Now
            });
        }
    }
}
