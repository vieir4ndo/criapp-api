using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class ChildMapping : IEntityTypeConfiguration<Child>
{
    public void Configure(EntityTypeBuilder<Child> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.BirthDate)
            .IsRequired();

        builder.Property(x => x.Sex)
            .IsRequired();
        
        builder.Property(x => x.MonthsOfBreastFeeding)
            .IsRequired();
        
        builder.HasOne(x => x.Prc)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.PrcId);
        
        builder.HasMany(x => x.Answers)
            .WithOne(x => x.Child)
            .HasForeignKey(x => x.ChildId);
        
        builder.Property(x => x.IsPremature)
            .IsRequired();
    }
}