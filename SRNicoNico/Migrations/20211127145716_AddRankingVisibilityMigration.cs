using Microsoft.EntityFrameworkCore.Migrations;

namespace SRNicoNico.Migrations {
    public partial class AddRankingVisibilityMigration : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "RankingVisibility",
                columns: table => new {
                    GenreKey = table.Column<string>(nullable: false),
                    IsVisible = table.Column<bool>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_RankingVisibility", x => x.GenreKey);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "RankingVisibility");
        }
    }
}
