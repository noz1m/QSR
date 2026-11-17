using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QSR.Domain.Entities.Branches.Menu;

namespace QSR.Infrastructure.Persistence.Branches.Configurations.Menu;

public class BranchMenuConfiguration : IEntityTypeConfiguration<BranchMenu>
{
    public void Configure(EntityTypeBuilder<BranchMenu> builder)
    {
        builder.ToTable("branch_menus");
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property<uint>("Version").IsRowVersion();
        builder.HasMany(x => x.Categories).WithOne().HasForeignKey(x => x.BranchMenuId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BranchCategoryConfiguration : IEntityTypeConfiguration<BranchCategory>
{
    public void Configure(EntityTypeBuilder<BranchCategory> builder)
    {
        builder.ToTable("branch_categories");
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property<uint>("Version").IsRowVersion();
        builder.HasMany(x => x.Products).WithOne().HasForeignKey(x => x.BranchCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
