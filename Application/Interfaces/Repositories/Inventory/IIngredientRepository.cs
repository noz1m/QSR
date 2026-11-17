using QSR.Domain.Entities.Branches.Inventory;

namespace QSR.Application.Interfaces.Repositories.Inventory;

public interface IIngredientRepository
{
    Task<Ingredient?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Ingredient>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Ingredient>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Ingredient>> GetLowStockAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Ingredient ingredient,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}