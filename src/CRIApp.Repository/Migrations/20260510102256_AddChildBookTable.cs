using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddChildBookTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Award_User_PrcId",
                table: "Award");

            migrationBuilder.DropTable(
                name: "UserAdvice");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ScoreLastUpdatedAt",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "PrcId",
                table: "Award",
                newName: "ChildBookId");

            migrationBuilder.RenameIndex(
                name: "IX_Award_PrcId",
                table: "Award",
                newName: "IX_Award_ChildBookId");

            migrationBuilder.CreateTable(
                name: "ChildBook",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    ScoreLastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildBook", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChildBook_Child_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Child",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChildBookAdvice",
                columns: table => new
                {
                    ChildBooksId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdvicesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildBookAdvice", x => new { x.AdvicesId, x.ChildBooksId });
                    table.ForeignKey(
                        name: "FK_ChildBookAdvice_Advice_AdvicesId",
                        column: x => x.AdvicesId,
                        principalTable: "Advice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChildBookAdvice_ChildBook_ChildBooksId",
                        column: x => x.ChildBooksId,
                        principalTable: "ChildBook",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChildBook_ChildId",
                table: "ChildBook",
                column: "ChildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChildBookAdvice_ChildBooksId",
                table: "ChildBookAdvice",
                column: "ChildBooksId");

            migrationBuilder.AddForeignKey(
                name: "FK_Award_ChildBook_ChildBookId",
                table: "Award",
                column: "ChildBookId",
                principalTable: "ChildBook",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Award_ChildBook_ChildBookId",
                table: "Award");

            migrationBuilder.DropTable(
                name: "ChildBookAdvice");

            migrationBuilder.DropTable(
                name: "ChildBook");

            migrationBuilder.RenameColumn(
                name: "ChildBookId",
                table: "Award",
                newName: "PrcId");

            migrationBuilder.RenameIndex(
                name: "IX_Award_ChildBookId",
                table: "Award",
                newName: "IX_Award_PrcId");

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScoreLastUpdatedAt",
                table: "User",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "UserAdvice",
                columns: table => new
                {
                    AdvicesId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAdvice", x => new { x.AdvicesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_UserAdvice_Advice_AdvicesId",
                        column: x => x.AdvicesId,
                        principalTable: "Advice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAdvice_User_UsersId",
                        column: x => x.UsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAdvice_UsersId",
                table: "UserAdvice",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Award_User_PrcId",
                table: "Award",
                column: "PrcId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
