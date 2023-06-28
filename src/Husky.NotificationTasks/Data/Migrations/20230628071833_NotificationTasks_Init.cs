using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Husky.NotificationTasks.Data.Migrations
{
    /// <inheritdoc />
    public partial class NotificationTasks_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiUrl = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    ContentBody = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ContentType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AutomatedCount = table.Column<int>(type: "int", nullable: false),
                    ManualAttemptedCount = table.Column<int>(type: "int", nullable: false),
                    ReceivedContent = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    FirstTriedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastTriedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduleNextTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTasks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationTasks");
        }
    }
}
