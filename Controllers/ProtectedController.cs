using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        [HttpGet("data")]
        [Authorize]  // this line ensures token is valid
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
