using Microsoft.EntityFrameworkCore;
using QSR.Application.Interfaces.Repositories.Inventory;
using QSR.Domain.Entities.Branches.Inventory;
using QSR.Infrastructure.Persistence.Branches;

namespace QSR.Infrastructure.Repositories.Inventory;

public class IngredientRepository(
    BranchDbContext dbContext)
    : IIngredientRepository
{
    public async Task<Ingredient?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Ingredients
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Ingredient>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var ingredientIds = ids
            .Distinct()
            .ToArray();

        return await dbContext.Ingredients
            .Where(x => ingredientIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Ingredient>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Ingredients
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Ingredient>> GetLowStockAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Ingredients
            .Where(x =>
                x.IsActive &&
                x.CurrentStock <= x.MinimumStock)
            .OrderBy(x => x.CurrentStock)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Ingredient ingredient,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ingredient);

        await dbContext.Ingredients.AddAsync(
            ingredient,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}