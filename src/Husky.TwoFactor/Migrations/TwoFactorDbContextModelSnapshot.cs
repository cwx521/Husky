using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Husky.TwoFactor.Data;

namespace Husky.TwoFactor.Migrations
{
    [DbContext(typeof(TwoFactorDbContext))]
    partial class TwoFactorDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Husky.TwoFactor.Data.TwoFactorCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedTime")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "getdate()");

                    b.Property<bool>("IsUsed")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.Property<string>("PassCode")
                        .HasColumnType("varchar(8)")
                        .HasMaxLength(24);

                    b.Property<int>("Purpose")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValueSql", "0");

                    b.Property<string>("SentTo")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<Guid?>("UserId");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("CreatedTime")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("TwoFactorCodes");
                });
        }
    }
}
