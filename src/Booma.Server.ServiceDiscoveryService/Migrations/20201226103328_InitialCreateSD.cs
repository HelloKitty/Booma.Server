using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.ServiceDiscoveryService.Migrations
{
    public partial class InitialCreateSD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ServiceName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services", x => x.Id);
                    table.UniqueConstraint("AK_services_ServiceName", x => x.ServiceName);
                });

            migrationBuilder.CreateTable(
                name: "service_endpoints",
                columns: table => new
                {
                    ServiceId = table.Column<int>(nullable: false),
                    Endpoint_Address = table.Column<string>(nullable: true),
                    Endpoint_Port = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_endpoints", x => x.ServiceId);
                    table.ForeignKey(
                        name: "FK_service_endpoints_services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "service_endpoints");

            migrationBuilder.DropTable(
                name: "services");
        }
    }
}
