using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class AdviceMapping: IEntityTypeConfiguration<Advice>
{
    public void Configure(EntityTypeBuilder<Advice> builder)
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

        builder.HasOne(x => x.Admin)
            .WithMany(x => x.AdminAdvices)
            .HasForeignKey(x => x.AdminId);

        builder.HasMany(x => x.Users)
            .WithMany(x => x.Advices)
            .UsingEntity<UserAdvice>(
                l => l.HasOne(ur => ur.User).WithMany().HasForeignKey(ur => ur.UsersId),
                r => r.HasOne(ur => ur.Advice).WithMany().HasForeignKey(ur => ur.AdvicesId),
                j =>
                {
                    j.ToTable("UserRecommendation");
                    j.Property(ur => ur.AdvicesId).HasColumnName("RecommendationsId");
                });

        builder.Property(x => x.Id)
            .HasColumnName("Id");
    }
}