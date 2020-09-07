﻿// <auto-generated />
using System;
using Husky.CommonModules.Users.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Husky.CommonModules.Users.Data.Migrations
{
    [DbContext(typeof(UserModuleDbContext))]
    partial class UserModuleDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Husky.CommonModules.Users.Data.CreditType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreditName")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.Property<string>("Unit")
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("CreditTypes");
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(36)")
                        .HasMaxLength(36);

                    b.Property<string>("PhotoUrl")
                        .HasColumnType("varchar(500)")
                        .HasMaxLength(500);

                    b.Property<DateTime>("RegisteredTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserAddress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(16)")
                        .HasMaxLength(16);

                    b.Property<string>("ContactName")
                        .HasColumnType("nvarchar(16)")
                        .HasMaxLength(16);

                    b.Property<string>("ContactPhoneNumber")
                        .HasColumnType("nvarchar(11)")
                        .HasMaxLength(11);

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("DetailAddress")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("District")
                        .IsRequired()
                        .HasColumnType("nvarchar(16)")
                        .HasMaxLength(16);

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<decimal?>("Lat")
                        .HasColumnType("decimal(11, 6)");

                    b.Property<decimal?>("Lon")
                        .HasColumnType("decimal(11, 6)");

                    b.Property<string>("Province")
                        .IsRequired()
                        .HasColumnType("nvarchar(16)")
                        .HasMaxLength(16);

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserAddresses");
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserCredit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<int>("CreditTypeId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreditTypeId");

                    b.HasIndex("UserId", "CreditTypeId")
                        .IsUnique();

                    b.ToTable("UserCredits");
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserLoginRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AttemptedAccount")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Ip")
                        .HasColumnType("varchar(39)")
                        .HasMaxLength(39);

                    b.Property<int>("LoginResult")
                        .HasColumnType("int");

                    b.Property<string>("SickPassword")
                        .HasColumnType("varchar(88)")
                        .HasMaxLength(88);

                    b.Property<string>("UserAgent")
                        .HasColumnType("nvarchar(500)")
                        .HasMaxLength(500);

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserLoginRecords");
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CommonContentId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(4000)")
                        .HasMaxLength(4000);

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommonContentId");

                    b.HasIndex("UserId");

                    b.ToTable("UserMessage");
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserMessageCommonContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(4000)")
                        .HasMaxLength(4000);

                    b.HasKey("Id");

                    b.ToTable("UserMessageCommonContents");
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserPassword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsObsoleted")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40);

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserPasswords");
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserPhone", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("varchar(11)")
                        .HasMaxLength(11);

                    b.HasKey("UserId");

                    b.HasIndex("Number")
                        .IsUnique();

                    b.ToTable("UserPhones");
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserWeChat", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("AccessToken")
                        .HasColumnType("varchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(24)")
                        .HasMaxLength(24);

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(24)")
                        .HasMaxLength(24);

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("HeadImageUrl")
                        .IsRequired()
                        .HasColumnType("varchar(500)")
                        .HasMaxLength(500);

                    b.Property<string>("MiniProgramOpenId")
                        .HasColumnType("varchar(32)")
                        .HasMaxLength(32);

                    b.Property<string>("MobilePlatformOpenId")
                        .HasColumnType("varchar(32)")
                        .HasMaxLength(32);

                    b.Property<string>("NickName")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)")
                        .HasMaxLength(36);

                    b.Property<string>("PrivateId")
                        .HasColumnType("varchar(32)")
                        .HasMaxLength(32);

                    b.Property<string>("Province")
                        .HasColumnType("nvarchar(24)")
                        .HasMaxLength(24);

                    b.Property<string>("RefreshToken")
                        .HasColumnType("varchar(128)")
                        .HasMaxLength(128);

                    b.Property<int>("Sex")
                        .HasColumnType("int");

                    b.Property<string>("UnionId")
                        .HasColumnType("varchar(32)")
                        .HasMaxLength(32);

                    b.HasKey("UserId");

                    b.HasIndex("MiniProgramOpenId")
                        .IsUnique()
                        .HasFilter("[MiniProgramOpenId] IS NOT NULL");

                    b.HasIndex("MobilePlatformOpenId")
                        .IsUnique()
                        .HasFilter("[MobilePlatformOpenId] IS NOT NULL");

                    b.HasIndex("UnionId")
                        .IsUnique()
                        .HasFilter("[UnionId] IS NOT NULL");

                    b.ToTable("UserWeChats");
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserAddress", b =>
                {
                    b.HasOne("Husky.CommonModules.Users.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserCredit", b =>
                {
                    b.HasOne("Husky.CommonModules.Users.Data.CreditType", "CreditType")
                        .WithMany()
                        .HasForeignKey("CreditTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Husky.CommonModules.Users.Data.User", "User")
                        .WithMany("Credits")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserLoginRecord", b =>
                {
                    b.HasOne("Husky.CommonModules.Users.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserMessage", b =>
                {
                    b.HasOne("Husky.CommonModules.Users.Data.UserMessageCommonContent", "CommonContent")
                        .WithMany("UserMessages")
                        .HasForeignKey("CommonContentId");

                    b.HasOne("Husky.CommonModules.Users.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserPassword", b =>
                {
                    b.HasOne("Husky.CommonModules.Users.Data.User", "User")
                        .WithMany("Passwords")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserPhone", b =>
                {
                    b.HasOne("Husky.CommonModules.Users.Data.User", "User")
                        .WithOne("Phone")
                        .HasForeignKey("Husky.CommonModules.Users.Data.UserPhone", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Husky.CommonModules.Users.Data.UserWeChat", b =>
                {
                    b.HasOne("Husky.CommonModules.Users.Data.User", "User")
                        .WithOne("WeChat")
                        .HasForeignKey("Husky.CommonModules.Users.Data.UserWeChat", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
