using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class AnswerMapping : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(x => x.Value)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Question)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.QuestionId);

        builder.HasOne(x => x.Child)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.ChildId);

        builder.HasOne(x => x.Prc)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.PrcId);
    }
}