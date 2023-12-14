using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRIApp.Repository.Mappings;

public class UserResetPasswordCodeMapping  : IEntityTypeConfiguration<UserResetPasswordCode>
{
    public void Configure(EntityTypeBuilder<UserResetPasswordCode> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.CodeHash)
            .IsRequired();
        
        builder.Property(x => x.ExpiresAt)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserResetPasswordCodes)
            .HasForeignKey(x => x.UserId);
    }
}