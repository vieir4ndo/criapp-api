using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class CommentMapping : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(x => x.Text)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow);

        builder.HasOne(x => x.Forum)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.ForumId);

        builder.HasOne(x => x.Prc)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.PrcId);
    }
}