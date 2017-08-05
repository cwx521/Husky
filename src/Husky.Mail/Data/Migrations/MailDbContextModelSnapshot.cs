using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Husky.Mail.Data;

namespace Husky.Mail.Data.Migrations
{
    [DbContext(typeof(MailDbContext))]
    partial class MailDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Husky.Mail.Data.MailRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body")
                        .HasMaxLength(4000);

                    b.Property<string>("Cc")
                        .HasMaxLength(2000);

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "getdate()");

                    b.Property<string>("Exception")
                        .HasMaxLength(500);

                    b.Property<bool>("IsHtml")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.Property<bool>("IsSuccessful")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.Property<Guid?>("SmtpId");

                    b.Property<string>("Subject")
                        .HasMaxLength(200);

                    b.Property<string>("To")
                        .HasMaxLength(2000);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("CreateTime")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("SmtpId");

                    b.ToTable("MailRecords");
                });

            modelBuilder.Entity("Husky.Mail.Data.MailRecordAttachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("ContentStream");

                    b.Property<string>("ContentType")
                        .HasMaxLength(32);

                    b.Property<DateTime>("CreatedTime")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "getdate()");

                    b.Property<Guid>("MailId");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("CreatedTime")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("MailId");

                    b.ToTable("MailRecordAttachments");
                });

            modelBuilder.Entity("Husky.Mail.Data.MailSmtpProvider", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CredentialName")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Host")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<bool>("IsInUse")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.Property<string>("PasswordEncrypted")
                        .IsRequired()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<int>("Port")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.Property<string>("SenderDisplayName")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("SenderMailAddress")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<bool>("Ssl")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.HasKey("Id");

                    b.HasAlternateKey("Host", "CredentialName");

                    b.ToTable("MailSmtpProviders");
                });

            modelBuilder.Entity("Husky.Mail.Data.MailRecord", b =>
                {
                    b.HasOne("Husky.Mail.Data.MailSmtpProvider", "Smtp")
                        .WithMany("SentMails")
                        .HasForeignKey("SmtpId");
                });

            modelBuilder.Entity("Husky.Mail.Data.MailRecordAttachment", b =>
                {
                    b.HasOne("Husky.Mail.Data.MailRecord", "Mail")
                        .WithMany("Attachments")
                        .HasForeignKey("MailId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
