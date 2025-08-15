using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class bsmartupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BSmartEquipment_BSmartShifts_BSmartShiftId",
                table: "BSmartEquipment");

            migrationBuilder.DropForeignKey(
                name: "FK_BSmartItems_BSmartEquipment_BSmartEquipmentId",
                table: "BSmartItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BSmartEquipment",
                table: "BSmartEquipment");

            migrationBuilder.RenameTable(
                name: "BSmartEquipment",
                newName: "BSmartEquipments");

            migrationBuilder.RenameIndex(
                name: "IX_BSmartEquipment_BSmartShiftId",
                table: "BSmartEquipments",
                newName: "IX_BSmartEquipments_BSmartShiftId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BSmartEquipments",
                table: "BSmartEquipments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BSmartEquipments_BSmartShifts_BSmartShiftId",
                table: "BSmartEquipments",
                column: "BSmartShiftId",
                principalTable: "BSmartShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BSmartItems_BSmartEquipments_BSmartEquipmentId",
                table: "BSmartItems",
                column: "BSmartEquipmentId",
                principalTable: "BSmartEquipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BSmartEquipments_BSmartShifts_BSmartShiftId",
                table: "BSmartEquipments");

            migrationBuilder.DropForeignKey(
                name: "FK_BSmartItems_BSmartEquipments_BSmartEquipmentId",
                table: "BSmartItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BSmartEquipments",
                table: "BSmartEquipments");

            migrationBuilder.RenameTable(
                name: "BSmartEquipments",
                newName: "BSmartEquipment");

            migrationBuilder.RenameIndex(
                name: "IX_BSmartEquipments_BSmartShiftId",
                table: "BSmartEquipment",
                newName: "IX_BSmartEquipment_BSmartShiftId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BSmartEquipment",
                table: "BSmartEquipment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BSmartEquipment_BSmartShifts_BSmartShiftId",
                table: "BSmartEquipment",
                column: "BSmartShiftId",
                principalTable: "BSmartShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BSmartItems_BSmartEquipment_BSmartEquipmentId",
                table: "BSmartItems",
                column: "BSmartEquipmentId",
                principalTable: "BSmartEquipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
