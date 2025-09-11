namespace ProductAPI.Models
{
    public class ResearchPublication
    {
        public int Id { get; set; }

        // Title of publication
        public string Title { get; set; } = null!;

        // Abstract/summary
        public string Abstract { get; set; } = null!;

        // Single author (agar multiple chahiye to string list bana sakte ho)
        public string Author { get; set; } = null!;   //  Controller me Authors call ho raha hai

        // Category (AI, ML, Blockchain etc.)
        public string Category { get; set; } = null!;  //  Category added

        // Year of publication
        public int Year { get; set; }

        // Date of publishing
        public DateTime PublishedAt { get; set; }

        // PDF file URL/path
        public string PdfUrl { get; set; } = null!;    //  PdfUrl added
    }
}
