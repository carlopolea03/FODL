using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class JRVMod4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelOilSubDetails_Components_ComponentId",
                table: "FuelOilSubDetails");

            migrationBuilder.DropIndex(
                name: "IX_FuelOilSubDetails_ComponentId",
                table: "FuelOilSubDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Components",
                table: "Components");

            migrationBuilder.DeleteData(
                table: "Components",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "ComponentCode",
                table: "FuelOilSubDetails",
                type: "VARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Components",
                type: "VARCHAR(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Components",
                table: "Components",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_FuelOilSubDetails_ComponentCode",
                table: "FuelOilSubDetails",
                column: "ComponentCode");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOilSubDetails_Components_ComponentCode",
                table: "FuelOilSubDetails",
                column: "ComponentCode",
                principalTable: "Components",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelOilSubDetails_Components_ComponentCode",
                table: "FuelOilSubDetails");

            migrationBuilder.DropIndex(
                name: "IX_FuelOilSubDetails_ComponentCode",
                table: "FuelOilSubDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Components",
                table: "Components");

            migrationBuilder.AlterColumn<string>(
                name: "ComponentCode",
                table: "FuelOilSubDetails",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Components",
                type: "VARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Components",
                table: "Components",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Components",
                columns: new[] { "Id", "Code", "DateModified", "Description", "Status" },
                values: new object[] { 1, null, new DateTime(2022, 6, 21, 15, 4, 12, 882, DateTimeKind.Local), null, "Default" });

            migrationBuilder.CreateIndex(
                name: "IX_FuelOilSubDetails_ComponentId",
                table: "FuelOilSubDetails",
                column: "ComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOilSubDetails_Components_ComponentId",
                table: "FuelOilSubDetails",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
