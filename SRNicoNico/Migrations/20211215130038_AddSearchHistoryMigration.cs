using Microsoft.EntityFrameworkCore.Migrations;

namespace SRNicoNico.Migrations {
    public partial class AddSearchHistoryMigration : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "SearchHistory",
                columns: table => new {
                    Query = table.Column<string>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_SearchHistory", x => x.Query);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "SearchHistory");
        }
    }
}
