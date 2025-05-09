using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Data
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(128);

            builder.HasMany(u => u.BorrowedBooks)
                .WithOne(bb => bb.User)
                .HasForeignKey(bb => bb.UserId);

            builder.Property(u => u.RefreshToken)
            .HasMaxLength(200);

            builder.Property(u => u.RefreshTokenExpiryTime);
        }
    }
}
