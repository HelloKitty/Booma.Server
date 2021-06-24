using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.GameConfigurationService.Migrations
{
    public partial class AddedPalleteConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "gameconfig_type",
                columns: new[] { "Type", "Description", "VisualName" },
                values: new object[] { 3, "", "ActionBar" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "gameconfig_type",
                keyColumn: "Type",
                keyValue: 3);
        }
    }
}
