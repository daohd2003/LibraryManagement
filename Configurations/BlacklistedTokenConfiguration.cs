using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LibraryManagement.Configurations
{
    public class BlacklistedTokenConfiguration : IEntityTypeConfiguration<BlacklistedToken>
    {
        public void Configure(EntityTypeBuilder<BlacklistedToken> builder)
        {
            builder.ToTable("BlacklistedTokens");

            builder.HasKey(bt => bt.Id);

            builder.Property(bt => bt.Token)
                .IsRequired()
                .HasMaxLength(512);

            builder.HasIndex(bt => bt.Token)
                .IsUnique();

            builder.Property(bt => bt.ExpiredAt)
                .IsRequired()
                .HasColumnType("datetime2");
        }
    }
}
