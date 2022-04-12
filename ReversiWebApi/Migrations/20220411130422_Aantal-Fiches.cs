using Microsoft.EntityFrameworkCore.Migrations;

namespace ReversiWebApi.Migrations
{
    public partial class AantalFiches : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AantalWit",
                table: "Spellen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AantalZwart",
                table: "Spellen",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AantalWit",
                table: "Spellen");

            migrationBuilder.DropColumn(
                name: "AantalZwart",
                table: "Spellen");
        }
    }
}
