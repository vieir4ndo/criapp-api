using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class QuestionMapping : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(x => x.Text)
            .IsRequired()
            .HasColumnType("varchar(250)");

        builder.Property(x => x.TextEn)
            .HasColumnType("varchar(250)");

        builder.Property(x => x.TextPt)
            .HasColumnType("varchar(250)");
        
        builder.Property(x => x.Active) 
            .IsRequired();
        
        builder.Property(x => x.Type) 
            .IsRequired();

        builder.Property(x => x.ScoringType)
            .IsRequired();

        builder.Property(x => x.ExcludedFamilyTypes)
            .IsRequired();
        
        builder.HasMany(x => x.Answers)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId);

        builder.HasOne(x => x.Admin)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.AdminId);
        
        builder.HasMany(x => x.Advices)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId);
    }
}