using Microsoft.AspNetCore.Identity;
using ProductAPI.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ProductAPI.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            string[] roles = new[] { "Admin", "Student", "Researcher" };

            foreach (var roleName in roles)
            {
                // check by normalized name
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpperInvariant(),
                        Description = $"{roleName} role",
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };

                    var result = await roleManager.CreateAsync(role);
                    Console.WriteLine(result.Succeeded
                        ? $"✅ Role created: {roleName}"
                        : $"❌ Failed creating role: {roleName} - {string.Join(',', result.Errors)}");
                }
                else
                {
                    Console.WriteLine($"ℹ️ Role already exists: {roleName}");
                }
            }
        }

        public static async Task EnsureAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@local.test";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var create = await userManager.CreateAsync(adminUser, "Admin@1234");
                if (create.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("✅ Default admin user created and assigned Admin role.");
                }
                else
                {
                    Console.WriteLine("❌ Failed to create admin user: " + string.Join(", ", create.Errors));
                }
            }
            else
            {
                // ensure admin has role
                var roles = await userManager.GetRolesAsync(adminUser);
                if (!roles.Contains("Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("✅ Existing user assigned Admin role.");
                }
                else
                {
                    Console.WriteLine("ℹ️ Admin user already present with Admin role.");
                }
            }
        }
    }
}
