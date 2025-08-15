using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class ModFeb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LubeTruckCode",
                table: "BSmartShifts",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DispenserCode",
                table: "BSmartShifts",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemNo",
                table: "BSmartItems",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LocationNo",
                table: "BSmartEquipments",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EquipmentNo",
                table: "BSmartEquipments",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DriverIdNumber",
                table: "BSmartEquipments",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BSmartShifts_DispenserCode",
                table: "BSmartShifts",
                column: "DispenserCode");

            migrationBuilder.CreateIndex(
                name: "IX_BSmartShifts_LubeTruckCode",
                table: "BSmartShifts",
                column: "LubeTruckCode");

            migrationBuilder.CreateIndex(
                name: "IX_BSmartItems_ItemNo",
                table: "BSmartItems",
                column: "ItemNo");

            migrationBuilder.CreateIndex(
                name: "IX_BSmartEquipments_DriverIdNumber",
                table: "BSmartEquipments",
                column: "DriverIdNumber");

            migrationBuilder.CreateIndex(
                name: "IX_BSmartEquipments_EquipmentNo",
                table: "BSmartEquipments",
                column: "EquipmentNo");

            migrationBuilder.CreateIndex(
                name: "IX_BSmartEquipments_LocationNo",
                table: "BSmartEquipments",
                column: "LocationNo");

            migrationBuilder.AddForeignKey(
                name: "FK_BSmartEquipments_Drivers_DriverIdNumber",
                table: "BSmartEquipments",
                column: "DriverIdNumber",
                principalTable: "Drivers",
                principalColumn: "IdNumber",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BSmartEquipments_Equipments_EquipmentNo",
                table: "BSmartEquipments",
                column: "EquipmentNo",
                principalTable: "Equipments",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BSmartEquipments_Locations_LocationNo",
                table: "BSmartEquipments",
                column: "LocationNo",
                principalTable: "Locations",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BSmartItems_Items_ItemNo",
                table: "BSmartItems",
                column: "ItemNo",
                principalTable: "Items",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BSmartShifts_Dispensers_DispenserCode",
                table: "BSmartShifts",
                column: "DispenserCode",
                principalTable: "Dispensers",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BSmartShifts_LubeTrucks_LubeTruckCode",
                table: "BSmartShifts",
                column: "LubeTruckCode",
                principalTable: "LubeTrucks",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BSmartEquipments_Drivers_DriverIdNumber",
                table: "BSmartEquipments");

            migrationBuilder.DropForeignKey(
                name: "FK_BSmartEquipments_Equipments_EquipmentNo",
                table: "BSmartEquipments");

            migrationBuilder.DropForeignKey(
                name: "FK_BSmartEquipments_Locations_LocationNo",
                table: "BSmartEquipments");

            migrationBuilder.DropForeignKey(
                name: "FK_BSmartItems_Items_ItemNo",
                table: "BSmartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BSmartShifts_Dispensers_DispenserCode",
                table: "BSmartShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_BSmartShifts_LubeTrucks_LubeTruckCode",
                table: "BSmartShifts");

            migrationBuilder.DropIndex(
                name: "IX_BSmartShifts_DispenserCode",
                table: "BSmartShifts");

            migrationBuilder.DropIndex(
                name: "IX_BSmartShifts_LubeTruckCode",
                table: "BSmartShifts");

            migrationBuilder.DropIndex(
                name: "IX_BSmartItems_ItemNo",
                table: "BSmartItems");

            migrationBuilder.DropIndex(
                name: "IX_BSmartEquipments_DriverIdNumber",
                table: "BSmartEquipments");

            migrationBuilder.DropIndex(
                name: "IX_BSmartEquipments_EquipmentNo",
                table: "BSmartEquipments");

            migrationBuilder.DropIndex(
                name: "IX_BSmartEquipments_LocationNo",
                table: "BSmartEquipments");

            migrationBuilder.AlterColumn<string>(
                name: "LubeTruckCode",
                table: "BSmartShifts",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DispenserCode",
                table: "BSmartShifts",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemNo",
                table: "BSmartItems",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LocationNo",
                table: "BSmartEquipments",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EquipmentNo",
                table: "BSmartEquipments",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DriverIdNumber",
                table: "BSmartEquipments",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
