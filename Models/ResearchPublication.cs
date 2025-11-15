namespace ProductAPI.Models
{
public class ResearchPublication
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
    public string Category { get; set; }
    public string Abstract { get; set; }
    public string PdfUrl { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? CreatedAt { get; set; }   // ADD THIS
}


}
