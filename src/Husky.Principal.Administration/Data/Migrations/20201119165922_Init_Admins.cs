﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Husky.Principal.Administration.Data.Migrations
{
    public partial class Init_Admins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    Powers = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    IsInitiator = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminInRoles",
                columns: table => new
                {
                    AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminInRoles", x => new { x.AdminId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AdminInRoles_AdminRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AdminRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdminInRoles_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminInRoles_RoleId",
                table: "AdminInRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminRoles_RoleName",
                table: "AdminRoles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserId",
                table: "Admins",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminInRoles");

            migrationBuilder.DropTable(
                name: "AdminRoles");

            migrationBuilder.DropTable(
                name: "Admins");
        }
    }
}
