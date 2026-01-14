using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Networth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSandboxInstitutionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create SandboxInstitution table with same schema as Institutions
            // This table is used when UseSandbox=true in development mode
            migrationBuilder.CreateTable(
                name: "SandboxInstitution",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    country_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    bic = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    countries = table.Column<string>(type: "jsonb", nullable: false),
                    supported_features = table.Column<string>(type: "jsonb", nullable: true),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SandboxInstitution", x => new { x.id, x.country_code });
                });

            // Same indexes as Institutions table
            migrationBuilder.CreateIndex(
                name: "IX_SandboxInstitution_CountryCode",
                table: "SandboxInstitution",
                column: "country_code");

            migrationBuilder.CreateIndex(
                name: "IX_SandboxInstitution_LastUpdated",
                table: "SandboxInstitution",
                column: "last_updated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SandboxInstitution");
        }
    }
}
