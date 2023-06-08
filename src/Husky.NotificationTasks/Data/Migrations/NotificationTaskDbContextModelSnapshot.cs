﻿// <auto-generated />
using System;
using Husky.FileStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Husky.NotificationTasks.Data.Migrations
{
    [DbContext(typeof(NotificationTaskDbContext))]
    partial class NotificationTaskDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Husky.NotificationTasks.Data.NotificationTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ApiUrl")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<int>("AutomatedCount")
                        .HasColumnType("int");

                    b.Property<string>("ContentBody")
                        .HasMaxLength(4000)
                        .HasColumnType("varchar(4000)");

                    b.Property<int>("ContentType")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<DateTime?>("FirstTriedTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastTriedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("ManualAttemptedCount")
                        .HasColumnType("int");

                    b.Property<string>("ReceivedContent")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<DateTime?>("ScheduleNextTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("NotificationTasks");
                });
#pragma warning restore 612, 618
        }
    }
}