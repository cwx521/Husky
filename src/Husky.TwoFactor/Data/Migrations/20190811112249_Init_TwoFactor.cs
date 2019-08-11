using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Husky.TwoFactor.Data.Migrations
{
    public partial class Init_TwoFactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TwoFactorCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "varchar(8)", nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
					ErrorTimes = table.Column<int>(nullable: false),
                    IsUsed = table.Column<bool>(nullable: false),
                    SentTo = table.Column<string>(type: "varchar(50)", nullable: true),
                    UserIdString = table.Column<string>(type: "varchar(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFactorCodes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwoFactorCodes");
        }
    }
}
