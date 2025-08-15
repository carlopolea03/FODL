using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class OldBSmartShift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalReferenceNo",
                table: "BSmartShifts",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "FirstName", "LastName", "Name", "Username" },
                values: new object[] { "jrventura@semirarampc.com", "jrventura", "jrventura", "jrventura", "jrventura" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalReferenceNo",
                table: "BSmartShifts");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "FirstName", "LastName", "Name", "Username" },
                values: new object[] { "kcmalapit@semirarampc.com", "Kristoffer", "Malapit", "Kristoffer Malapit", "kcmalapit" });
        }
    }
}
