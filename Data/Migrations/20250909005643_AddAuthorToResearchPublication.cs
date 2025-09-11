using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorToResearchPublication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Authors",
                table: "ResearchPublications",
                newName: "Author");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "ResearchPublications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "ResearchPublications");

            migrationBuilder.RenameColumn(
                name: "Author",
                table: "ResearchPublications",
                newName: "Authors");
        }
    }
}
