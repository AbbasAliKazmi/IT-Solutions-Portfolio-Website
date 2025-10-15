using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ✅ Needed for ToListAsync()
using Microsoft.Extensions.Configuration;
using ProductAPI.Data;
using ProductAPI.Models;
using ProductAPI.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        // ✅ Dependency Injection — Verified
        public FeedbackController(ApplicationDbContext context, IEmailSender emailSender, IConfiguration config)
        {
            _context = context;
            _emailSender = emailSender;
            _config = config;
        }

        // 🟢 POST: /api/feedback (Public)
        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] FeedbackDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Message))
            {
                return BadRequest(new { message = "Name, Email, and Message are required." });
            }

            var feedback = new Feedback
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim(),
                Message = dto.Message.Trim(),
                CreatedAt = DateTime.UtcNow,
                Status = "New"
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // 📨 Email notification to admin (non-blocking)
            try
            {
                var adminEmail = _config["Admin:Email"] ?? _config["Smtp:FromEmail"];
                var subject = $"New feedback from {feedback.Name}";
                var body = $@"
                    <p><strong>Name:</strong> {feedback.Name}</p>
                    <p><strong>Email:</strong> {feedback.Email}</p>
                    <p><strong>Message:</strong><br/>{feedback.Message}</p>
                    <p><em>Received: {feedback.CreatedAt:u}</em></p>";

                await _emailSender.SendEmailAsync(adminEmail!, subject, body);
            }
            catch
            {
                // ignore email errors (only log in production)
            }

            return Ok(new { message = "Feedback submitted successfully. Thank you!" });
        }

        // 🔵 GET: /api/feedback (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFeedback()
        {
            // ✅ Async version using EF Core ToListAsync()
            var feedbacks = await _context.Feedbacks
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return Ok(feedbacks);
        }

        // 🟡 PUT: /api/feedback/{id}/resolve (Admin only)
        [HttpPut("{id}/resolve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkResolved(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
                return NotFound();

            feedback.Status = "Resolved";
            await _context.SaveChangesAsync();

            return Ok(feedback);
        }

        // 🔴 DELETE: /api/feedback/{id} (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
                return NotFound();

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // 🧩 DTO for feedback submission
    public class FeedbackDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
