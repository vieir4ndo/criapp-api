using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveChildBookIdFromRecommendationCheckpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecommendationCheckpoint_ChildBook_ChildBookId",
                table: "RecommendationCheckpoint");

            migrationBuilder.DropIndex(
                name: "IX_RecommendationCheckpoint_ChildBookId",
                table: "RecommendationCheckpoint");

            migrationBuilder.DropColumn(
                name: "ChildBookId",
                table: "RecommendationCheckpoint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChildBookId",
                table: "RecommendationCheckpoint",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationCheckpoint_ChildBookId",
                table: "RecommendationCheckpoint",
                column: "ChildBookId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecommendationCheckpoint_ChildBook_ChildBookId",
                table: "RecommendationCheckpoint",
                column: "ChildBookId",
                principalTable: "ChildBook",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
