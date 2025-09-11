namespace ProductAPI.Models
{
    public class Repository
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Type { get; set; } = "Free"; // "Free" ya "Premium"
        public string GitHubLink { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
