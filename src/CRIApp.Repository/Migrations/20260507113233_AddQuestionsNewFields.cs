using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    public partial class AddQuestionsNewFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "ExcludedFamilyTypes",
                table: "Question",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int>(
                name: "ScoringType",
                table: "Question",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcludedFamilyTypes",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "ScoringType",
                table: "Question");
        }
    }
}
