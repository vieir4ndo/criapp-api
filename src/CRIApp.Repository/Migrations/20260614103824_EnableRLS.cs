using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRIApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class EnableRLS : Migration
    {
        /// <inheritdoc />
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

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in Tables)
            {
                migrationBuilder.Sql($"""
                    ALTER TABLE "{table}" ENABLE ROW LEVEL SECURITY;
                    ALTER TABLE "{table}" FORCE ROW LEVEL SECURITY;
                    CREATE POLICY "allow_app_user" ON "{table}"
                        TO postgres
                        USING (true)
                        WITH CHECK (true);
                    """);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var table in Tables)
            {
                migrationBuilder.Sql($"""
                    DROP POLICY IF EXISTS "allow_app_user" ON "{table}";
                    ALTER TABLE "{table}" NO FORCE ROW LEVEL SECURITY;
                    ALTER TABLE "{table}" DISABLE ROW LEVEL SECURITY;
                    """);
            }
        }
    }
}
