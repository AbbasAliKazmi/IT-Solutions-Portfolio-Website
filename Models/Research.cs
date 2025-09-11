using System;

namespace ProductAPI.Models
{
    public class Research
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Abstract { get; set; } = null!;

        public string Author { get; set; } = null!;

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

        public string? FilePath { get; set; }

        //  Fix
        public int Year { get; set; }
    }
}
