using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class AdminMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.BirthDate)
            .IsRequired();

        builder.Property(x => x.Sex)
            .IsRequired();

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.Active)
            .IsRequired();

        builder.Property(x => x.NumberOfChildren)
            .IsRequired(false);

        builder.Property(x => x.FamilyType)
            .IsRequired(false);

        builder.Property(x => x.EducationalLevel)
            .IsRequired(false);

        builder.Property(x => x.IsAdmin)
            .IsRequired();
        
        builder.Property(x=> x.Score)
            .IsRequired();
    }
}