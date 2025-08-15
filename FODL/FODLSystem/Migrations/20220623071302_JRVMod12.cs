using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class JRVMod12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastDownloaded",
                table: "SynchronizeInformations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDownloaded",
                table: "SynchronizeInformations");
        }
    }
}
