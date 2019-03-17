﻿// <auto-generated />
using System;
using BinaryDiff.Result.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinaryDiff.Result.Infrastructure.Database.Migrations
{
    [DbContext(typeof(ResultContext))]
    [Migration("20190317200442_initial_migration")]
    partial class initial_migration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("BinaryDiff.Result.Domain.Models.DiffResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("DiffId");

                    b.Property<int>("Result");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.HasIndex("DiffId");

                    b.ToTable("DiffResults");
                });

            modelBuilder.Entity("BinaryDiff.Result.Domain.Models.InputDifference", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Length");

                    b.Property<int>("Offset");

                    b.Property<Guid>("ResultId");

                    b.HasKey("Id");

                    b.HasIndex("ResultId");

                    b.ToTable("Differences");
                });

            modelBuilder.Entity("BinaryDiff.Result.Domain.Models.InputDifference", b =>
                {
                    b.HasOne("BinaryDiff.Result.Domain.Models.DiffResult", "Result")
                        .WithMany("Differences")
                        .HasForeignKey("ResultId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
