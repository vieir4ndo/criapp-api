using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    public partial class AddRecommendationsToUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecommendationUser",
                columns: table => new
                {
                    RecommendationsId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendationUser", x => new { x.RecommendationsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RecommendationUser_Recommendation_RecommendationsId",
                        column: x => x.RecommendationsId,
                        principalTable: "Recommendation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecommendationUser_User_UsersId",
                        column: x => x.UsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationUser_UsersId",
                table: "RecommendationUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecommendationUser");
        }
    }
}
