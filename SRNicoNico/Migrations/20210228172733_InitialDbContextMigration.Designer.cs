﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SRNicoNico.Models;

namespace SRNicoNico.Migrations {
    [DbContext(typeof(ViewerDbContext))]
    [Migration("20210228172733_InitialDbContextMigration")]
    partial class InitialDbContextMigration {
        protected override void BuildTargetModel(ModelBuilder modelBuilder) {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.12");

            modelBuilder.Entity("SRNicoNico.Entities.LocalHistory", b => {
                b.Property<string>("VideoId")
                    .HasColumnType("TEXT");

                b.Property<int>("Duration")
                    .HasColumnType("INTEGER");

                b.Property<long>("LastWatchedAt")
                    .HasColumnType("INTEGER");

                b.Property<long>("PostedAt")
                    .HasColumnType("INTEGER");

                b.Property<string>("ShortDescription")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<string>("ThumbnailUrl")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<int>("WatchCount")
                    .HasColumnType("INTEGER");

                b.HasKey("VideoId");

                b.HasIndex("LastWatchedAt");

                b.ToTable("LocalHistory");
            });
#pragma warning restore 612, 618
        }
    }
}
