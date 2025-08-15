using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class JRVMod5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelOils_Dispensers_DispenserId",
                table: "FuelOils");

            migrationBuilder.DropForeignKey(
                name: "FK_FuelOils_LubeTrucks_LubeTruckId",
                table: "FuelOils");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Departments_DepartmentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DepartmentId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LubeTrucks",
                table: "LubeTrucks");

            migrationBuilder.DropIndex(
                name: "IX_LubeTrucks_No_Status",
                table: "LubeTrucks");

            migrationBuilder.DropIndex(
                name: "IX_FuelOils_DispenserId",
                table: "FuelOils");

            migrationBuilder.DropIndex(
                name: "IX_FuelOils_LubeTruckId",
                table: "FuelOils");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dispensers",
                table: "Dispensers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                table: "Departments");

            migrationBuilder.DeleteData(
                table: "Dispensers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "IdNumber",
                keyValue: "00000");

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "No",
                keyValue: "na");

            migrationBuilder.DeleteData(
                table: "LubeTrucks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ComponentId",
                table: "FuelOilSubDetails");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "FuelOilSubDetails");

            migrationBuilder.DropColumn(
                name: "DispenserId",
                table: "FuelOils");

            migrationBuilder.DropColumn(
                name: "LubeTruckId",
                table: "FuelOils");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "Users",
                type: "VARCHAR(20)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "LubeTrucks",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DispenserCode",
                table: "FuelOils",
                type: "VARCHAR(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LubeTruckCode",
                table: "FuelOils",
                type: "VARCHAR(20)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Departments",
                type: "VARCHAR(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LubeTrucks",
                table: "LubeTrucks",
                column: "No");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dispensers",
                table: "Dispensers",
                column: "No");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                table: "Departments",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentCode",
                table: "Users",
                column: "DepartmentCode");

            migrationBuilder.CreateIndex(
                name: "IX_FuelOils_DispenserCode",
                table: "FuelOils",
                column: "DispenserCode");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOils_Dispensers_DispenserCode",
                table: "FuelOils",
                column: "DispenserCode",
                principalTable: "Dispensers",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Departments_DepartmentCode",
                table: "Users",
                column: "DepartmentCode",
                principalTable: "Departments",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelOils_Dispensers_DispenserCode",
                table: "FuelOils");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Departments_DepartmentCode",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DepartmentCode",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LubeTrucks",
                table: "LubeTrucks");

            migrationBuilder.DropIndex(
                name: "IX_FuelOils_DispenserCode",
                table: "FuelOils");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dispensers",
                table: "Dispensers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DispenserCode",
                table: "FuelOils");

            migrationBuilder.DropColumn(
                name: "LubeTruckCode",
                table: "FuelOils");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "LubeTrucks",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComponentId",
                table: "FuelOilSubDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "FuelOilSubDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DispenserId",
                table: "FuelOils",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LubeTruckId",
                table: "FuelOils",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Departments",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LubeTrucks",
                table: "LubeTrucks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dispensers",
                table: "Dispensers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                table: "Departments",
                column: "ID");

            migrationBuilder.InsertData(
                table: "Dispensers",
                columns: new[] { "Id", "DateModified", "Name", "No", "Status" },
                values: new object[] { 1, null, "N/A", "na", "Default" });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "IdNumber", "DateModified", "ID", "Name", "Position", "Status" },
                values: new object[] { "00000", new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "N/A", "N/A", "Enabled" });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "No", "Id", "List", "OfficeCode", "Status" },
                values: new object[] { "na", 1, "N/A", "000", "Default" });

            migrationBuilder.InsertData(
                table: "LubeTrucks",
                columns: new[] { "Id", "DateModified", "Description", "No", "OldId", "Status" },
                values: new object[] { 1, null, "N/A", "na", "0", "Default" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "DepartmentId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LubeTrucks_No_Status",
                table: "LubeTrucks",
                columns: new[] { "No", "Status" },
                unique: true,
                filter: "[Status] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FuelOils_DispenserId",
                table: "FuelOils",
                column: "DispenserId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelOils_LubeTruckId",
                table: "FuelOils",
                column: "LubeTruckId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOils_Dispensers_DispenserId",
                table: "FuelOils",
                column: "DispenserId",
                principalTable: "Dispensers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOils_LubeTrucks_LubeTruckId",
                table: "FuelOils",
                column: "LubeTruckId",
                principalTable: "LubeTrucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Departments_DepartmentId",
                table: "Users",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
