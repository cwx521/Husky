using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Husky.Principal.Users.Data.Migrations
{
    public partial class Init_Users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    PhotoUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RegisteredTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Location_Lat = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Location_Lon = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Location_LatLonType = table.Column<int>(type: "int", nullable: true),
                    DisplayAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayAddressAlternate = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    City = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    District = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: true),
                    Street = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccuratePlace = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    ContactPhoneNumber = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
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
                name: "UserEmails",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EmailAddress = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEmails", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserEmails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInGroups",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInGroups", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_UserInGroups_UserGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "UserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserInGroups_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AttemptedAccount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SickPassword = table.Column<string>(type: "varchar(88)", maxLength: 88, nullable: true),
                    LoginResult = table.Column<int>(type: "int", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Ip = table.Column<string>(type: "varchar(45)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLoginRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPasswords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    IsObsolete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SocialIdNumber = table.Column<string>(type: "varchar(18)", maxLength: 18, nullable: true),
                    RealName = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: true),
                    Sex = table.Column<int>(type: "int", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PrivateId = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    UnionId = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    NickName = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Sex = table.Column<int>(type: "int", nullable: false),
                    HeadImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: true),
                    City = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeChatId = table.Column<int>(type: "int", nullable: false),
                    OpenIdType = table.Column<int>(type: "int", nullable: false),
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
                name: "IX_UserEmails_EmailAddress",
                table: "UserEmails",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInGroups_GroupId",
                table: "UserInGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginRecords_UserId",
                table: "UserLoginRecords",
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
                name: "IX_UserWeChatOpenIds_OpenIdValue",
                table: "UserWeChatOpenIds",
                column: "OpenIdValue",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWeChatOpenIds_WeChatId",
                table: "UserWeChatOpenIds",
                column: "WeChatId");

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
                name: "UserEmails");

            migrationBuilder.DropTable(
                name: "UserInGroups");

            migrationBuilder.DropTable(
                name: "UserLoginRecords");

            migrationBuilder.DropTable(
                name: "UserPasswords");

            migrationBuilder.DropTable(
                name: "UserPhones");

            migrationBuilder.DropTable(
                name: "UserReals");

            migrationBuilder.DropTable(
                name: "UserWeChatOpenIds");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "UserWeChats");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
