using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SRNicoNico.Entities;

namespace SRNicoNico.Models {
    /// <summary>
    /// ローカルのSQLiteにアクセスするDbContext
    /// </summary>
    public class ViewerDbContext : DbContext {

        /// <summary>
        /// ローカル視聴履歴テーブル
        /// </summary>
        public DbSet<LocalHistory> LocalHistories => Set<LocalHistory>();
        /// <summary>
        /// ABリピート管理テーブル
        /// </summary>
        public DbSet<ABRepeatPosition> ABRepeatPositions => Set<ABRepeatPosition>();

        /// <summary>
        /// ABリピート管理テーブル
        /// </summary>
        public DbSet<RankingVisibility> RankingVisibilities => Set<RankingVisibility>();

        /// <summary>
        /// ミュート設定テーブル
        /// </summary>
        public DbSet<MutedAccount> MutedAccounts => Set<MutedAccount>();

        /// <summary>
        /// 検索履歴テーブル
        /// </summary>
        public DbSet<SearchHistory> SearchHistories => Set<SearchHistory>();


        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            // 最終視聴日にインデックスを付ける
            var history = modelBuilder.Entity<LocalHistory>().ToTable(nameof(LocalHistory));
            history.HasIndex(i => i.LastWatchedAt);
            history.Property(p => p.LastWatchedAt)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            history.Property(p => p.PostedAt)
                .HasConversion(new DateTimeOffsetToBinaryConverter());

            modelBuilder.Entity<ABRepeatPosition>().ToTable(nameof(ABRepeatPosition));
            modelBuilder.Entity<RankingVisibility>().ToTable(nameof(RankingVisibility));
            modelBuilder.Entity<MutedAccount>().ToTable(nameof(MutedAccount));
            modelBuilder.Entity<SearchHistory>().ToTable(nameof(SearchHistory));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

            var env = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            optionsBuilder.UseSqlite($"Filename={Path.Combine(env, "SRNicoNico", "viewer.db")}");
        }
    }
}
