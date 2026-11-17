using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QSR.Domain.Entities.Branches.Cash;

namespace QSR.Infrastructure.Persistence.Branches.Configurations.Cash;

public class CashShiftConfiguration : IEntityTypeConfiguration<CashShift>
{
    public void Configure(EntityTypeBuilder<CashShift> builder)
    {
        builder.ToTable("cash_shifts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RegisterCode).HasMaxLength(50);
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.OpenedBy).HasMaxLength(200);
        builder.Property(x => x.ClosedBy).HasMaxLength(200);
        builder.Property(x => x.OpeningAmount).HasPrecision(18, 2);
        builder.Property(x => x.ExpectedAmount).HasPrecision(18, 2);
        builder.Property(x => x.CountedAmount).HasPrecision(18, 2);
        builder.Property<uint>("Version").IsRowVersion();
        builder.Ignore(x => x.Difference);
        builder.HasIndex(x => new { x.BranchId, x.RegisterCode })
            .IsUnique().HasFilter("\"ClosedAt\" IS NULL");
        builder.HasMany(x => x.Entries).WithOne().HasForeignKey(x => x.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CashEntryConfiguration : IEntityTypeConfiguration<CashEntry>
{
    public void Configure(EntityTypeBuilder<CashEntry> builder)
    {
        builder.ToTable("cash_entries");
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Actor).HasMaxLength(200);
        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.HasIndex(x => new { x.ShiftId, x.RequestKey }).IsUnique();
        builder.HasIndex(x => new { x.PaymentId, x.Type }).IsUnique();
    }
}
