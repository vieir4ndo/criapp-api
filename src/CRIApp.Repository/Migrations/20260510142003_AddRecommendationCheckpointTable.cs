using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddRecommendationCheckpointTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecommendationCheckpoint",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecommendationId = table.Column<int>(type: "integer", nullable: false),
                    ChildBookId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendationCheckpoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecommendationCheckpoint_ChildBook_ChildBookId",
                        column: x => x.ChildBookId,
                        principalTable: "ChildBook",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecommendationCheckpoint_Recommendation_RecommendationId",
                        column: x => x.RecommendationId,
                        principalTable: "Recommendation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationCheckpoint_ChildBookId",
                table: "RecommendationCheckpoint",
                column: "ChildBookId");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationCheckpoint_RecommendationId_ChildBookId_Date",
                table: "RecommendationCheckpoint",
                columns: new[] { "RecommendationId", "ChildBookId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecommendationCheckpoint");
        }
    }
}
