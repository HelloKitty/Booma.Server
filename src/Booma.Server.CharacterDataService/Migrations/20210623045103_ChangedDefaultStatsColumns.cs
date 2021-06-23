using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.CharacterDataService.Migrations
{
    public partial class ChangedDefaultStatsColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.CreateTable(
                name: "RPGStatValue<CharacterStatType>",
                columns: table => new
                {
                    StatType = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    Race = table.Column<int>(nullable: false),
                    ClassId = table.Column<byte>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RPGStatValue<CharacterStatType>", x => new { x.Level, x.Race, x.ClassId, x.StatType });
                    table.ForeignKey(
                        name: "FK_RPGStatValue<CharacterStatType>_stat_StatType",
                        column: x => x.StatType,
                        principalTable: "stat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RPGStatValue<CharacterStatType>_character_stat_default_Level~",
                        columns: x => new { x.Level, x.Race, x.ClassId },
                        principalTable: "character_stat_default",
                        principalColumns: new[] { "Level", "RaceId", "ClassId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RPGStatValue<CharacterStatType>_StatType",
                table: "RPGStatValue<CharacterStatType>",
                column: "StatType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RPGStatValue<CharacterStatType>");

            migrationBuilder.CreateTable(
                name: "RPGStatDefinition<CharacterStatType>",
                columns: table => new
                {
                    Level = table.Column<int>(type: "int", nullable: false),
                    Race = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Stat = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RPGStatDefinition<CharacterStatType>", x => new { x.Level, x.Race, x.ClassId, x.Stat });
                    table.ForeignKey(
                        name: "FK_RPGStatDefinition<CharacterStatType>_stat_Stat",
                        column: x => x.Stat,
                        principalTable: "stat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RPGStatDefinition<CharacterStatType>_character_stat_default_~",
                        columns: x => new { x.Level, x.Race, x.ClassId },
                        principalTable: "character_stat_default",
                        principalColumns: new[] { "Level", "RaceId", "ClassId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RPGStatDefinition<CharacterStatType>_Stat",
                table: "RPGStatDefinition<CharacterStatType>",
                column: "Stat");
        }
    }
}
