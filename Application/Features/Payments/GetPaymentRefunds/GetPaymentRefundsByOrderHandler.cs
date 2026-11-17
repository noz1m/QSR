using QSR.Application.Interfaces.Repositories.Payments;

namespace QSR.Application.Features.Payments.GetPaymentRefunds;

public class GetPaymentRefundsByOrderHandler(
    IPaymentRefundRepository paymentRefundRepository)
{
    public async Task<IReadOnlyCollection<PaymentRefundResult>> HandleAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Order ID is required.");
        }

        var refunds =
            await paymentRefundRepository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

        return refunds
            .Select(x =>
                new PaymentRefundResult(
                    x.Id,
                    x.PaymentId,
                    x.OrderId,
                    x.Amount.Amount,
                    x.Amount.Currency,
                    x.Status,
                    x.Reason,
                    x.ExternalTransactionId,
                    x.CreatedAt))
            .ToArray();
    }
}