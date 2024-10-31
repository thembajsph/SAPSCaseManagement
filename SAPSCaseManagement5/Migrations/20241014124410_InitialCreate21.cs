using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAPSCaseManagement5.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Offense",
                table: "CriminalRecords");

            migrationBuilder.AddColumn<int>(
                name: "OffenseId",
                table: "CriminalRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Offense",
                columns: table => new
                {
                    OffenseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OffenseName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offense", x => x.OffenseId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CriminalRecords_OffenseId",
                table: "CriminalRecords",
                column: "OffenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CriminalRecords_Offense_OffenseId",
                table: "CriminalRecords",
                column: "OffenseId",
                principalTable: "Offense",
                principalColumn: "OffenseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CriminalRecords_Offense_OffenseId",
                table: "CriminalRecords");

            migrationBuilder.DropTable(
                name: "Offense");

            migrationBuilder.DropIndex(
                name: "IX_CriminalRecords_OffenseId",
                table: "CriminalRecords");

            migrationBuilder.DropColumn(
                name: "OffenseId",
                table: "CriminalRecords");

            migrationBuilder.AddColumn<string>(
                name: "Offense",
                table: "CriminalRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
