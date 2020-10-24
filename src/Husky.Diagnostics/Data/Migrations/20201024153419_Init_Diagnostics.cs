using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Husky.Diagnostics.Data.Migrations
{
    public partial class Init_Diagnostics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExceptionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnonymousId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    Time = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Repeated = table.Column<int>(nullable: false),
                    LastTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Md5Comparison = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    HttpMethod = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: true),
                    Url = table.Column<string>(maxLength: 1000, nullable: true),
                    Referrer = table.Column<string>(maxLength: 1000, nullable: true),
                    Data = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    UserIp = table.Column<string>(type: "varchar(39)", maxLength: 39, nullable: true),
                    IsAjax = table.Column<bool>(nullable: false),
                    ExceptionType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(maxLength: 1000, nullable: true),
                    Source = table.Column<string>(nullable: true),
                    StackTrace = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnonymousId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    Time = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Repeated = table.Column<int>(nullable: false),
                    LastTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Md5Comparison = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    LogLevel = table.Column<int>(nullable: false),
                    Message = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnonymousId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    Time = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Repeated = table.Column<int>(nullable: false),
                    LastTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Md5Comparison = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    HttpMethod = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: true),
                    Url = table.Column<string>(maxLength: 1000, nullable: true),
                    Referrer = table.Column<string>(maxLength: 1000, nullable: true),
                    Data = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    UserIp = table.Column<string>(type: "varchar(39)", maxLength: 39, nullable: true),
                    IsAjax = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExceptionLogs_Md5Comparison",
                table: "ExceptionLogs",
                column: "Md5Comparison")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OperationLogs_Md5Comparison",
                table: "OperationLogs",
                column: "Md5Comparison")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_Md5Comparison",
                table: "RequestLogs",
                column: "Md5Comparison")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExceptionLogs");

            migrationBuilder.DropTable(
                name: "OperationLogs");

            migrationBuilder.DropTable(
                name: "RequestLogs");
        }
    }
}
