using QSR.Application.Interfaces.Repositories.Payments;

namespace QSR.Application.Features.Payments.GetPaymentRefunds;

public class GetPaymentRefundsByPaymentHandler(
    IPaymentRefundRepository paymentRefundRepository)
{
    public async Task<IReadOnlyCollection<PaymentRefundResult>> HandleAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        if (paymentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Payment ID is required.");
        }

        var refunds =
            await paymentRefundRepository.GetByPaymentIdAsync(
                paymentId,
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