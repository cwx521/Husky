﻿// <auto-generated />
using Husky.KeyValues.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Husky.KeyValues.Data.Migrations
{
    [DbContext(typeof(KeyValueDbContext))]
    [Migration("20180824063957_Init_KeyValue")]
    partial class Init_KeyValue
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Husky.Configuration.Data.KeyValue", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Value")
                        .HasMaxLength(2000);

                    b.HasKey("Key");

                    b.ToTable("KeyValues");
                });
#pragma warning restore 612, 618
        }
    }
}