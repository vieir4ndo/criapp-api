using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RenameAdviceToRecommendation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChildBookAdvice_Advice_AdvicesId",
                table: "ChildBookAdvice");

            migrationBuilder.DropForeignKey(
                name: "FK_ChildBookAdvice_ChildBook_ChildBooksId",
                table: "ChildBookAdvice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChildBookAdvice",
                table: "ChildBookAdvice");

            migrationBuilder.DropIndex(
                name: "IX_ChildBookAdvice_ChildBooksId",
                table: "ChildBookAdvice");

            migrationBuilder.DropForeignKey(
                name: "FK_Advice_Question_QuestionId",
                table: "Advice");

            migrationBuilder.DropIndex(
                name: "IX_Advice_QuestionId",
                table: "Advice");

            migrationBuilder.RenameTable(
                name: "Advice",
                newName: "Recommendation");

            migrationBuilder.RenameTable(
                name: "ChildBookAdvice",
                newName: "ChildBookRecommendation");

            migrationBuilder.RenameColumn(
                name: "AdvicesId",
                table: "ChildBookRecommendation",
                newName: "RecommendationsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recommendation",
                table: "Recommendation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendation_Question_QuestionId",
                table: "Recommendation",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex(
                name: "IX_Recommendation_QuestionId",
                table: "Recommendation",
                column: "QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChildBookRecommendation",
                table: "ChildBookRecommendation",
                columns: new[] { "ChildBooksId", "RecommendationsId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChildBookRecommendation_RecommendationsId",
                table: "ChildBookRecommendation",
                column: "RecommendationsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChildBookRecommendation_ChildBook_ChildBooksId",
                table: "ChildBookRecommendation",
                column: "ChildBooksId",
                principalTable: "ChildBook",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChildBookRecommendation_Recommendation_RecommendationsId",
                table: "ChildBookRecommendation",
                column: "RecommendationsId",
                principalTable: "Recommendation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChildBookRecommendation_ChildBook_ChildBooksId",
                table: "ChildBookRecommendation");

            migrationBuilder.DropForeignKey(
                name: "FK_ChildBookRecommendation_Recommendation_RecommendationsId",
                table: "ChildBookRecommendation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChildBookRecommendation",
                table: "ChildBookRecommendation");

            migrationBuilder.DropIndex(
                name: "IX_ChildBookRecommendation_RecommendationsId",
                table: "ChildBookRecommendation");

            migrationBuilder.DropForeignKey(
                name: "FK_Recommendation_Question_QuestionId",
                table: "Recommendation");

            migrationBuilder.DropIndex(
                name: "IX_Recommendation_QuestionId",
                table: "Recommendation");

            migrationBuilder.RenameTable(
                name: "Recommendation",
                newName: "Advice");

            migrationBuilder.RenameTable(
                name: "ChildBookRecommendation",
                newName: "ChildBookAdvice");

            migrationBuilder.RenameColumn(
                name: "RecommendationsId",
                table: "ChildBookAdvice",
                newName: "AdvicesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Advice",
                table: "Advice",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Advice_Question_QuestionId",
                table: "Advice",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex(
                name: "IX_Advice_QuestionId",
                table: "Advice",
                column: "QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChildBookAdvice",
                table: "ChildBookAdvice",
                columns: new[] { "AdvicesId", "ChildBooksId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChildBookAdvice_ChildBooksId",
                table: "ChildBookAdvice",
                column: "ChildBooksId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChildBookAdvice_Advice_AdvicesId",
                table: "ChildBookAdvice",
                column: "AdvicesId",
                principalTable: "Advice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChildBookAdvice_ChildBook_ChildBooksId",
                table: "ChildBookAdvice",
                column: "ChildBooksId",
                principalTable: "ChildBook",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}