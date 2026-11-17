using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QSR.Domain.Entities.Branches.Kitchen;

namespace QSR.Infrastructure.Persistence.Branches.Configurations.Kitchen;

public class KitchenStationConfiguration
    : IEntityTypeConfiguration<KitchenStation>
{
    public void Configure(
        EntityTypeBuilder<KitchenStation> builder)
    {
        builder.ToTable("kitchen_stations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BranchId)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => new
            {
                x.BranchId,
                x.Code
            })
            .IsUnique();

        builder.HasIndex(x => x.BranchId);

        builder.HasIndex(x => x.IsActive);
    }
}