using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class AudiovisualResourceMapping : IEntityTypeConfiguration<AudiovisualResource>

{
    public void Configure(EntityTypeBuilder<AudiovisualResource> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(x => x.Name)
            .IsRequired();
        
        builder.Property(x => x.Description)
            .IsRequired();
        
        builder.Property(x => x.Link)
            .IsRequired();
        
        builder.HasOne(x => x.Admin)
            .WithMany(x => x.AudiovisualResources)
            .HasForeignKey(x => x.AdminId);
    }
}