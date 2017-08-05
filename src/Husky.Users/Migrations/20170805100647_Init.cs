using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Husky.Users.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AwaitReactivateTime = table.Column<DateTime>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    DisplayName = table.Column<string>(maxLength: 32, nullable: true),
                    Email = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true),
                    IsEmailVerified = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    IsMobileVerified = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    Mobile = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true),
                    Password = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true),
                    Status = table.Column<int>(nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "UserChangeRecords",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Description = table.Column<string>(maxLength: 100, nullable: true),
                    FieldName = table.Column<string>(maxLength: 50, nullable: true),
                    IsBySelf = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    NewValue = table.Column<string>(maxLength: 100, nullable: true),
                    OldValue = table.Column<string>(maxLength: 100, nullable: true),
                    UserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChangeRecords", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_UserChangeRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginRecords",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Description = table.Column<string>(maxLength: 100, nullable: true),
                    InputAccount = table.Column<string>(maxLength: 50, nullable: true),
                    Ip = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true),
                    LoginResult = table.Column<int>(nullable: false, defaultValueSql: "0"),
                    SickPassword = table.Column<string>(maxLength: 18, nullable: true),
                    UserAgent = table.Column<string>(maxLength: 500, nullable: true),
                    UserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginRecords", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_UserLoginRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPersonals",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    DateOfBirth = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 18, nullable: false),
                    FirstNamePhonetic = table.Column<string>(maxLength: 18, nullable: false),
                    LastName = table.Column<string>(maxLength: 18, nullable: false),
                    LastNamePhonetic = table.Column<string>(maxLength: 18, nullable: false),
                    Location = table.Column<string>(maxLength: 18, nullable: true),
                    Photo = table.Column<byte[]>(nullable: true),
                    Sex = table.Column<int>(nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPersonals", x => x.UserId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_UserPersonals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedTime",
                table: "Users",
                column: "CreatedTime")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Mobile",
                table: "Users",
                column: "Mobile",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_UserChangeRecords_CreateTime",
                table: "UserChangeRecords",
                column: "CreateTime")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_UserChangeRecords_UserId",
                table: "UserChangeRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginRecords_CreateTime",
                table: "UserLoginRecords",
                column: "CreateTime")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginRecords_UserId",
                table: "UserLoginRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPersonals_CreatedTime",
                table: "UserPersonals",
                column: "CreatedTime")
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserChangeRecords");

            migrationBuilder.DropTable(
                name: "UserLoginRecords");

            migrationBuilder.DropTable(
                name: "UserPersonals");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
