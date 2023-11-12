using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class ForumMapping : IEntityTypeConfiguration<Forum>
{
    public void Configure(EntityTypeBuilder<Forum> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(x => x.Name)
            .IsRequired();
        
        builder.Property(x => x.Description)
            .IsRequired();
        
        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Forum);
        
        builder.HasOne(x => x.Prc)
            .WithMany(x => x.Forums)
            .HasForeignKey(x => x.PrcId);
    }
}