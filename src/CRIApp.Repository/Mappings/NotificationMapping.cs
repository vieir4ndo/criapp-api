using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class NotificationMapping : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notification");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ChildBookId, x.Type, x.Date })
            .IsUnique();

        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.Delivered).IsRequired();
        builder.Property(x => x.Read).IsRequired();

        builder.HasOne(x => x.ChildBook)
            .WithMany()
            .HasForeignKey(x => x.ChildBookId);
    }
}
