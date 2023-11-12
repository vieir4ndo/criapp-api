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
        
        builder.HasOne(x => x.Prc)
            .WithMany(x => x.Awards)
            .HasForeignKey(x => x.PrcId);
    }
}