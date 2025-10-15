using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ProductContext _productContext;
        private readonly ApplicationDbContext _appContext;

        public SearchController(ProductContext productContext, ApplicationDbContext appContext)
        {
            _productContext = productContext;
            _appContext = appContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string category = "", string keyword = "", string domain = "")
        {
            keyword = (keyword ?? "").Trim();
            domain = (domain ?? "").Trim();
            category = (category ?? "").Trim();

            var results = new List<object>();

            // ---------------------- PRODUCTS ----------------------
            if (string.IsNullOrEmpty(category) || category.Equals("Products", StringComparison.OrdinalIgnoreCase))
            {
                var prods = await _appContext.Products
                    .AsNoTracking()
                    .Where(p =>
                        string.IsNullOrEmpty(keyword) ||
                        (p.Name != null && p.Name.Contains(keyword)) ||
                        (p.ShortDescription != null && p.ShortDescription.Contains(keyword)) ||
                        (p.LongDescription != null && p.LongDescription.Contains(keyword)) ||
                        (p.Domain != null && p.Domain.Contains(keyword))
                    )
                    .ToListAsync();

                results.AddRange(prods.Select(p => new
                {
                    id = p.Id,
                    title = p.Name,
                    snippet = p.ShortDescription,
                    type = "Product",
                    domain = p.Domain,
                    url = $"/products/{p.Id}"
                }));
            }

            // ---------------------- REPOSITORIES ----------------------
            if (string.IsNullOrEmpty(category) || category.Equals("Repositories", StringComparison.OrdinalIgnoreCase))
            {
                var repos = await _appContext.Repositories
                    .AsNoTracking()
                    .Where(r =>
                        string.IsNullOrEmpty(keyword) ||
                        (r.Title != null && r.Title.Contains(keyword)) ||
                        (r.Description != null && r.Description.Contains(keyword)) ||
                        (r.GitHubLink != null && r.GitHubLink.Contains(keyword))
                    )
                    .ToListAsync();

                results.AddRange(repos.Select(r => new
                {
                    id = r.Id,
                    title = r.Title,
                    snippet = r.Description,
                    type = "Repository",
                    domain = r.Type,
                    url = $"/repositories/{r.Id}"
                }));
            }

            // ---------------------- RESEARCH PUBLICATIONS ----------------------
            if (string.IsNullOrEmpty(category)
                || category.Equals("Research", StringComparison.OrdinalIgnoreCase)
                || category.Equals("ResearchPublications", StringComparison.OrdinalIgnoreCase))
            {
                var research = await _appContext.ResearchPublications
                    .AsNoTracking()
                    .Where(r =>
                        string.IsNullOrEmpty(keyword) ||
                        (r.Title != null && r.Title.Contains(keyword)) ||
                        (r.Abstract != null && r.Abstract.Contains(keyword)) ||
                        (r.Author != null && r.Author.Contains(keyword)) ||
                        (r.Category != null && r.Category.Contains(keyword))
                    )
                    .ToListAsync();

                results.AddRange(research.Select(r => new
                {
                    id = r.Id,
                    title = r.Title,
                    snippet = r.Abstract?.Length > 200 ? r.Abstract.Substring(0, 200) + "..." : r.Abstract,
                    type = "ResearchPublication",
                    domain = r.Category,
                    url = $"/research-publications/{r.Id}"
                }));
            }

            // ---------------------- DOMAIN FILTER ----------------------
            if (!string.IsNullOrEmpty(domain))
            {
                results = results.Where(obj =>
                {
                    var dict = obj.GetType()
                                  .GetProperties()
                                  .ToDictionary(p => p.Name, p => p.GetValue(obj)?.ToString() ?? "");
                    return dict.TryGetValue("domain", out var d)
                        && !string.IsNullOrWhiteSpace(d)
                        && d.IndexOf(domain, StringComparison.OrdinalIgnoreCase) >= 0;
                }).ToList();
            }

            return Ok(results.Take(100));
        }
    }
}
