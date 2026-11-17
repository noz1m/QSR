namespace QSR.Application.Features.Orders.CreateOrder;

public sealed record CreateOrderLineCommand(
    Guid BranchProductId,
    Guid? BranchProductVariantId,
    int Quantity,
    IReadOnlyCollection<CreateOrderModifierCommand> Modifiers);