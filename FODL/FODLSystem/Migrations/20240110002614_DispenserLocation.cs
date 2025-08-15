using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class DispenserLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispensers_Locations_LocationCode",
                table: "Dispensers");

            migrationBuilder.DropIndex(
                name: "IX_Dispensers_LocationCode",
                table: "Dispensers");
        }
    }
}
