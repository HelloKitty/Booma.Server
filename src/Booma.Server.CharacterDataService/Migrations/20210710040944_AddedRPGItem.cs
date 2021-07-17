using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.CharacterDataService.Migrations
{
    public partial class AddedRPGItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "item_class",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    VisualName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_class", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "quality",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    VisualName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Color_X = table.Column<byte>(nullable: true),
                    Color_Y = table.Column<byte>(nullable: true),
                    Color_Z = table.Column<byte>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quality", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "item_sub_class",
                columns: table => new
                {
                    SubClassId = table.Column<int>(nullable: false),
                    ItemClassId = table.Column<byte>(nullable: false),
                    VisualName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_sub_class", x => new { x.ItemClassId, x.SubClassId });
                    table.ForeignKey(
                        name: "FK_item_sub_class_item_class_ItemClassId",
                        column: x => x.ItemClassId,
                        principalTable: "item_class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_template",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClassId = table.Column<byte>(nullable: false),
                    SubClassId = table.Column<int>(nullable: false),
                    VisualName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    QualityType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_template", x => x.Id);
                    table.ForeignKey(
                        name: "FK_item_template_quality_QualityType",
                        column: x => x.QualityType,
                        principalTable: "quality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_template_item_sub_class_ClassId_SubClassId",
                        columns: x => new { x.ClassId, x.SubClassId },
                        principalTable: "item_sub_class",
                        principalColumns: new[] { "ItemClassId", "SubClassId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "item_class",
                columns: new[] { "Id", "Description", "VisualName" },
                values: new object[,]
                {
                    { (byte)0, "", "Weapon" },
                    { (byte)1, "", "Guard" },
                    { (byte)2, "", "Mag" },
                    { (byte)3, "", "Tool" },
                    { (byte)4, "", "Meseta" }
                });

            migrationBuilder.InsertData(
                table: "quality",
                columns: new[] { "Id", "Description", "VisualName" },
                values: new object[,]
                {
                    { 1, "", "Common" },
                    { 2, "", "Rare" },
                    { 3, "", "Epic" },
                    { 4, "", "Legendary" },
                    { 5, "", "SRank" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_item_template_QualityType",
                table: "item_template",
                column: "QualityType");

            migrationBuilder.CreateIndex(
                name: "IX_item_template_ClassId_SubClassId",
                table: "item_template",
                columns: new[] { "ClassId", "SubClassId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_template");

            migrationBuilder.DropTable(
                name: "quality");

            migrationBuilder.DropTable(
                name: "item_sub_class");

            migrationBuilder.DropTable(
                name: "item_class");
        }
    }
}
