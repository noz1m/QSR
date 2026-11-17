using QSR.Application.Interfaces.Repositories.Payments;

namespace QSR.Application.Features.Payments.GetPaymentsByOrder;

public class GetPaymentsByOrderHandler(
    IPaymentRepository paymentRepository)
{
    public async Task<IReadOnlyCollection<GetPaymentResult>> HandleAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        var payments =
            await paymentRepository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

        return payments
            .Select(x =>
                new GetPaymentResult(
                    x.Id,
                    x.OrderId,
                    x.Method,
                    x.Status,
                    x.Amount.Amount,
                    x.Amount.Currency,
                    x.ExternalTransactionId,
                    x.CreatedAt))
            .ToArray();
    }
}