using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class JRVMod2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelOilSubDetails_Items_ItemId",
                table: "FuelOilSubDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_FuelOilSubDetails_ItemId",
                table: "FuelOilSubDetails");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Components");

            migrationBuilder.AddColumn<string>(
                name: "ItemNo",
                table: "FuelOilSubDetails",
                type: "VARCHAR(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Components",
                type: "VARCHAR(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Components",
                type: "VARCHAR(20)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "No");

            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateModified",
                value: new DateTime(2022, 6, 21, 14, 58, 11, 605, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_FuelOilSubDetails_ItemNo",
                table: "FuelOilSubDetails",
                column: "ItemNo");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOilSubDetails_Items_ItemNo",
                table: "FuelOilSubDetails",
                column: "ItemNo",
                principalTable: "Items",
                principalColumn: "No",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelOilSubDetails_Items_ItemNo",
                table: "FuelOilSubDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_FuelOilSubDetails_ItemNo",
                table: "FuelOilSubDetails");

            migrationBuilder.DropColumn(
                name: "ItemNo",
                table: "FuelOilSubDetails");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Components");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Components",
                type: "VARCHAR(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Components",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateModified", "Name" },
                values: new object[] { new DateTime(2022, 6, 21, 13, 37, 57, 85, DateTimeKind.Local), "N/A" });

            migrationBuilder.CreateIndex(
                name: "IX_FuelOilSubDetails_ItemId",
                table: "FuelOilSubDetails",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelOilSubDetails_Items_ItemId",
                table: "FuelOilSubDetails",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
