using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAwardToManyToManyWithImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Award_ChildBook_ChildBookId",
                table: "Award");

            migrationBuilder.DropIndex(
                name: "IX_Award_ChildBookId",
                table: "Award");

            migrationBuilder.DropColumn(
                name: "ChildBookId",
                table: "Award");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Award",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Award",
                type: "varchar(500)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TextEn",
                table: "Award",
                type: "varchar(500)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextPt",
                table: "Award",
                type: "varchar(500)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Advice",
                type: "bytea",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChildBookAward",
                columns: table => new
                {
                    ChildBooksId = table.Column<Guid>(type: "uuid", nullable: false),
                    AwardsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildBookAward", x => new { x.AwardsId, x.ChildBooksId });
                    table.ForeignKey(
                        name: "FK_ChildBookAward_Award_AwardsId",
                        column: x => x.AwardsId,
                        principalTable: "Award",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChildBookAward_ChildBook_ChildBooksId",
                        column: x => x.ChildBooksId,
                        principalTable: "ChildBook",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChildBookAward_ChildBooksId",
                table: "ChildBookAward",
                column: "ChildBooksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildBookAward");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Award");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Award");

            migrationBuilder.DropColumn(
                name: "TextEn",
                table: "Award");

            migrationBuilder.DropColumn(
                name: "TextPt",
                table: "Award");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Advice");

            migrationBuilder.AddColumn<Guid>(
                name: "ChildBookId",
                table: "Award",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Award_ChildBookId",
                table: "Award",
                column: "ChildBookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Award_ChildBook_ChildBookId",
                table: "Award",
                column: "ChildBookId",
                principalTable: "ChildBook",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
