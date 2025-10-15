namespace ProductAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        // Old properties
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ProductUrl { get; set; } = string.Empty;

        // New properties
        public string? Name { get; set; }   // Add this
        public decimal Price { get; set; }  // Add this


        public string? LongDescription { get; set; }  // 🔹 Add this

        public string? Domain { get; set; }  // 🔹 Add this
        

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
