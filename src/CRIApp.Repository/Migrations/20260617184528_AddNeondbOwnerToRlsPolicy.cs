using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddNeondbOwnerToRlsPolicy : Migration
    {
        private static readonly string[] Tables =
        [
            "__EFMigrationsHistory",
            "Answer",
            "AudiovisualResource",
            "Award",
            "Child",
            "ChildBook",
            "ChildBookAward",
            "ChildBookRecommendation",
            "Comment",
            "Faq",
            "Forum",
            "Notification",
            "Question",
            "Recommendation",
            "RecommendationCheckpoint",
            "RefreshToken",
            "User",
            "UserResetPasswordCode"
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in Tables)
            {
                migrationBuilder.Sql($"""
                    ALTER POLICY "allow_app_user" ON "{table}"
                        TO postgres, neondb_owner;
                    """);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var table in Tables)
            {
                migrationBuilder.Sql($"""
                    ALTER POLICY "allow_app_user" ON "{table}"
                        TO postgres;
                    """);
            }
        }
    }
}
