using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("LibraryTransactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.UserId).IsRequired();

            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(t => t.PaymentMethod)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.Status)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(t => t.TransactionCode)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.HasOne(t => t.User)
                   .WithMany() // hoặc WithMany(u => u.Transactions) nếu có navigation ở User
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}