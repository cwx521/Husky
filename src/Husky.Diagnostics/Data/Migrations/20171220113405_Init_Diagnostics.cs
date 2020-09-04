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
                    Md5Comparison = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    Url = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true),
                    HttpMethod = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    ExceptionType = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Message = table.Column<string>(maxLength: 1000, nullable: true),
                    Source = table.Column<string>(nullable: true),
                    StackTrace = table.Column<string>(nullable: true),
                    UserIdString = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    UserName = table.Column<string>(maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    UserIp = table.Column<string>(type: "varchar(39)", maxLength: 39, nullable: true),
                    Count = table.Column<int>(nullable: false),
                    FirstTime = table.Column<DateTime>(nullable: false),
                    LastTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(maxLength: 1000, nullable: true),
                    Referrer = table.Column<string>(maxLength: 1000, nullable: true),
                    Data = table.Column<string>(nullable: true),
                    HttpMethod = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    UserIdString = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    UserName = table.Column<string>(maxLength: 100, nullable: true),
                    IsAjax = table.Column<bool>(nullable: false),
                    UserAgent = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    UserIp = table.Column<string>(type: "varchar(39)", maxLength: 39, nullable: true),
                    Time = table.Column<DateTime>(nullable: false)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExceptionLogs");

            migrationBuilder.DropTable(
                name: "RequestLogs");
        }
    }
}
