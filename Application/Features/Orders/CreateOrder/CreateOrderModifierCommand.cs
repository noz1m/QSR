namespace QSR.Application.Features.Orders.CreateOrder;

public sealed record CreateOrderModifierCommand(
    Guid BranchModifierOptionId);