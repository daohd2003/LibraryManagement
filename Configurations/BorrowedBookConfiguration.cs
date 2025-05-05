using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Data
{
    public class BorrowedBookConfiguration : IEntityTypeConfiguration<BorrowedBook>
    {
        public void Configure(EntityTypeBuilder<BorrowedBook> builder)
        {
            builder.ToTable("BorrowedBooks");

            builder.HasKey(bb => bb.Id);

            builder.Property(bb => bb.BorrowDate)
                .IsRequired();

            builder.Property(bb => bb.DueDate)
                .IsRequired();

            builder.Property(bb => bb.ReturnDate)
                .IsRequired();

            builder.Property(bb => bb.Status)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(bb => bb.Book)
                .WithMany(b => b.BorrowedBooks)
                .HasForeignKey(b => b.BookId);

            builder.HasOne(bb => bb.User)
                .WithMany(u => u.BorrowedBooks)
                .HasForeignKey(u => u.UserId);
        }
    }
}
