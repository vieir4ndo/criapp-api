using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslationColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextEn",
                table: "Recommendation",
                type: "varchar(500)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextPt",
                table: "Recommendation",
                type: "varchar(500)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextEn",
                table: "Question",
                type: "varchar(250)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextPt",
                table: "Question",
                type: "varchar(250)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerEn",
                table: "Faq",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerPt",
                table: "Faq",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionEn",
                table: "Faq",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionPt",
                table: "Faq",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "AudiovisualResource",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionPt",
                table: "AudiovisualResource",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "AudiovisualResource",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NamePt",
                table: "AudiovisualResource",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextEn",
                table: "Recommendation");

            migrationBuilder.DropColumn(
                name: "TextPt",
                table: "Recommendation");

            migrationBuilder.DropColumn(
                name: "TextEn",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "TextPt",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "AnswerEn",
                table: "Faq");

            migrationBuilder.DropColumn(
                name: "AnswerPt",
                table: "Faq");

            migrationBuilder.DropColumn(
                name: "QuestionEn",
                table: "Faq");

            migrationBuilder.DropColumn(
                name: "QuestionPt",
                table: "Faq");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "AudiovisualResource");

            migrationBuilder.DropColumn(
                name: "DescriptionPt",
                table: "AudiovisualResource");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "AudiovisualResource");

            migrationBuilder.DropColumn(
                name: "NamePt",
                table: "AudiovisualResource");
        }
    }
}
