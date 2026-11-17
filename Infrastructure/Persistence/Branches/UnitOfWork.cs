using Microsoft.EntityFrameworkCore;
using QSR.Application.Interfaces.Persistence;

namespace QSR.Infrastructure.Persistence.Branches;

public class UnitOfWork(
    BranchDbContext dbContext)
    : IUnitOfWork
{
    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable, cancellationToken);

        await action(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
