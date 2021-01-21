using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.CharacterDataService.Migrations
{
    public partial class NewInitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_customization_slot_type",
                columns: table => new
                {
                    SlotType = table.Column<int>(nullable: false),
                    VisualName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_customization_slot_type", x => x.SlotType);
                });

            migrationBuilder.CreateTable(
                name: "character_progress",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Experience = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    PlayTime = table.Column<TimeSpan>(nullable: false, defaultValue: new TimeSpan(0, 0, 0, 0, 0)),
                    LastModifiedDate = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_progress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "character_proportion_slot_type",
                columns: table => new
                {
                    SlotType = table.Column<int>(nullable: false),
                    VisualName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_proportion_slot_type", x => x.SlotType);
                });

            migrationBuilder.CreateTable(
                name: "character",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LastModifiedDate = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character", x => x.Id);
                    table.ForeignKey(
                        name: "FK_character_character_progress_Id",
                        column: x => x.Id,
                        principalTable: "character_progress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_customization_slot",
                columns: table => new
                {
                    CharacterId = table.Column<int>(nullable: false),
                    SlotType = table.Column<int>(nullable: false),
                    CustomizationId = table.Column<int>(nullable: false),
                    SlotColor_X = table.Column<ushort>(nullable: true),
                    SlotColor_Y = table.Column<ushort>(nullable: true),
                    SlotColor_Z = table.Column<ushort>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_customization_slot", x => new { x.CharacterId, x.SlotType });
                    table.ForeignKey(
                        name: "FK_character_customization_slot_character_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_customization_slot_character_customization_slot_ty~",
                        column: x => x.SlotType,
                        principalTable: "character_customization_slot_type",
                        principalColumn: "SlotType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_ownership",
                columns: table => new
                {
                    OwnershipId = table.Column<int>(nullable: false),
                    CharacterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_ownership", x => new { x.OwnershipId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_character_ownership_character_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_proportion_slot",
                columns: table => new
                {
                    CharacterId = table.Column<int>(nullable: false),
                    SlotType = table.Column<int>(nullable: false),
                    Proportion_X = table.Column<float>(nullable: true),
                    Proportion_Y = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_proportion_slot", x => new { x.CharacterId, x.SlotType });
                    table.ForeignKey(
                        name: "FK_character_proportion_slot_character_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_proportion_slot_character_proportion_slot_type_Slo~",
                        column: x => x.SlotType,
                        principalTable: "character_proportion_slot_type",
                        principalColumn: "SlotType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "character_customization_slot_type",
                columns: new[] { "SlotType", "Description", "VisualName" },
                values: new object[,]
                {
                    { 1, "", "Costume" },
                    { 2, "", "Skin" },
                    { 3, "", "Face" },
                    { 4, "", "Head" },
                    { 5, "", "Hair" },
                    { 6, "", "Override" }
                });

            migrationBuilder.InsertData(
                table: "character_proportion_slot_type",
                columns: new[] { "SlotType", "Description", "VisualName" },
                values: new object[] { 1, "", "Default" });

            migrationBuilder.CreateIndex(
                name: "IX_character_customization_slot_CharacterId",
                table: "character_customization_slot",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_character_customization_slot_SlotType",
                table: "character_customization_slot",
                column: "SlotType");

            migrationBuilder.CreateIndex(
                name: "IX_character_ownership_CharacterId",
                table: "character_ownership",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_character_ownership_OwnershipId",
                table: "character_ownership",
                column: "OwnershipId");

            migrationBuilder.CreateIndex(
                name: "IX_character_proportion_slot_CharacterId",
                table: "character_proportion_slot",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_character_proportion_slot_SlotType",
                table: "character_proportion_slot",
                column: "SlotType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_customization_slot");

            migrationBuilder.DropTable(
                name: "character_ownership");

            migrationBuilder.DropTable(
                name: "character_proportion_slot");

            migrationBuilder.DropTable(
                name: "character_customization_slot_type");

            migrationBuilder.DropTable(
                name: "character");

            migrationBuilder.DropTable(
                name: "character_proportion_slot_type");

            migrationBuilder.DropTable(
                name: "character_progress");
        }
    }
}
