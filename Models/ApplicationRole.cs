using Microsoft.AspNetCore.Identity;
using System;

namespace ProductAPI.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
