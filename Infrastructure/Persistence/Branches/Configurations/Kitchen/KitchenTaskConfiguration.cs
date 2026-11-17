using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QSR.Domain.Entities.Branches.Kitchen;
using QSR.Domain.Entities.Branches.Orders;

namespace QSR.Infrastructure.Persistence.Branches.Configurations.Kitchen;

public class KitchenTaskConfiguration
    : IEntityTypeConfiguration<KitchenTask>
{
    public void Configure(
        EntityTypeBuilder<KitchenTask> builder)
    {
        builder.ToTable("kitchen_tasks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BranchId)
            .IsRequired();

        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.Property(x => x.KitchenStationId)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.AcceptedAt);

        builder.Property(x => x.StartedAt);

        builder.Property(x => x.ReadyAt);

        builder.Property(x => x.HandedOffAt);

        builder.Property(x => x.CancelledAt);

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => x.BranchId);

        builder.HasIndex(x => x.OrderId);

        builder.HasIndex(x => x.KitchenStationId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => new
        {
            x.KitchenStationId,
            x.Status
        });

        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<KitchenStation>()
            .WithMany()
            .HasForeignKey(x => x.KitchenStationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.KitchenTaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}