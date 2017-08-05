using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Husky.TwoFactor.Data;
using Husky.Sugar;

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

                    b.Property<DateTime>("CreatedTime");

                    b.Property<bool>("IsUsed");

                    b.Property<string>("PassCode")
                        .HasColumnType("varchar(8)")
                        .HasMaxLength(24);

                    b.Property<int>("Purpose");

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
