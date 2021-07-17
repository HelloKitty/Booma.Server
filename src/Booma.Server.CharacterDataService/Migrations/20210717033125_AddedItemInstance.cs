using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.CharacterDataService.Migrations
{
    public partial class AddedItemInstance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "item_instance",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TemplateId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_instance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_item_instance_item_template_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "item_template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_instance_ownership",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    OwnershipType = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_instance_ownership", x => new { x.Id, x.OwnershipType });
                    table.ForeignKey(
                        name: "FK_item_instance_ownership_item_instance_Id",
                        column: x => x.Id,
                        principalTable: "item_instance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_item_inventory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<int>(nullable: false),
                    OwnershipId = table.Column<int>(nullable: false),
                    OwnershipType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_item_inventory", x => x.Id);
                    table.CheckConstraint("OwnershipType", "OwnershipType = 1");
                    table.ForeignKey(
                        name: "FK_character_item_inventory_character_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_item_inventory_item_instance_ownership_OwnershipId~",
                        columns: x => new { x.OwnershipId, x.OwnershipType },
                        principalTable: "item_instance_ownership",
                        principalColumns: new[] { "Id", "OwnershipType" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_character_item_inventory_CharacterId",
                table: "character_item_inventory",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_character_item_inventory_OwnershipId",
                table: "character_item_inventory",
                column: "OwnershipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_character_item_inventory_OwnershipId_OwnershipType",
                table: "character_item_inventory",
                columns: new[] { "OwnershipId", "OwnershipType" });

            migrationBuilder.CreateIndex(
                name: "IX_item_instance_TemplateId",
                table: "item_instance",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_item_instance_ownership_Id",
                table: "item_instance_ownership",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_item_inventory");

            migrationBuilder.DropTable(
                name: "item_instance_ownership");

            migrationBuilder.DropTable(
                name: "item_instance");
        }
    }
}
