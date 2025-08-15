using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class JRVMod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelOilDetails_Drivers_DriverId",
                table: "FuelOilDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FuelOilDetails_Equipments_EquipmentId",
                table: "FuelOilDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FuelOilDetails_Locations_LocationId",
                table: "FuelOilDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locations",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_FuelOilDetails_DriverId",
                table: "FuelOilDetails");

            migrationBuilder.DropIndex(
                name: "IX_FuelOilDetails_EquipmentId",
                table: "FuelOilDetails");

            migrationBuilder.DropIndex(
                name: "IX_FuelOilDetails_LocationId",
                table: "FuelOilDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Drivers",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_IdNumber",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "FuelOilDetails");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "FuelOilDetails");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "FuelOilDetails");

            migrationBuilder.AddColumn<string>(
                name: "DriverIdNumber",
                table: "FuelOilDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentNo",
                table: "FuelOilDetails",
                type: "VARCHAR(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationNo",
                table: "FuelOilDetails",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdNumber",
                table: "Drivers",
                type: "VARCHAR(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locations",
                table: "Locations",
                column: "No");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments",
                column: "No");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Drivers",
                table: "Drivers",
                column: "IdNumber");

            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateModified",
                value: new DateTime(2022, 6, 21, 13, 37, 57, 85, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_FuelOilDetails_DriverIdNumber",
                table: "FuelOilDetails",
                column: "DriverIdNumber");

            migrationBuilder.CreateIndex(
                name: "IX_FuelOilDetails_EquipmentNo",
                table: "FuelOilDetails",
                column: "EquipmentNo");

            migrationBuilder.CreateIndex(
                name: "IX_FuelOilDetails_LocationNo",
                table: "FuelOilDetails",
                column: "LocationNo");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOilDetails_Drivers_DriverIdNumber",
                table: "FuelOilDetails",
                column: "DriverIdNumber",
                principalTable: "Drivers",
                principalColumn: "IdNumber",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOilDetails_Equipments_EquipmentNo",
                table: "FuelOilDetails",
                column: "EquipmentNo",
                principalTable: "Equipments",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOilDetails_Locations_LocationNo",
                table: "FuelOilDetails",
                column: "LocationNo",
                principalTable: "Locations",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelOilDetails_Drivers_DriverIdNumber",
                table: "FuelOilDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FuelOilDetails_Equipments_EquipmentNo",
                table: "FuelOilDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FuelOilDetails_Locations_LocationNo",
                table: "FuelOilDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locations",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_FuelOilDetails_DriverIdNumber",
                table: "FuelOilDetails");

            migrationBuilder.DropIndex(
                name: "IX_FuelOilDetails_EquipmentNo",
                table: "FuelOilDetails");

            migrationBuilder.DropIndex(
                name: "IX_FuelOilDetails_LocationNo",
                table: "FuelOilDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Drivers",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DriverIdNumber",
                table: "FuelOilDetails");

            migrationBuilder.DropColumn(
                name: "EquipmentNo",
                table: "FuelOilDetails");

            migrationBuilder.DropColumn(
                name: "LocationNo",
                table: "FuelOilDetails");

            migrationBuilder.AddColumn<int>(
                name: "DriverId",
                table: "FuelOilDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentId",
                table: "FuelOilDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "FuelOilDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "IdNumber",
                table: "Drivers",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locations",
                table: "Locations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Drivers",
                table: "Drivers",
                column: "ID");

            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateModified",
                value: new DateTime(2022, 1, 13, 13, 23, 52, 31, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_FuelOilDetails_DriverId",
                table: "FuelOilDetails",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelOilDetails_EquipmentId",
                table: "FuelOilDetails",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelOilDetails_LocationId",
                table: "FuelOilDetails",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_IdNumber",
                table: "Drivers",
                column: "IdNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOilDetails_Drivers_DriverId",
                table: "FuelOilDetails",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOilDetails_Equipments_EquipmentId",
                table: "FuelOilDetails",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOilDetails_Locations_LocationId",
                table: "FuelOilDetails",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
