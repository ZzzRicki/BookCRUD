using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCRUD.Migrations
{
    public partial class AddBookContentFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Format",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "Books");
        }
    }
}