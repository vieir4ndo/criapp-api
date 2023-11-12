using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class FaqMapping : IEntityTypeConfiguration<Faq>
{
    public void Configure(EntityTypeBuilder<Faq> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(x => x.Question)
            .IsRequired();
        
        builder.Property(x => x.Answer)
            .IsRequired();
        
        builder.HasOne(x => x.Admin)
            .WithMany(x => x.Faqs)
            .HasForeignKey(x => x.AdminId);
    }
}