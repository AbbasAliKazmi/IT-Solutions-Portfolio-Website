using Microsoft.AspNetCore.Mvc;
using ProductAPI.Services;
using System.Threading.Tasks;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailTestController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public EmailTestController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpGet("send")]
        public async Task<IActionResult> SendTest()
        {
            await _emailSender.SendEmailAsync(
                "test@example.com",   // koi bhi test email address
                "Test from ProductAPI",
                "<p>This is a test email via Mailtrap.</p>"
            );

            return Ok("Email sent (check Mailtrap inbox)");
        }
    }
}
