using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class UpdateFODL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DispenserName",
                table: "FuelOils",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewName",
                table: "Dispensers",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DispenserName",
                table: "BSmartShifts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DispenserName",
                table: "FuelOils");

            migrationBuilder.DropColumn(
                name: "NewName",
                table: "Dispensers");

            migrationBuilder.DropColumn(
                name: "DispenserName",
                table: "BSmartShifts");
        }
    }
}
