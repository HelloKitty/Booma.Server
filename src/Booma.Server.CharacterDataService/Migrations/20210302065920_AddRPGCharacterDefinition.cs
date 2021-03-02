using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.CharacterDataService.Migrations
{
    public partial class AddRPGCharacterDefinition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_character_class_Class",
                table: "character");

            migrationBuilder.DropForeignKey(
                name: "FK_character_race_Race",
                table: "character");

            migrationBuilder.DropIndex(
                name: "IX_character_Class",
                table: "character");

            migrationBuilder.DropIndex(
                name: "IX_character_Race",
                table: "character");

            migrationBuilder.DropColumn(
                name: "Class",
                table: "character");

            migrationBuilder.DropColumn(
                name: "Race",
                table: "character");

            migrationBuilder.CreateTable(
                name: "character_definition",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Race = table.Column<int>(nullable: false),
                    Class = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_definition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_character_definition_class_Class",
                        column: x => x.Class,
                        principalTable: "class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_definition_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_definition_race_Race",
                        column: x => x.Race,
                        principalTable: "race",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_character_definition_Class",
                table: "character_definition",
                column: "Class");

            migrationBuilder.CreateIndex(
                name: "IX_character_definition_Race",
                table: "character_definition",
                column: "Race");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_definition");

            migrationBuilder.AddColumn<byte>(
                name: "Class",
                table: "character",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "Race",
                table: "character",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_character_Class",
                table: "character",
                column: "Class");

            migrationBuilder.CreateIndex(
                name: "IX_character_Race",
                table: "character",
                column: "Race");

            migrationBuilder.AddForeignKey(
                name: "FK_character_class_Class",
                table: "character",
                column: "Class",
                principalTable: "class",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_race_Race",
                table: "character",
                column: "Race",
                principalTable: "race",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
