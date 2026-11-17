using QSR.Domain.Enums;

namespace QSR.Application.Features.Orders.CreateOrder;

public sealed record CreateOrderCommand(
    Guid BranchId,
    Guid? CustomerId,
    OrderType Type,
    string Currency,
    IReadOnlyCollection<CreateOrderLineCommand> Lines);