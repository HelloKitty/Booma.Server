using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.GameConfigurationService.Migrations
{
    public partial class GeneralizedConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "config_keybinding");

            migrationBuilder.CreateTable(
                name: "gameconfig_type",
                columns: table => new
                {
                    Type = table.Column<int>(nullable: false),
                    VisualName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameconfig_type", x => x.Type);
                });

            migrationBuilder.CreateTable(
                name: "gameconfig_account",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    type = table.Column<int>(nullable: false),
                    data = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameconfig_account", x => new { x.id, x.type });
                    table.ForeignKey(
                        name: "FK_gameconfig_account_gameconfig_type_type",
                        column: x => x.type,
                        principalTable: "gameconfig_type",
                        principalColumn: "Type",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gameconfig_character",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    type = table.Column<int>(nullable: false),
                    data = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameconfig_character", x => new { x.id, x.type });
                    table.ForeignKey(
                        name: "FK_gameconfig_character_gameconfig_type_type",
                        column: x => x.type,
                        principalTable: "gameconfig_type",
                        principalColumn: "Type",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "gameconfig_type",
                columns: new[] { "Type", "Description", "VisualName" },
                values: new object[] { 1, "", "Key" });

            migrationBuilder.InsertData(
                table: "gameconfig_type",
                columns: new[] { "Type", "Description", "VisualName" },
                values: new object[] { 2, "", "Joystick" });

            migrationBuilder.CreateIndex(
                name: "IX_gameconfig_account_type",
                table: "gameconfig_account",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_gameconfig_character_type",
                table: "gameconfig_character",
                column: "type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gameconfig_account");

            migrationBuilder.DropTable(
                name: "gameconfig_character");

            migrationBuilder.DropTable(
                name: "gameconfig_type");

            migrationBuilder.CreateTable(
                name: "config_keybinding",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    data = table.Column<byte[]>(type: "longblob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_config_keybinding", x => x.id);
                });
        }
    }
}
