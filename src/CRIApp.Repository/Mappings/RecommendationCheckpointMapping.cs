using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class RecommendationCheckpointMapping : IEntityTypeConfiguration<RecommendationCheckpoint>
{
    public void Configure(EntityTypeBuilder<RecommendationCheckpoint> builder)
    {
        builder.ToTable("RecommendationCheckpoint");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ChildBookRecommendationId, x.Date })
            .IsUnique();

        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.Completed).IsRequired();
    }
}
