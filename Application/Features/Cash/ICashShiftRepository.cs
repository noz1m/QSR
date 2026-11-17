using QSR.Domain.Entities.Branches.Cash;

namespace QSR.Application.Features.Cash;

public interface ICashShiftRepository
{
    Task<CashShift?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<CashShift?> GetOpenAsync(Guid branchId, string registerCode, CancellationToken cancellationToken);
    Task AddAsync(CashShift shift, CancellationToken cancellationToken);
    Task SaveAsync(CancellationToken cancellationToken);
}
