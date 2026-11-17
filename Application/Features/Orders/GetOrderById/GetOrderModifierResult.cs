namespace QSR.Application.Features.Orders.GetOrderById;

public sealed record GetOrderModifierResult(
    Guid Id,
    Guid BranchModifierOptionId,
    string Name,
    decimal PriceAdjustment,
    string Currency);