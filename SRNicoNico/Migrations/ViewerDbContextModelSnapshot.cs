﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SRNicoNico.Models;

namespace SRNicoNico.Migrations {
    [DbContext(typeof(ViewerDbContext))]
    partial class ViewerDbContextModelSnapshot : ModelSnapshot {
        protected override void BuildModel(ModelBuilder modelBuilder) {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.22");

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

            modelBuilder.Entity("SRNicoNico.Entities.MutedAccount", b => {
                b.Property<int>("Key")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<string>("AccountId")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<int>("AccountType")
                    .HasColumnType("INTEGER");

                b.HasKey("Key");

                b.ToTable("MutedAccount");
            });

            modelBuilder.Entity("SRNicoNico.Entities.RankingVisibility", b => {
                b.Property<string>("GenreKey")
                    .HasColumnType("TEXT");

                b.Property<bool>("IsVisible")
                    .HasColumnType("INTEGER");

                b.HasKey("GenreKey");

                b.ToTable("RankingVisibility");
            });

            modelBuilder.Entity("SRNicoNico.Entities.SearchHistory", b => {
                b.Property<string>("Query")
                    .HasColumnType("TEXT");

                b.Property<int>("Order")
                    .HasColumnType("INTEGER");

                b.HasKey("Query");

                b.ToTable("SearchHistory");
            });
#pragma warning restore 612, 618
        }
    }
}
