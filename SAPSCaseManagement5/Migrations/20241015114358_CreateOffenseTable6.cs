using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAPSCaseManagement5.Migrations
{
    /// <inheritdoc />
    public partial class CreateOffenseTable6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CaseManagers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CaseManagers");
        }
    }
}
