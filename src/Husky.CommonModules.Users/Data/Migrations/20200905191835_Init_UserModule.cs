using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Husky.CommonModules.Users.Data.Migrations
{
    public partial class Init_UserModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreditName = table.Column<string>(maxLength: 10, nullable: false),
                    Unit = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserMessageCommonContents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessageCommonContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(maxLength: 36, nullable: true),
                    PhotoUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    RegisteredTime = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Lon = table.Column<decimal>(type: "decimal(11, 6)", nullable: true),
                    Lat = table.Column<decimal>(type: "decimal(11, 6)", nullable: true),
                    Province = table.Column<string>(maxLength: 16, nullable: false),
                    City = table.Column<string>(maxLength: 16, nullable: false),
                    District = table.Column<string>(maxLength: 16, nullable: false),
                    DetailAddress = table.Column<string>(maxLength: 100, nullable: false),
                    ContactName = table.Column<string>(maxLength: 16, nullable: true),
                    ContactPhoneNumber = table.Column<string>(maxLength: 11, nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAddresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCredits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    CreditTypeId = table.Column<int>(nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(8, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCredits_CreditTypes_CreditTypeId",
                        column: x => x.CreditTypeId,
                        principalTable: "CreditTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCredits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: true),
                    AttemptedAccount = table.Column<string>(maxLength: 50, nullable: false),
                    SickPassword = table.Column<string>(type: "varchar(88)", maxLength: 88, nullable: true),
                    LoginResult = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(maxLength: 500, nullable: true),
                    Ip = table.Column<string>(type: "varchar(39)", maxLength: 39, nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLoginRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserMessage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    CommonContentId = table.Column<int>(nullable: true),
                    Content = table.Column<string>(maxLength: 4000, nullable: true),
                    IsRead = table.Column<bool>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMessage_UserMessageCommonContents_CommonContentId",
                        column: x => x.CommonContentId,
                        principalTable: "UserMessageCommonContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserMessage_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPasswords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Password = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    IsObsoleted = table.Column<bool>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPasswords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPhones",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    Number = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    IsVerified = table.Column<bool>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhones", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserPhones_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWeChats",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    PrivateId = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    MobilePlatformOpenId = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    MiniProgramOpenId = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    UnionId = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    NickName = table.Column<string>(maxLength: 36, nullable: false),
                    Sex = table.Column<int>(nullable: false),
                    HeadImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    Province = table.Column<string>(maxLength: 24, nullable: true),
                    City = table.Column<string>(maxLength: 24, nullable: true),
                    Country = table.Column<string>(maxLength: 24, nullable: true),
                    AccessToken = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    RefreshToken = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWeChats", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserWeChats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                table: "UserAddresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredits_CreditTypeId",
                table: "UserCredits",
                column: "CreditTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredits_UserId_CreditTypeId",
                table: "UserCredits",
                columns: new[] { "UserId", "CreditTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginRecords_UserId",
                table: "UserLoginRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessage_CommonContentId",
                table: "UserMessage",
                column: "CommonContentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessage_UserId",
                table: "UserMessage",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswords_UserId",
                table: "UserPasswords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPhones_Number",
                table: "UserPhones",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWeChats_MiniProgramOpenId",
                table: "UserWeChats",
                column: "MiniProgramOpenId",
                unique: true,
                filter: "[MiniProgramOpenId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserWeChats_MobilePlatformOpenId",
                table: "UserWeChats",
                column: "MobilePlatformOpenId",
                unique: true,
                filter: "[MobilePlatformOpenId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserWeChats_UnionId",
                table: "UserWeChats",
                column: "UnionId",
                unique: true,
                filter: "[UnionId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropTable(
                name: "UserCredits");

            migrationBuilder.DropTable(
                name: "UserLoginRecords");

            migrationBuilder.DropTable(
                name: "UserMessage");

            migrationBuilder.DropTable(
                name: "UserPasswords");

            migrationBuilder.DropTable(
                name: "UserPhones");

            migrationBuilder.DropTable(
                name: "UserWeChats");

            migrationBuilder.DropTable(
                name: "CreditTypes");

            migrationBuilder.DropTable(
                name: "UserMessageCommonContents");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
