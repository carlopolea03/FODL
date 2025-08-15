using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class JRVMod3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComponentCode",
                table: "FuelOilSubDetails",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateModified",
                value: new DateTime(2022, 6, 21, 15, 4, 12, 882, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComponentCode",
                table: "FuelOilSubDetails");

            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateModified",
                value: new DateTime(2022, 6, 21, 14, 58, 11, 605, DateTimeKind.Local));
        }
    }
}
