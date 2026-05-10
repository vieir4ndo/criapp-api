using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class AwardMapping : IEntityTypeConfiguration<Award>
{
    public void Configure(EntityTypeBuilder<Award> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Text)
            .IsRequired()
            .HasColumnType("varchar(500)");

        builder.Property(x => x.TextEn)
            .HasColumnType("varchar(500)");

        builder.Property(x => x.TextPt)
            .HasColumnType("varchar(500)");

        builder.Property(x => x.Description)
            .HasColumnType("varchar(1000)");

        builder.Property(x => x.DescriptionEn)
            .HasColumnType("varchar(1000)");

        builder.Property(x => x.DescriptionPt)
            .HasColumnType("varchar(1000)");

        builder.Property(x => x.Image);

        builder.HasMany(x => x.ChildBooks)
            .WithMany(x => x.Awards)
            .UsingEntity<ChildBookAward>(
                l => l.HasOne(cb => cb.ChildBook).WithMany().HasForeignKey(cb => cb.ChildBooksId),
                r => r.HasOne(cb => cb.Award).WithMany().HasForeignKey(cb => cb.AwardsId),
                j =>
                {
                    j.ToTable("ChildBookAward");
                    j.Property(cb => cb.AwardsId).HasColumnName("AwardsId");
                });
    }
}