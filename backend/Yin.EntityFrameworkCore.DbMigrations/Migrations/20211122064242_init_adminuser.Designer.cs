﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Yin.EntityFrameworkCore.Context;

#nullable disable

namespace Yin.EntityFrameworkCore.DbMigrations.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20211122064242_init_adminuser")]
    partial class init_adminuser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Yin.Domain.AggregateRoot.AdminUserAggregate.AdminRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("Creator")
                        .HasColumnType("char(36)");

                    b.Property<string>("Description")
                        .HasMaxLength(1024)
                        .HasColumnType("varchar(1024)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("ModifyTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("Sort")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("AdminRole", (string)null);
                });

            modelBuilder.Entity("Yin.Domain.AggregateRoot.AdminUserAggregate.AdminUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Account")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("Creator")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("LastLoginErrorTime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("LastLoginSuccessTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("LoginErrorCount")
                        .HasColumnType("int");

                    b.Property<Guid?>("ModifiedBy")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("ModifyTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("AdminUser", (string)null);
                });

            modelBuilder.Entity("Yin.Domain.AggregateRoot.AdminUserAggregate.AdminUserRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.ToTable("AdminUserRole", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
