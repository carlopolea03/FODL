using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class ModFeb2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ComponentCode",
                table: "BSmartItems",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BSmartItems_ComponentCode",
                table: "BSmartItems",
                column: "ComponentCode");

            migrationBuilder.AddForeignKey(
                name: "FK_BSmartItems_Components_ComponentCode",
                table: "BSmartItems",
                column: "ComponentCode",
                principalTable: "Components",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BSmartItems_Components_ComponentCode",
                table: "BSmartItems");

            migrationBuilder.DropIndex(
                name: "IX_BSmartItems_ComponentCode",
                table: "BSmartItems");

            migrationBuilder.AlterColumn<string>(
                name: "ComponentCode",
                table: "BSmartItems",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
