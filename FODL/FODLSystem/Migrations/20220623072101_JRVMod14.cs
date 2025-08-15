using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class JRVMod14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "Departments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "Departments");
        }
    }
}
