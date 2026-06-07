using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class RecommendationMapping: IEntityTypeConfiguration<Recommendation>
{
    public void Configure(EntityTypeBuilder<Recommendation> builder)
    {
        builder.ToTable("Recommendation");

        builder.Property(p => p.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.Text)
            .IsRequired()
            .HasColumnType("varchar(500)");

        builder.Property(x => x.TextEn)
            .HasColumnType("varchar(500)");

        builder.Property(x => x.TextPt)
            .HasColumnType("varchar(500)");

        builder.Property(x => x.Image);

        builder.HasMany(x => x.ChildBooks)
            .WithMany(x => x.Recommendations)
            .UsingEntity<ChildBookRecommendation>(
                l => l.HasOne(cb => cb.ChildBook).WithMany().HasForeignKey(cb => cb.ChildBooksId),
                r => r.HasOne(cb => cb.Recommendation).WithMany().HasForeignKey(cb => cb.RecommendationsId),
                j =>
                {
                    j.ToTable("ChildBookRecommendation");
                    j.Property(cb => cb.RecommendationsId).HasColumnName("RecommendationsId");
                    j.Property(cb => cb.CreatedAt).IsRequired().HasDefaultValueSql("NOW()");
                });
    }
}
