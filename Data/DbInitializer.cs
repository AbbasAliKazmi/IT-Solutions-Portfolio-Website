using ProductAPI.Models;

namespace ProductAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ProductContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if any products exist
            if (context.Products.Any())
            {
                return; // DB has been seeded
            }

            var products = new Product[]
            {
                new Product
                {
                    Title = "Smart Scheduler",
                    ShortDescription = "Automates meeting scheduling.",
                    ImageUrl = "https://images.pexels.com/photos/32083393/pexels-photo-32083393.jpeg",
                    ProductUrl = "https://yourdomain.com/products/scheduler"
                },
                new Product
                {
                    Title = "Auto Invoice",
                    ShortDescription = "Generates smart invoices instantly.",
                    ImageUrl = "https://images.pexels.com/photos/374885/pexels-photo-374885.jpeg",
                    ProductUrl = "https://yourdomain.com/products/invoice"
                }
            };

            foreach (var p in products)
            {
                context.Products.Add(p);
            }

            context.SaveChanges();
        }
    }
}
