﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SRNicoNico.Models;

namespace SRNicoNico.Migrations {
    [DbContext(typeof(ViewerDbContext))]
    [Migration("20211127145716_AddRankingVisibilityMigration")]
    partial class AddRankingVisibilityMigration {
        protected override void BuildTargetModel(ModelBuilder modelBuilder) {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.18");

            modelBuilder.Entity("SRNicoNico.Entities.ABRepeatPosition", b => {
                b.Property<string>("VideoId")
                    .HasColumnType("TEXT");

                b.Property<double>("RepeatA")
                    .HasColumnType("REAL");

                b.Property<double>("RepeatB")
                    .HasColumnType("REAL");

                b.HasKey("VideoId");

                b.ToTable("ABRepeatPosition");
            });

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

            modelBuilder.Entity("SRNicoNico.Entities.RankingVisibility", b => {
                b.Property<string>("GenreKey")
                    .HasColumnType("TEXT");

                b.Property<bool>("IsVisible")
                    .HasColumnType("INTEGER");

                b.HasKey("GenreKey");

                b.ToTable("RankingVisibility");
            });
#pragma warning restore 612, 618
        }
    }
}
