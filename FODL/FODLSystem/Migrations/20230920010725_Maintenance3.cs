using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class Maintenance3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Locationcode",
                table: "LubeTrucks",
                newName: "LocationCode");

            migrationBuilder.RenameColumn(
                name: "Locationcode",
                table: "Dispensers",
                newName: "LocationCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocationCode",
                table: "LubeTrucks",
                newName: "Locationcode");

            migrationBuilder.RenameColumn(
                name: "LocationCode",
                table: "Dispensers",
                newName: "Locationcode");
        }
    }
}
