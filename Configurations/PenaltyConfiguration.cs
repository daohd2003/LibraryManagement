using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Configurations
{
    public class PenaltyConfiguration : IEntityTypeConfiguration<Penalty>
    {
        public void Configure(EntityTypeBuilder<Penalty> builder)
        {
            builder.ToTable("Penalties");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.ViolationType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(p => p.BorrowedBook)
                .WithMany(b => b.Penalties)
                .HasForeignKey(p => p.BorrowedBookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
