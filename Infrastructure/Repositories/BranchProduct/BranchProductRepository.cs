using Microsoft.EntityFrameworkCore;
using QSR.Application.Interfaces.Repositories.Menu;
using QSR.Domain.Entities.Branches.Menu;
using QSR.Infrastructure.Persistence.Branches;

namespace QSR.Infrastructure.Repositories.Menu;

public class BranchProductRepository(
    BranchDbContext dbContext)
    : IBranchProductRepository
{
    public async Task<BranchProduct?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.BranchProducts
            .Include(x => x.Variants)
            .Include(x => x.ModifierGroups)
                .ThenInclude(x => x.Options)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<BranchProduct>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var productIds = ids
            .Distinct()
            .ToArray();

        return await dbContext.BranchProducts
            .Include(x => x.Variants)
            .Include(x => x.ModifierGroups)
                .ThenInclude(x => x.Options)
            .Where(x => productIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}