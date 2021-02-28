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


        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            // 最終視聴日にインデックスを付ける
            var history = modelBuilder.Entity<LocalHistory>().ToTable(nameof(LocalHistory));
            history.HasIndex(i => i.LastWatchedAt);
            history.Property(p => p.LastWatchedAt)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            history.Property(p => p.PostedAt)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

            var env = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            optionsBuilder.UseSqlite($"Filename={Path.Combine(env, "SRNicoNico", "viewer.db")}");
        }
    }
}
