using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class ChildBookMapping : IEntityTypeConfiguration<ChildBook>
{
    public void Configure(EntityTypeBuilder<ChildBook> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Score)
            .IsRequired();

        builder.Property(x => x.ScoreLastUpdatedAt)
            .IsRequired();

        builder.HasOne(x => x.Child)
            .WithOne(x => x.ChildBook)
            .HasForeignKey<ChildBook>(x => x.ChildId);
    }
}
