using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Local = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeZone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoginStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceTypeId = table.Column<int>(type: "integer", nullable: false),
                    StatusTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginStatistics_UserAction_Id",
                        column: x => x.Id,
                        principalTable: "UserAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogoutStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceTypeId = table.Column<int>(type: "integer", nullable: false),
                    StatusTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogoutStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogoutStatistics_UserAction_Id",
                        column: x => x.Id,
                        principalTable: "UserAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceTypeId = table.Column<int>(type: "integer", nullable: false),
                    StatusTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationStatistics_UserAction_Id",
                        column: x => x.Id,
                        principalTable: "UserAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAction_UserId",
                table: "UserAction",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAction_UserId_UTC",
                table: "UserAction",
                columns: new[] { "UserId", "UTC" });

            migrationBuilder.CreateIndex(
                name: "IX_UserAction_UTC",
                table: "UserAction",
                column: "UTC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginStatistics");

            migrationBuilder.DropTable(
                name: "LogoutStatistics");

            migrationBuilder.DropTable(
                name: "RegistrationStatistics");

            migrationBuilder.DropTable(
                name: "UserAction");
        }
    }
}
