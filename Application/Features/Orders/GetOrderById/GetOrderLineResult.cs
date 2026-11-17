namespace QSR.Application.Features.Orders.GetOrderById;

public sealed record GetOrderLineResult(
    Guid Id,
    Guid BranchProductId,
    Guid? BranchProductVariantId,
    string ProductName,
    string? VariantName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    string Currency,
    IReadOnlyCollection<GetOrderModifierResult> Modifiers);