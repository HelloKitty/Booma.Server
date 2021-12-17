using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.CharacterDataService.Migrations
{
    public partial class UpdateToLatest1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_item_default",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ItemTemplateId = table.Column<int>(nullable: false),
                    RaceId = table.Column<int>(nullable: false),
                    ClassId = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_item_default", x => x.Id);
                    table.ForeignKey(
                        name: "FK_character_item_default_class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_item_default_item_template_ItemTemplateId",
                        column: x => x.ItemTemplateId,
                        principalTable: "item_template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_item_default_race_RaceId",
                        column: x => x.RaceId,
                        principalTable: "race",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "map",
                columns: table => new
                {
                    MapId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContentPath = table.Column<string>(nullable: true),
                    EntryPoint_X = table.Column<float>(nullable: true),
                    EntryPoint_Y = table.Column<float>(nullable: true),
                    EntryPoint_Z = table.Column<float>(nullable: true),
                    IsPersistent = table.Column<bool>(nullable: false),
                    VisualName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_map", x => x.MapId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_character_item_default_ClassId",
                table: "character_item_default",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_character_item_default_ItemTemplateId",
                table: "character_item_default",
                column: "ItemTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_character_item_default_RaceId",
                table: "character_item_default",
                column: "RaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_item_default");

            migrationBuilder.DropTable(
                name: "map");
        }
    }
}
