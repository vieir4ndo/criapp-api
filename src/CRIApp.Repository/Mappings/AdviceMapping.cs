using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class AdviceMapping: IEntityTypeConfiguration<Advice>
{
    public void Configure(EntityTypeBuilder<Advice> builder)
    {
        builder.ToTable("Advice");

        builder.Property(p => p.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.Text)
            .IsRequired()
            .HasColumnType("varchar(500)");

        builder.Property(x => x.TextEn)
            .HasColumnType("varchar(500)");

        builder.Property(x => x.TextPt)
            .HasColumnType("varchar(500)");

        builder.HasMany(x => x.ChildBooks)
            .WithMany(x => x.Advices)
            .UsingEntity<ChildBookAdvice>(
                l => l.HasOne(cb => cb.ChildBook).WithMany().HasForeignKey(cb => cb.ChildBooksId),
                r => r.HasOne(cb => cb.Advice).WithMany().HasForeignKey(cb => cb.AdvicesId),
                j =>
                {
                    j.ToTable("ChildBookAdvice");
                    j.Property(cb => cb.AdvicesId).HasColumnName("AdvicesId");
                });

        builder.Property(x => x.Id)
            .HasColumnName("Id");
    }
}
