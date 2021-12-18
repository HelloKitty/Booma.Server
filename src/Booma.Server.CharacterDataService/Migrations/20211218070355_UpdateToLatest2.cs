using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.CharacterDataService.Migrations
{
    public partial class UpdateToLatest2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RPGStatValue<CharacterStatType>_stat_StatType",
                table: "RPGStatValue<CharacterStatType>");

            migrationBuilder.DropForeignKey(
                name: "FK_RPGStatValue<CharacterStatType>_character_stat_default_Level~",
                table: "RPGStatValue<CharacterStatType>");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RPGStatValue<CharacterStatType>",
                table: "RPGStatValue<CharacterStatType>");

            migrationBuilder.RenameTable(
                name: "RPGStatValue<CharacterStatType>",
                newName: "character_stat_default_rpgstats");

            migrationBuilder.RenameIndex(
                name: "IX_RPGStatValue<CharacterStatType>_StatType",
                table: "character_stat_default_rpgstats",
                newName: "IX_character_stat_default_rpgstats_StatType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_character_stat_default_rpgstats",
                table: "character_stat_default_rpgstats",
                columns: new[] { "Level", "Race", "ClassId", "StatType" });

            migrationBuilder.AddForeignKey(
                name: "FK_character_stat_default_rpgstats_stat_StatType",
                table: "character_stat_default_rpgstats",
                column: "StatType",
                principalTable: "stat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_stat_default_rpgstats_character_stat_default_Level~",
                table: "character_stat_default_rpgstats",
                columns: new[] { "Level", "Race", "ClassId" },
                principalTable: "character_stat_default",
                principalColumns: new[] { "Level", "RaceId", "ClassId" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_character_stat_default_rpgstats_stat_StatType",
                table: "character_stat_default_rpgstats");

            migrationBuilder.DropForeignKey(
                name: "FK_character_stat_default_rpgstats_character_stat_default_Level~",
                table: "character_stat_default_rpgstats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_character_stat_default_rpgstats",
                table: "character_stat_default_rpgstats");

            migrationBuilder.RenameTable(
                name: "character_stat_default_rpgstats",
                newName: "RPGStatValue<CharacterStatType>");

            migrationBuilder.RenameIndex(
                name: "IX_character_stat_default_rpgstats_StatType",
                table: "RPGStatValue<CharacterStatType>",
                newName: "IX_RPGStatValue<CharacterStatType>_StatType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RPGStatValue<CharacterStatType>",
                table: "RPGStatValue<CharacterStatType>",
                columns: new[] { "Level", "Race", "ClassId", "StatType" });

            migrationBuilder.AddForeignKey(
                name: "FK_RPGStatValue<CharacterStatType>_stat_StatType",
                table: "RPGStatValue<CharacterStatType>",
                column: "StatType",
                principalTable: "stat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RPGStatValue<CharacterStatType>_character_stat_default_Level~",
                table: "RPGStatValue<CharacterStatType>",
                columns: new[] { "Level", "Race", "ClassId" },
                principalTable: "character_stat_default",
                principalColumns: new[] { "Level", "RaceId", "ClassId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
