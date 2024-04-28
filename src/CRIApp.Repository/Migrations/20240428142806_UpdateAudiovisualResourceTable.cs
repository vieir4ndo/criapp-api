using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    public partial class UpdateAudiovisualResourceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Link",
                table: "AudiovisualResource",
                newName: "Iframe");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Iframe",
                table: "AudiovisualResource",
                newName: "Link");
        }
    }
}
