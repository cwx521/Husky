using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Husky.TwoFactor.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TwoFactorCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    IsUsed = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    PassCode = table.Column<string>(type: "varchar(8)", maxLength: 24, nullable: true),
                    Purpose = table.Column<int>(nullable: false, defaultValueSql: "0"),
                    SentTo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFactorCodes", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorCodes_CreatedTime",
                table: "TwoFactorCodes",
                column: "CreatedTime")
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwoFactorCodes");
        }
    }
}
