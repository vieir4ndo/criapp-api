using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RenameRecommendationsIdToRecommendationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChildBookRecommendation_Recommendation_RecommendationsId",
                table: "ChildBookRecommendation");

            migrationBuilder.RenameColumn(
                name: "RecommendationsId",
                table: "ChildBookRecommendation",
                newName: "RecommendationId");

            migrationBuilder.RenameIndex(
                name: "IX_ChildBookRecommendation_RecommendationsId",
                table: "ChildBookRecommendation",
                newName: "IX_ChildBookRecommendation_RecommendationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChildBookRecommendation_Recommendation_RecommendationId",
                table: "ChildBookRecommendation",
                column: "RecommendationId",
                principalTable: "Recommendation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChildBookRecommendation_Recommendation_RecommendationId",
                table: "ChildBookRecommendation");

            migrationBuilder.RenameColumn(
                name: "RecommendationId",
                table: "ChildBookRecommendation",
                newName: "RecommendationsId");

            migrationBuilder.RenameIndex(
                name: "IX_ChildBookRecommendation_RecommendationId",
                table: "ChildBookRecommendation",
                newName: "IX_ChildBookRecommendation_RecommendationsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChildBookRecommendation_Recommendation_RecommendationsId",
                table: "ChildBookRecommendation",
                column: "RecommendationsId",
                principalTable: "Recommendation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
