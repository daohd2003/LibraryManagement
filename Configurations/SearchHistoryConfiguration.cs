using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Configurations
{
    public class SearchHistoryConfiguration : IEntityTypeConfiguration<SearchHistory>
    {
        public void Configure(EntityTypeBuilder<SearchHistory> builder)
        {
            builder.ToTable("SearchHistories");

            builder.HasKey(sh => sh.Id);

            builder.Property(sh => sh.Query)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(sh => sh.SearchedAt)
                   .IsRequired();

            builder.Property(sh => sh.ClientIdentifier);

            builder.HasOne(sh => sh.User)
                   .WithMany(u => u.SearchHistories)
                   .HasForeignKey(sh => sh.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}