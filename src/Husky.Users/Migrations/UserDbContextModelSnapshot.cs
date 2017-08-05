using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Husky.Users.Data;
using Husky.Sugar;

namespace Husky.Users.Migrations
{
    [DbContext(typeof(UserDbContext))]
    partial class UserDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Husky.Users.Data.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("AwaitReactivateTime");

                    b.Property<DateTime>("CreatedTime")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "getdate()");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(32);

                    b.Property<string>("Email")
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40);

                    b.Property<bool>("IsEmailVerified")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.Property<bool>("IsMobileVerified")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.Property<string>("Mobile")
                        .HasColumnType("varchar(15)")
                        .HasMaxLength(15);

                    b.Property<string>("Password")
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40);

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("CreatedTime")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Mobile")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Husky.Users.Data.UserChangeRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "getdate()");

                    b.Property<string>("Description")
                        .HasMaxLength(100);

                    b.Property<string>("FieldName")
                        .HasMaxLength(50);

                    b.Property<bool>("IsBySelf")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.Property<string>("NewValue")
                        .HasMaxLength(100);

                    b.Property<string>("OldValue")
                        .HasMaxLength(100);

                    b.Property<Guid?>("UserId");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("CreateTime")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("UserId");

                    b.ToTable("UserChangeRecords");
                });

            modelBuilder.Entity("Husky.Users.Data.UserLoginRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "getdate()");

                    b.Property<string>("Description")
                        .HasMaxLength(100);

                    b.Property<string>("InputAccount")
                        .HasMaxLength(50);

                    b.Property<string>("Ip")
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40);

                    b.Property<int>("LoginResult")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.Property<string>("SickPassword")
                        .HasMaxLength(18);

                    b.Property<string>("UserAgent")
                        .HasMaxLength(500);

                    b.Property<Guid?>("UserId");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("CreateTime")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("UserId");

                    b.ToTable("UserLoginRecords");
                });

            modelBuilder.Entity("Husky.Users.Data.UserPersonal", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<DateTime>("CreatedTime")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "getdate()");

                    b.Property<DateTime?>("DateOfBirth");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(18);

                    b.Property<string>("FirstNamePhonetic")
                        .IsRequired()
                        .HasMaxLength(18);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(18);

                    b.Property<string>("LastNamePhonetic")
                        .IsRequired()
                        .HasMaxLength(18);

                    b.Property<string>("Location")
                        .HasMaxLength(18);

                    b.Property<byte[]>("Photo");

                    b.Property<int>("Sex")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.HasKey("UserId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("CreatedTime")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("UserPersonals");
                });

            modelBuilder.Entity("Husky.Users.Data.UserChangeRecord", b =>
                {
                    b.HasOne("Husky.Users.Data.User", "User")
                        .WithMany("ChangeRecords")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Husky.Users.Data.UserLoginRecord", b =>
                {
                    b.HasOne("Husky.Users.Data.User", "User")
                        .WithMany("LoginRecords")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Husky.Users.Data.UserPersonal", b =>
                {
                    b.HasOne("Husky.Users.Data.User", "User")
                        .WithOne("Personal")
                        .HasForeignKey("Husky.Users.Data.UserPersonal", "UserId");
                });
        }
    }
}
