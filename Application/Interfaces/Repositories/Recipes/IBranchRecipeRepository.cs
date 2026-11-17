using QSR.Domain.Entities.Branches.Recipes;

namespace QSR.Application.Interfaces.Repositories.Recipes;

public interface IBranchRecipeRepository
{
    Task<BranchRecipe?> GetVersionAsync(Guid productId, int version,
        CancellationToken cancellationToken = default);

    Task AddAsync(BranchRecipe recipe, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<BranchRecipe?> GetByBranchProductIdAsync(
        Guid branchProductId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BranchRecipe>> GetByBranchProductIdsAsync(
        IEnumerable<Guid> branchProductIds,
        CancellationToken cancellationToken = default);
}