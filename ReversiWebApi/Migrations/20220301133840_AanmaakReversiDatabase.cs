using Microsoft.EntityFrameworkCore.Migrations;

namespace ReversiWebApi.Migrations
{
    public partial class AanmaakReversiDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Spellen",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StringBord = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Omschrijving = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Speler1Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Speler2Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AandeBeurt = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spellen", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Spellen");
        }
    }
}
