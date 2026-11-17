using Microsoft.EntityFrameworkCore;
using QSR.Application.Interfaces.Repositories.Recipes;
using QSR.Domain.Entities.Branches.Recipes;
using QSR.Infrastructure.Persistence.Branches;

namespace QSR.Infrastructure.Repositories.Recipes;

public class BranchRecipeRepository(
    BranchDbContext dbContext)
    : IBranchRecipeRepository
{
    public Task<BranchRecipe?> GetVersionAsync(Guid productId, int version,
        CancellationToken cancellationToken = default)
    {
        return dbContext.BranchRecipes.Include(x => x.Ingredients)
            .SingleOrDefaultAsync(x => x.BranchProductId == productId &&
                x.Version == version, cancellationToken);
    }

    public async Task AddAsync(BranchRecipe recipe, CancellationToken cancellationToken = default)
    {
        await dbContext.BranchRecipes.AddAsync(recipe, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<BranchRecipe?> GetByBranchProductIdAsync(
        Guid branchProductId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        return await dbContext.BranchRecipes
            .Include(x => x.Ingredients)
            .Where(x => x.IsActive && x.EffectiveFrom <= now &&
                (x.EffectiveTo == null || x.EffectiveTo > now))
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync(
                x => x.BranchProductId == branchProductId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<BranchRecipe>> GetByBranchProductIdsAsync(
        IEnumerable<Guid> branchProductIds,
        CancellationToken cancellationToken = default)
    {
        var productIds = branchProductIds
            .Distinct()
            .ToArray();

        var now = DateTimeOffset.UtcNow;

        var recipes = await dbContext.BranchRecipes
            .Include(x => x.Ingredients)
            .Where(x => x.IsActive && x.EffectiveFrom <= now &&
                (x.EffectiveTo == null || x.EffectiveTo > now))
            .OrderByDescending(x => x.Version)
            .Where(x => productIds.Contains(x.BranchProductId))
            .ToListAsync(cancellationToken);

        return recipes.DistinctBy(x => x.BranchProductId).ToArray();
    }
}