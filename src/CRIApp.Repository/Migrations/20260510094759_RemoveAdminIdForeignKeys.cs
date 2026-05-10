using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAdminIdForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advice_User_AdminId",
                table: "Advice");

            migrationBuilder.DropForeignKey(
                name: "FK_AudiovisualResource_User_AdminId",
                table: "AudiovisualResource");

            migrationBuilder.DropForeignKey(
                name: "FK_Faq_User_AdminId",
                table: "Faq");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_User_AdminId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_AdminId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Faq_AdminId",
                table: "Faq");

            migrationBuilder.DropIndex(
                name: "IX_AudiovisualResource_AdminId",
                table: "AudiovisualResource");

            migrationBuilder.DropIndex(
                name: "IX_Advice_AdminId",
                table: "Advice");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Faq");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "AudiovisualResource");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Advice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdminId",
                table: "Question",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AdminId",
                table: "Faq",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AdminId",
                table: "AudiovisualResource",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AdminId",
                table: "Advice",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Question_AdminId",
                table: "Question",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Faq_AdminId",
                table: "Faq",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_AudiovisualResource_AdminId",
                table: "AudiovisualResource",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Advice_AdminId",
                table: "Advice",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advice_User_AdminId",
                table: "Advice",
                column: "AdminId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AudiovisualResource_User_AdminId",
                table: "AudiovisualResource",
                column: "AdminId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Faq_User_AdminId",
                table: "Faq",
                column: "AdminId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_User_AdminId",
                table: "Question",
                column: "AdminId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
