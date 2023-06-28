using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Husky.Mail.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mail_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailSmtpProviders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Host = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    Ssl = table.Column<bool>(type: "bit", nullable: false),
                    CredentialName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PasswordEncrypted = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    SenderMailAddress = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SenderDisplayName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IsInUse = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailSmtpProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmtpId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsHtml = table.Column<bool>(type: "bit", nullable: false),
                    To = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Cc = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailRecords_MailSmtpProviders_SmtpId",
                        column: x => x.SmtpId,
                        principalTable: "MailSmtpProviders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MailRecordAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MailId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContentBytes = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
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

            migrationBuilder.CreateIndex(
                name: "IX_MailSmtpProviders_CredentialName",
                table: "MailSmtpProviders",
                column: "CredentialName",
                unique: true,
                filter: "[CredentialName] IS NOT NULL");
        }

        /// <inheritdoc />
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
