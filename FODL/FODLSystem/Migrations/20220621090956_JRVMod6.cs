using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class JRVMod6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FuelOils_LubeTruckCode",
                table: "FuelOils",
                column: "LubeTruckCode");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOils_LubeTrucks_LubeTruckCode",
                table: "FuelOils",
                column: "LubeTruckCode",
                principalTable: "LubeTrucks",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelOils_LubeTrucks_LubeTruckCode",
                table: "FuelOils");

            migrationBuilder.DropIndex(
                name: "IX_FuelOils_LubeTruckCode",
                table: "FuelOils");
        }
    }
}
