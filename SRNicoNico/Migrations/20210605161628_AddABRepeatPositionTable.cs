using Microsoft.EntityFrameworkCore.Migrations;

namespace SRNicoNico.Migrations {
    public partial class AddABRepeatPositionTable : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "ABRepeatPosition",
                columns: table => new {
                    VideoId = table.Column<string>(nullable: false),
                    RepeatA = table.Column<double>(nullable: false),
                    RepeatB = table.Column<double>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_ABRepeatPosition", x => x.VideoId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "ABRepeatPosition");
        }
    }
}
