using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddChildBookRecommendationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecommendationCheckpoint_Recommendation_RecommendationId",
                table: "RecommendationCheckpoint");

            migrationBuilder.DropIndex(
                name: "IX_RecommendationCheckpoint_RecommendationId_ChildBookId_Date",
                table: "RecommendationCheckpoint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChildBookRecommendation",
                table: "ChildBookRecommendation");

            migrationBuilder.DropColumn(
                name: "RecommendationId",
                table: "RecommendationCheckpoint");

            migrationBuilder.AddColumn<Guid>(
                name: "ChildBookRecommendationId",
                table: "RecommendationCheckpoint",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ChildBookRecommendation",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChildBookRecommendation",
                table: "ChildBookRecommendation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationCheckpoint_ChildBookRecommendationId_Date",
                table: "RecommendationCheckpoint",
                columns: new[] { "ChildBookRecommendationId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChildBookRecommendation_ChildBooksId",
                table: "ChildBookRecommendation",
                column: "ChildBooksId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecommendationCheckpoint_ChildBookRecommendation_ChildBookR~",
                table: "RecommendationCheckpoint",
                column: "ChildBookRecommendationId",
                principalTable: "ChildBookRecommendation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecommendationCheckpoint_ChildBookRecommendation_ChildBookR~",
                table: "RecommendationCheckpoint");

            migrationBuilder.DropIndex(
                name: "IX_RecommendationCheckpoint_ChildBookRecommendationId_Date",
                table: "RecommendationCheckpoint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChildBookRecommendation",
                table: "ChildBookRecommendation");

            migrationBuilder.DropIndex(
                name: "IX_ChildBookRecommendation_ChildBooksId",
                table: "ChildBookRecommendation");

            migrationBuilder.DropColumn(
                name: "ChildBookRecommendationId",
                table: "RecommendationCheckpoint");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ChildBookRecommendation");

            migrationBuilder.AddColumn<int>(
                name: "RecommendationId",
                table: "RecommendationCheckpoint",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChildBookRecommendation",
                table: "ChildBookRecommendation",
                columns: new[] { "ChildBooksId", "RecommendationsId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationCheckpoint_RecommendationId_ChildBookId_Date",
                table: "RecommendationCheckpoint",
                columns: new[] { "RecommendationId", "ChildBookId", "Date" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RecommendationCheckpoint_Recommendation_RecommendationId",
                table: "RecommendationCheckpoint",
                column: "RecommendationId",
                principalTable: "Recommendation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
