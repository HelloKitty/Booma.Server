using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.CharacterDataService.Migrations
{
    public partial class FixedStatsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_RPGStatDefinition<CharacterStatType>_character_stat_default_~",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RPGStatDefinition<CharacterStatType>",
                table: "RPGStatDefinition<CharacterStatType>");*/

            migrationBuilder.DropIndex(
                name: "IX_RPGStatDefinition<CharacterStatType>_DBRPGCharacterStatDefau~",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropColumn(
                name: "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Cha~",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropColumn(
                name: "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Ch~1",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropColumn(
                name: "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Ch~2",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "RPGStatDefinition<CharacterStatType>",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Race",
                table: "RPGStatDefinition<CharacterStatType>",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "ClassId",
                table: "RPGStatDefinition<CharacterStatType>",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "Stat",
                table: "RPGStatDefinition<CharacterStatType>",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RPGStatDefinition<CharacterStatType>",
                table: "RPGStatDefinition<CharacterStatType>",
                columns: new[] { "Level", "Race", "ClassId", "Stat" });

            migrationBuilder.CreateIndex(
                name: "IX_RPGStatDefinition<CharacterStatType>_Stat",
                table: "RPGStatDefinition<CharacterStatType>",
                column: "Stat");

            migrationBuilder.AddForeignKey(
                name: "FK_RPGStatDefinition<CharacterStatType>_stat_Stat",
                table: "RPGStatDefinition<CharacterStatType>",
                column: "Stat",
                principalTable: "stat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RPGStatDefinition<CharacterStatType>_character_stat_default_~",
                table: "RPGStatDefinition<CharacterStatType>",
                columns: new[] { "Level", "Race", "ClassId" },
                principalTable: "character_stat_default",
                principalColumns: new[] { "Level", "RaceId", "ClassId" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_RPGStatDefinition<CharacterStatType>_stat_Stat",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropForeignKey(
                name: "FK_RPGStatDefinition<CharacterStatType>_character_stat_default_~",
                table: "RPGStatDefinition<CharacterStatType>");*/

            migrationBuilder.DropPrimaryKey(
                name: "PK_RPGStatDefinition<CharacterStatType>",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropIndex(
                name: "IX_RPGStatDefinition<CharacterStatType>_Stat",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropColumn(
                name: "Race",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.DropColumn(
                name: "Stat",
                table: "RPGStatDefinition<CharacterStatType>");

            migrationBuilder.AddColumn<int>(
                name: "Key",
                table: "RPGStatDefinition<CharacterStatType>",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<byte>(
                name: "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Cha~",
                table: "RPGStatDefinition<CharacterStatType>",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Ch~1",
                table: "RPGStatDefinition<CharacterStatType>",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Ch~2",
                table: "RPGStatDefinition<CharacterStatType>",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RPGStatDefinition<CharacterStatType>",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RPGStatDefinition<CharacterStatType>",
                table: "RPGStatDefinition<CharacterStatType>",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_RPGStatDefinition<CharacterStatType>_DBRPGCharacterStatDefau~",
                table: "RPGStatDefinition<CharacterStatType>",
                columns: new[] { "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Ch~1", "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Ch~2", "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Cha~" });

            migrationBuilder.AddForeignKey(
                name: "FK_RPGStatDefinition<CharacterStatType>_character_stat_default_~",
                table: "RPGStatDefinition<CharacterStatType>",
                columns: new[] { "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Ch~1", "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Ch~2", "DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, Cha~" },
                principalTable: "character_stat_default",
                principalColumns: new[] { "Level", "RaceId", "ClassId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
