using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FODLSystem.Migrations
{
    public partial class BSmartData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BSmartShifts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReferenceNo = table.Column<string>(nullable: true),
                    Shift = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DispenserCode = table.Column<string>(nullable: true),
                    LubeTruckCode = table.Column<string>(nullable: true),
                    TransactionDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    TransferDate = table.Column<DateTime>(nullable: false),
                    TransferredBy = table.Column<string>(nullable: true),
                    SourceReferenceNo = table.Column<string>(nullable: true),
                    OriginalDate = table.Column<DateTime>(nullable: false),
                    BatchName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BSmartShifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BSmartEquipment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EquipmentNo = table.Column<string>(nullable: true),
                    LocationNo = table.Column<string>(nullable: true),
                    SMR = table.Column<decimal>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    FuelOilId = table.Column<int>(nullable: false),
                    BSmartShiftId = table.Column<int>(nullable: false),
                    OldId = table.Column<int>(nullable: false),
                    DriverIdNumber = table.Column<string>(nullable: true),
                    DetailNo = table.Column<string>(nullable: true),
                    OldDetailNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BSmartEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BSmartEquipment_BSmartShifts_BSmartShiftId",
                        column: x => x.BSmartShiftId,
                        principalTable: "BSmartShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BSmartItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeInput = table.Column<DateTime>(nullable: false),
                    ItemNo = table.Column<string>(nullable: true),
                    ComponentCode = table.Column<string>(nullable: true),
                    VolumeQty = table.Column<int>(nullable: false),
                    BSmartEquipmentId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    OldId = table.Column<int>(nullable: false),
                    OldFuelOilDetailNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BSmartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BSmartItems_BSmartEquipment_BSmartEquipmentId",
                        column: x => x.BSmartEquipmentId,
                        principalTable: "BSmartEquipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BSmartEquipment_BSmartShiftId",
                table: "BSmartEquipment",
                column: "BSmartShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_BSmartItems_BSmartEquipmentId",
                table: "BSmartItems",
                column: "BSmartEquipmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BSmartItems");

            migrationBuilder.DropTable(
                name: "BSmartEquipment");

            migrationBuilder.DropTable(
                name: "BSmartShifts");
        }
    }
}
