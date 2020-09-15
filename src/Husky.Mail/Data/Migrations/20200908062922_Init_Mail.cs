using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Husky.Mail.Data.Migrations
{
    public partial class Init_Mail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailSmtpProviders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Host = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CredentialName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Port = table.Column<int>(nullable: false),
                    Ssl = table.Column<bool>(nullable: false),
                    PasswordEncrypted = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    SenderMailAddress = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    SenderDisplayName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    IsInUse = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailSmtpProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmtpId = table.Column<Guid>(nullable: true),
                    Subject = table.Column<string>(maxLength: 200, nullable: false),
                    Body = table.Column<string>(nullable: false),
                    IsHtml = table.Column<bool>(nullable: false),
                    To = table.Column<string>(maxLength: 2000, nullable: false),
                    Cc = table.Column<string>(maxLength: 2000, nullable: true),
                    Exception = table.Column<string>(maxLength: 500, nullable: true),
                    IsSuccessful = table.Column<bool>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailRecords_MailSmtpProviders_SmtpId",
                        column: x => x.SmtpId,
                        principalTable: "MailSmtpProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MailRecordAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MailId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ContentStream = table.Column<byte[]>(nullable: false),
                    ContentType = table.Column<string>(maxLength: 32, nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailRecordAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailRecordAttachments_MailRecords_MailId",
                        column: x => x.MailId,
                        principalTable: "MailRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MailRecordAttachments_MailId",
                table: "MailRecordAttachments",
                column: "MailId");

            migrationBuilder.CreateIndex(
                name: "IX_MailRecords_SmtpId",
                table: "MailRecords",
                column: "SmtpId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailRecordAttachments");

            migrationBuilder.DropTable(
                name: "MailRecords");

            migrationBuilder.DropTable(
                name: "MailSmtpProviders");
        }
    }
}
