using Microsoft.EntityFrameworkCore.Migrations;

namespace SRNicoNico.Migrations {
    public partial class AddMutedAccountMigration : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "MutedAccount",
                columns: table => new {
                    Key = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountType = table.Column<int>(nullable: false),
                    AccountId = table.Column<string>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_MutedAccount", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "MutedAccount");
        }
    }
}
