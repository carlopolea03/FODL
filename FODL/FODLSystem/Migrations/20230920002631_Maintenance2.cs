using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class Maintenance2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Locationcode",
                table: "LubeTrucks",
                type: "VARCHAR(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Locationcode",
                table: "Dispensers",
                type: "VARCHAR(20)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locationcode",
                table: "LubeTrucks");

            migrationBuilder.DropColumn(
                name: "Locationcode",
                table: "Dispensers");
        }
    }
}
