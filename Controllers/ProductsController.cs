using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductAPI.Data;
using System.IO;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductContext _context;

        public ProductsController(ProductContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            return _context.Products.ToList();
        }

        // POST: api/products
        [HttpPost]
        public IActionResult Post([FromBody] Product product)
        {
            if (product == null)
                return BadRequest();

            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(product);
        }

        // ✅ POST: api/products/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] FileUploadModel model)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest("No file selected.");

            // ✅ ALLOWED EXTENSIONS: images & video
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".mp4", ".webm" };
            var extension = Path.GetExtension(model.File.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Unsupported file type.");

            var fileName = Guid.NewGuid() + extension;

            // ✅ Save to ASP.NET project folder
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            // ✅ Also copy to WordPress folder (XAMPP path)
            var wordpressPath = @"C:\xampp\htdocs\Weburio\wordpress\content";
            var destinationPath = Path.Combine(wordpressPath, fileName);
            try
            {
                System.IO.File.Copy(filePath, destinationPath, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to copy to WordPress folder: " + ex.Message);
            }

            // ✅ Return relative path for frontend
            var publicPath = $"/content/{fileName}";
            return Ok(new { imageUrl = publicPath });
        }
    }
}
