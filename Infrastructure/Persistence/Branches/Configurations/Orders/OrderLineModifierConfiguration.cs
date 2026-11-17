using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QSR.Domain.Entities.Branches.Orders;

namespace QSR.Infrastructure.Persistence.Branches.Configurations.Orders;

public class OrderLineModifierConfiguration
    : IEntityTypeConfiguration<OrderLineModifier>
{
    public void Configure(
        EntityTypeBuilder<OrderLineModifier> builder)
    {
        builder.ToTable("order_line_modifiers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderLineId)
            .IsRequired();

        builder.Property(x => x.BranchModifierOptionId)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.ComplexProperty(
            x => x.PriceAdjustment,
            money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("price_adjustment")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

        builder.HasIndex(x => x.OrderLineId);

        builder.HasIndex(x => x.BranchModifierOptionId);
    }
}