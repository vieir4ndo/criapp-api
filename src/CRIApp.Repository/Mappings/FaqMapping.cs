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

        builder.Property(x => x.QuestionEn);

        builder.Property(x => x.AnswerEn);

        builder.Property(x => x.QuestionPt);

        builder.Property(x => x.AnswerPt);
        
        builder.Property(x => x.Answer)
            .IsRequired();
    }
}