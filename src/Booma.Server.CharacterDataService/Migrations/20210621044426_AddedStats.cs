using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.CharacterDataService.Migrations
{
    public partial class AddedStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_stat_default",
                columns: table => new
                {
                    Level = table.Column<int>(nullable: false),
                    RaceId = table.Column<int>(nullable: false),
                    ClassId = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_stat_default", x => new { x.Level, x.RaceId, x.ClassId });
                    table.ForeignKey(
                        name: "FK_character_stat_default_class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_stat_default_race_RaceId",
                        column: x => x.RaceId,
                        principalTable: "race",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    VisualName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RPGStatDefinition<CharacterStatType>",
                columns: table => new
                {
                    Key = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Id = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false),
                    DBRPGCharacterStatDefaultCharacterStatTypeCharacterRaceCha = table.Column<byte>(name: "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Cha~", nullable: false),
                    DBRPGCharacterStatDefaultCharacterStatTypeCharacterRaceCh1 = table.Column<int>(name: "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Ch~1", nullable: false),
                    DBRPGCharacterStatDefaultCharacterStatTypeCharacterRaceCh2 = table.Column<int>(name: "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Ch~2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RPGStatDefinition<CharacterStatType>", x => x.Key);
                    table.ForeignKey(
                        name: "FK_RPGStatDefinition<CharacterStatType>_character_stat_default_~",
                        columns: x => new { x.DBRPGCharacterStatDefaultCharacterStatTypeCharacterRaceCh1, x.DBRPGCharacterStatDefaultCharacterStatTypeCharacterRaceCh2, x.DBRPGCharacterStatDefaultCharacterStatTypeCharacterRaceCha },
                        principalTable: "character_stat_default",
                        principalColumns: new[] { "Level", "RaceId", "ClassId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "stat",
                columns: new[] { "Id", "Description", "VisualName" },
                values: new object[,]
                {
                    { 0, "", "ATP" },
                    { 1, "", "MST" },
                    { 2, "", "EVP" },
                    { 3, "", "HP" },
                    { 4, "", "DFP" },
                    { 5, "", "ATA" },
                    { 6, "", "LCK" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_character_stat_default_ClassId",
                table: "character_stat_default",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_character_stat_default_RaceId",
                table: "character_stat_default",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_RPGStatDefinition<CharacterStatType>_DBRPGCharacterStatDefau~",
                table: "RPGStatDefinition<CharacterStatType>",
                columns: new[] { "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Ch~1", "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Ch~2", "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Cha~" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropTable(
                name: "stat");

            migrationBuilder.DropTable(
                name: "character_stat_default");
        }
    }
}
