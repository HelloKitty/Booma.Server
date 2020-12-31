using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Booma.Server.ServiceDiscoveryService.Migrations
{
    public partial class UpdateServiceDiscovery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ServiceType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services", x => x.Id);
                    table.UniqueConstraint("AK_services_ServiceType", x => x.ServiceType);
                });

            migrationBuilder.CreateTable(
                name: "service_endpoints",
                columns: table => new
                {
                    ServiceId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Endpoint_Address = table.Column<string>(nullable: true),
                    Endpoint_Port = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_endpoints", x => new { x.ServiceId, x.Name });
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
