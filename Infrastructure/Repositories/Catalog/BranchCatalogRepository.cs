using Microsoft.EntityFrameworkCore;
using QSR.Application.Features.Catalog;
using QSR.Domain.Entities.Branches.Kitchen;
using QSR.Domain.Entities.Branches.Menu;
using QSR.Infrastructure.Persistence.Branches;

namespace QSR.Infrastructure.Repositories.Catalog;

public class BranchCatalogRepository(BranchDbContext dbContext) : IBranchCatalogRepository
{
    public Task<BranchMenu?> GetMenuAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Set<BranchMenu>().Include(x => x.Categories)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<BranchCategory?> GetCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Set<BranchCategory>().Include(x => x.Products)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<KitchenStation?> GetStationAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.KitchenStations.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<BranchMenu>> GetMenusAsync(
        Guid branchId, CancellationToken cancellationToken)
    {
        return await dbContext.Set<BranchMenu>().AsNoTracking()
            .Include(x => x.Categories).ThenInclude(x => x.Products)
            .Where(x => x.BranchId == branchId).OrderBy(x => x.Name)
            .AsSplitQuery().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<KitchenStation>> GetStationsAsync(
        Guid branchId, CancellationToken cancellationToken)
    {
        return await dbContext.KitchenStations.AsNoTracking()
            .Where(x => x.BranchId == branchId).OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task AddMenuAsync(BranchMenu menu, CancellationToken cancellationToken)
    {
        await dbContext.Set<BranchMenu>().AddAsync(menu, cancellationToken);
    }

    public async Task AddStationAsync(KitchenStation station, CancellationToken cancellationToken)
    {
        await dbContext.KitchenStations.AddAsync(station, cancellationToken);
    }

    public async Task AddCategoryAsync(BranchCategory category, CancellationToken cancellationToken)
    {
        await dbContext.Set<BranchCategory>().AddAsync(category, cancellationToken);
    }

    public async Task AddProductAsync(BranchProduct product, CancellationToken cancellationToken)
    {
        await dbContext.BranchProducts.AddAsync(product, cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
