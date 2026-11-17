using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QSR.Domain.Entities.Branches.Orders;

namespace QSR.Infrastructure.Persistence.Branches.Configurations.Orders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BranchId)
            .IsRequired();

        builder.Property(x => x.CustomerId);
        builder.Property(x => x.CreationKey).HasMaxLength(32);
        builder.Property(x => x.CreationFingerprint).HasMaxLength(64);
        builder.HasIndex(x => new { x.BranchId, x.CreationKey }).IsUnique();

        builder.Property(x => x.OrderNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.ComplexProperty(
            x => x.TotalAmount,
            money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("total_amount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();

        builder.HasIndex(x => x.BranchId);

        builder.HasIndex(x => x.CustomerId);

        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}