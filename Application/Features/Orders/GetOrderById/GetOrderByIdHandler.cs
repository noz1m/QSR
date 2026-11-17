using QSR.Application.Interfaces.Repositories.Orders;

namespace QSR.Application.Features.Orders.GetOrderById;

public class GetOrderByIdHandler(
    IOrderRepository orderRepository)
{
    public async Task<GetOrderByIdResult?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        var order = await orderRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (order is null)
            return null;

        var lines = order.Lines
            .Select(line =>
                new GetOrderLineResult(
                    line.Id,
                    line.BranchProductId,
                    line.BranchProductVariantId,
                    line.ProductName,
                    line.VariantName,
                    line.Quantity,
                    line.UnitPrice.Amount,
                    line.TotalPrice.Amount,
                    line.TotalPrice.Currency,
                    line.Modifiers
                        .Select(modifier =>
                            new GetOrderModifierResult(
                                modifier.Id,
                                modifier.BranchModifierOptionId,
                                modifier.Name,
                                modifier.PriceAdjustment.Amount,
                                modifier.PriceAdjustment.Currency))
                        .ToArray()))
            .ToArray();

        return new GetOrderByIdResult(
            order.Id,
            order.BranchId,
            order.CustomerId,
            order.OrderNumber,
            order.Type,
            order.Status,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency,
            order.CreatedAt,
            lines);
    }
}