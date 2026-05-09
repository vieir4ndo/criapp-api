using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class RecommendationMapping: IEntityTypeConfiguration<Recommendation>
{
    public void Configure(EntityTypeBuilder<Recommendation> builder)
    {
        builder.Property(p => p.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.Text)
            .IsRequired()
            .HasColumnType("varchar(500)");

        builder.Property(x => x.TextEn)
            .HasColumnType("varchar(500)");

        builder.Property(x => x.TextPt)
            .HasColumnType("varchar(500)");

        builder.HasOne(x => x.Admin)
            .WithMany(x => x.AdminRecommendations)
            .HasForeignKey(x => x.AdminId);

        builder.HasMany(x => x.Users)
            .WithMany(x => x.Recommendations)
            .UsingEntity<Dictionary<int, object>>();
    }
}