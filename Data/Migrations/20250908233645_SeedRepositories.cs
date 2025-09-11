using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRepositories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Repositories",
                columns: new[] { "Id", "CreatedAt", "Description", "GitHubLink", "Title", "Type" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 8, 23, 36, 44, 755, DateTimeKind.Utc).AddTicks(282), "Open source AI repository", "https://github.com/example/free-ai", "AI Free Repo", "Free" },
                    { 2, new DateTime(2025, 9, 8, 23, 36, 44, 755, DateTimeKind.Utc).AddTicks(2074), "Premium ML project", "https://github.com/example/premium-ml", "ML Premium Repo", "Premium" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Repositories",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
