using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QSR.Domain.Entities.Branches.Kitchen;
using QSR.Domain.Entities.Branches.Orders;

namespace QSR.Infrastructure.Persistence.Branches.Configurations.Kitchen;

public class KitchenTaskItemConfiguration
    : IEntityTypeConfiguration<KitchenTaskItem>
{
    public void Configure(
        EntityTypeBuilder<KitchenTaskItem> builder)
    {
        builder.ToTable("kitchen_task_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.KitchenTaskId)
            .IsRequired();

        builder.Property(x => x.OrderLineId)
            .IsRequired();

        builder.Property(x => x.BranchProductId)
            .IsRequired();

        builder.Property(x => x.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.HasIndex(x => x.KitchenTaskId);

        builder.HasIndex(x => x.OrderLineId);

        builder.HasIndex(x => x.BranchProductId);

        builder.HasOne<OrderLine>()
            .WithMany()
            .HasForeignKey(x => x.OrderLineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}