using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CultureWeb.Migrations
{
    /// <inheritdoc />
    public partial class addColorKh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductColor_kh",
                table: "Products",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductColor_kh",
                table: "Products");
        }
    }
}
