using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
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
            modelBuilder.Entity<LocalHistory>().ToTable(nameof(LocalHistory))
                .HasIndex(i => i.LastWatchedAt);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

            var env = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            optionsBuilder.UseSqlite($"Filename={Path.Combine(env, "SRNicoNico", "viewer.db")}");
        }
    }
}
