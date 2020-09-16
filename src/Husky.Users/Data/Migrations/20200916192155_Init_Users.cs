using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Husky.Users.Data.Migrations
{
    public partial class Init_Users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserMessageCommonContents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(maxLength: 4000, nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
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
                    Status = table.Column<int>(nullable: false),
                    RegisteredTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
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
                    District = table.Column<string>(maxLength: 16, nullable: true),
                    Location = table.Column<string>(maxLength: 100, nullable: true),
                    ContactName = table.Column<string>(maxLength: 16, nullable: true),
                    ContactPhoneNumber = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
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
                name: "UserLoginRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: true),
                    AttemptedAccount = table.Column<string>(maxLength: 50, nullable: false),
                    SickPassword = table.Column<string>(type: "varchar(88)", maxLength: 88, nullable: true),
                    LoginResult = table.Column<int>(nullable: false),
                    UserAgent = table.Column<string>(maxLength: 500, nullable: true),
                    Ip = table.Column<string>(type: "varchar(39)", maxLength: 39, nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
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
                    Content = table.Column<string>(maxLength: 4000, nullable: false),
                    IsRead = table.Column<bool>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
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
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
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
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
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
                name: "UserReals",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    SocialIdNumber = table.Column<string>(type: "varchar(11)", maxLength: 18, nullable: true),
                    RealName = table.Column<string>(maxLength: 24, nullable: false),
                    RealNamePhonetic = table.Column<string>(type: "varchar(60)", nullable: true),
                    RealNamePhoneticInitials = table.Column<string>(type: "varchar(10)", nullable: true),
                    Sex = table.Column<int>(nullable: true),
                    IsVerified = table.Column<bool>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReals", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserReals_Users_UserId",
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
                    UnionId = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    NickName = table.Column<string>(maxLength: 36, nullable: false),
                    Sex = table.Column<int>(nullable: false),
                    HeadImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    Province = table.Column<string>(maxLength: 24, nullable: true),
                    City = table.Column<string>(maxLength: 24, nullable: true),
                    Country = table.Column<string>(maxLength: 24, nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
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

            migrationBuilder.CreateTable(
                name: "UserWeChatOpenIds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeChatId = table.Column<int>(nullable: false),
                    OpenIdType = table.Column<int>(nullable: false),
                    OpenIdValue = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWeChatOpenIds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWeChatOpenIds_UserWeChats_WeChatId",
                        column: x => x.WeChatId,
                        principalTable: "UserWeChats",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                table: "UserAddresses",
                column: "UserId");

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
                name: "IX_UserReals_SocialIdNumber",
                table: "UserReals",
                column: "SocialIdNumber",
                unique: true,
                filter: "[SocialIdNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserWeChatOpenIds_OpenIdValue",
                table: "UserWeChatOpenIds",
                column: "OpenIdValue",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWeChatOpenIds_WeChatId",
                table: "UserWeChatOpenIds",
                column: "WeChatId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWeChats_PrivateId",
                table: "UserWeChats",
                column: "PrivateId",
                unique: true,
                filter: "[PrivateId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropTable(
                name: "UserLoginRecords");

            migrationBuilder.DropTable(
                name: "UserMessage");

            migrationBuilder.DropTable(
                name: "UserPasswords");

            migrationBuilder.DropTable(
                name: "UserPhones");

            migrationBuilder.DropTable(
                name: "UserReals");

            migrationBuilder.DropTable(
                name: "UserWeChatOpenIds");

            migrationBuilder.DropTable(
                name: "UserMessageCommonContents");

            migrationBuilder.DropTable(
                name: "UserWeChats");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
