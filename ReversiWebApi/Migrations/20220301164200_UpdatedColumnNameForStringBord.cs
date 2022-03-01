using Microsoft.EntityFrameworkCore.Migrations;

namespace ReversiWebApi.Migrations
{
    public partial class UpdatedColumnNameForStringBord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StringBord",
                table: "Spellen",
                newName: "Bord");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Bord",
                table: "Spellen",
                newName: "StringBord");
        }
    }
}
