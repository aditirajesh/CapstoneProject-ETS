﻿// <auto-generated />
using System;
using ExpenseTrackingSystem.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExpenseTrackingSystem.Migrations
{
    [DbContext(typeof(ExpenseContext))]
    partial class ExpenseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AuditLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Details")
                        .HasColumnType("text");

                    b.Property<Guid?>("EntityId")
                        .HasColumnType("uuid");

                    b.Property<string>("EntityName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Username");

                    b.ToTable("AuditLogs");
                });

            modelBuilder.Entity("ExpenseTrackingSystem.Models.Expense", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("Username");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("ExpenseTrackingSystem.Models.Receipt", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ExpenseId")
                        .HasColumnType("uuid");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ReceiptName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("ExpenseId")
                        .IsUnique();

                    b.HasIndex("Username");

                    b.ToTable("Receipts");
                });

            modelBuilder.Entity("ExpenseTrackingSystem.Models.User", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("RefreshTokenExpiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.HasKey("Username")
                        .HasName("PK_User");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AuditLog", b =>
                {
                    b.HasOne("ExpenseTrackingSystem.Models.User", null)
                        .WithMany()
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_AuditLog_User");
                });

            modelBuilder.Entity("ExpenseTrackingSystem.Models.Expense", b =>
                {
                    b.HasOne("ExpenseTrackingSystem.Models.User", "User")
                        .WithMany("Expenses")
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Expense_User");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ExpenseTrackingSystem.Models.Receipt", b =>
                {
                    b.HasOne("ExpenseTrackingSystem.Models.Expense", "Expense")
                        .WithOne("Receipt")
                        .HasForeignKey("ExpenseTrackingSystem.Models.Receipt", "ExpenseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Receipt_Expense");

                    b.HasOne("ExpenseTrackingSystem.Models.User", "User")
                        .WithMany("Receipts")
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Receipt_User");

                    b.Navigation("Expense");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ExpenseTrackingSystem.Models.Expense", b =>
                {
                    b.Navigation("Receipt");
                });

            modelBuilder.Entity("ExpenseTrackingSystem.Models.User", b =>
                {
                    b.Navigation("Expenses");

                    b.Navigation("Receipts");
                });
#pragma warning restore 612, 618
        }
    }
}
