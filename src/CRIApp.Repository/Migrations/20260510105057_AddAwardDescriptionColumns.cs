using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddAwardDescriptionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Award",
                type: "varchar(1000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Award",
                type: "varchar(1000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionPt",
                table: "Award",
                type: "varchar(1000)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Award");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Award");

            migrationBuilder.DropColumn(
                name: "DescriptionPt",
                table: "Award");
        }
    }
}
