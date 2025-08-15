using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class Maintenance5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispensers_Locations_LocationCode",
                table: "Dispensers");

            migrationBuilder.DropForeignKey(
                name: "FK_LubeTrucks_Locations_LocationCode",
                table: "LubeTrucks");

            migrationBuilder.DropIndex(
                name: "IX_LubeTrucks_LocationCode",
                table: "LubeTrucks");

            migrationBuilder.DropIndex(
                name: "IX_Dispensers_LocationCode",
                table: "Dispensers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LubeTrucks_LocationCode",
                table: "LubeTrucks",
                column: "LocationCode");

            migrationBuilder.CreateIndex(
                name: "IX_Dispensers_LocationCode",
                table: "Dispensers",
                column: "LocationCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispensers_Locations_LocationCode",
                table: "Dispensers",
                column: "LocationCode",
                principalTable: "Locations",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LubeTrucks_Locations_LocationCode",
                table: "LubeTrucks",
                column: "LocationCode",
                principalTable: "Locations",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
