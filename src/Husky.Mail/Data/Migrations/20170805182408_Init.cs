using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Husky.Mail.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailSmtpProviders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CredentialName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Host = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    IsInUse = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    PasswordEncrypted = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    Port = table.Column<int>(nullable: false, defaultValueSql: "0"),
                    SenderDisplayName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    SenderMailAddress = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Ssl = table.Column<bool>(nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailSmtpProviders", x => x.Id);
                    table.UniqueConstraint("AK_MailSmtpProviders_Host_CredentialName", x => new { x.Host, x.CredentialName });
                });

            migrationBuilder.CreateTable(
                name: "MailRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Body = table.Column<string>(maxLength: 4000, nullable: true),
                    Cc = table.Column<string>(maxLength: 2000, nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Exception = table.Column<string>(maxLength: 500, nullable: true),
                    IsHtml = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    IsSuccessful = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    SmtpId = table.Column<Guid>(nullable: true),
                    Subject = table.Column<string>(maxLength: 200, nullable: true),
                    To = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailRecords", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<Guid>(nullable: false),
                    ContentStream = table.Column<byte[]>(nullable: true),
                    ContentType = table.Column<string>(maxLength: 32, nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    MailId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailRecordAttachments", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_MailRecordAttachments_MailRecords_MailId",
                        column: x => x.MailId,
                        principalTable: "MailRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MailRecords_CreateTime",
                table: "MailRecords",
                column: "CreateTime")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_MailRecords_SmtpId",
                table: "MailRecords",
                column: "SmtpId");

            migrationBuilder.CreateIndex(
                name: "IX_MailRecordAttachments_CreatedTime",
                table: "MailRecordAttachments",
                column: "CreatedTime")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_MailRecordAttachments_MailId",
                table: "MailRecordAttachments",
                column: "MailId");
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
