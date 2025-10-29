using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Networth.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    InstitutionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    AccountType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Requisitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Redirect = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InstitutionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AgreementId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Accounts = table.Column<string[]>(type: "text[]", nullable: false),
                    AuthenticationLink = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AccountSelection = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requisitions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_Status",
                table: "Requisitions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Requisitions");
        }
    }
}
