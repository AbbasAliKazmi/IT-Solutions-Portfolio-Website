namespace ProductAPI.Models
{
    public class Repository
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = "Free"; // For access-level
        public string GitHubURL { get; set; } = string.Empty;
        public string License { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string PreviewURL { get; set; } = string.Empty;
        public bool IsPremium { get; set; } = false;
    }
}
