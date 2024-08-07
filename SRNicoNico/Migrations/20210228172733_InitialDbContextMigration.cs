using Microsoft.EntityFrameworkCore.Migrations;

namespace SRNicoNico.Migrations {
    public partial class InitialDbContextMigration : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "LocalHistory",
                columns: table => new {
                    VideoId = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    ShortDescription = table.Column<string>(nullable: false),
                    ThumbnailUrl = table.Column<string>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    WatchCount = table.Column<int>(nullable: false),
                    PostedAt = table.Column<long>(nullable: false),
                    LastWatchedAt = table.Column<long>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_LocalHistory", x => x.VideoId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalHistory_LastWatchedAt",
                table: "LocalHistory",
                column: "LastWatchedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "LocalHistory");
        }
    }
}
