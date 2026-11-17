using QSR.Application.Interfaces.Persistence;
using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Application.Interfaces.Repositories.Payments;
using QSR.Domain.Entities.Branches.Payments;
using QSR.Domain.Enums;
using QSR.Domain.ValueObjects;

namespace QSR.Application.Features.Payments.RefundPayment;

public class RefundPaymentHandler(
    IPaymentRepository paymentRepository,
    IPaymentRefundRepository paymentRefundRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<RefundPaymentResult> HandleAsync(
        RefundPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.PaymentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Payment ID is required.");
        }

        if (command.Amount <= 0)
        {
            throw new ArgumentException(
                "Refund amount must be greater than zero.");
        }

        RefundPaymentResult? result = null;

        await unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var payment =
                    await paymentRepository.GetByIdAsync(
                        command.PaymentId,
                        ct);

                if (payment is null)
                {
                    throw new InvalidOperationException(
                        "Payment was not found.");
                }

                if (payment.Status == PaymentStatus.Refunded)
                {
                    throw new InvalidOperationException(
                        "Payment has already been fully refunded.");
                }

                if (payment.Status != PaymentStatus.Completed)
                {
                    throw new InvalidOperationException(
                        "Only completed payments can be refunded.");
                }

                var order =
                    await orderRepository.GetByIdAsync(
                        payment.OrderId,
                        ct);

                if (order is null)
                {
                    throw new InvalidOperationException(
                        "Order was not found.");
                }

                var paymentRefunds =
                    await paymentRefundRepository.GetByPaymentIdAsync(
                        payment.Id,
                        ct);

                var alreadyRefundedAmount = paymentRefunds
                    .Where(x =>
                        x.Status == PaymentRefundStatus.Completed)
                    .Sum(x => x.Amount.Amount);

                var paymentAmount =
                    payment.Amount.Amount;

                var remainingRefundableAmount =
                    paymentAmount - alreadyRefundedAmount;

                if (remainingRefundableAmount <= 0)
                {
                    throw new InvalidOperationException(
                        "Payment has already been fully refunded.");
                }

                if (command.Amount > remainingRefundableAmount)
                {
                    throw new InvalidOperationException(
                        $"Refund amount exceeds remaining refundable amount. " +
                        $"Maximum refundable amount is " +
                        $"{remainingRefundableAmount} {payment.Amount.Currency}.");
                }

                if (order.Status == OrderStatus.Completed ||
                    order.Status == OrderStatus.Cancelled)
                {
                    order.MarkRefundPending();
                }
                else if (order.Status != OrderStatus.RefundPending &&
                         order.Status != OrderStatus.PartiallyRefunded)
                {
                    throw new InvalidOperationException(
                        $"Order in status {order.Status} cannot be refunded.");
                }

                var refund =
                    PaymentRefund.Create(
                        payment.Id,
                        order.Id,
                        new Money(
                            command.Amount,
                            payment.Amount.Currency),
                        command.Reason);

                refund.Complete(
                    command.ExternalTransactionId);

                await paymentRefundRepository.AddAsync(
                    refund,
                    ct);

                var newTotalRefundedAmount =
                    alreadyRefundedAmount +
                    command.Amount;

                var newRemainingRefundableAmount =
                    paymentAmount -
                    newTotalRefundedAmount;

                if (newRemainingRefundableAmount == 0)
                {
                    payment.MarkRefunded();
                }

                var orderRefunds =
                    await paymentRefundRepository.GetByOrderIdAsync(
                        order.Id,
                        ct);

                var previousOrderRefundedAmount = orderRefunds
                    .Where(x =>
                        x.Status == PaymentRefundStatus.Completed)
                    .Sum(x => x.Amount.Amount);

                var totalOrderRefundedAmount =
                    previousOrderRefundedAmount +
                    command.Amount;

                var orderPayments = await paymentRepository.GetByOrderIdAsync(order.Id, ct);
                var capturedAmount = orderPayments
                    .Where(x => x.Status is PaymentStatus.Completed or PaymentStatus.Refunded)
                    .Sum(x => x.Amount.Amount);

                if (totalOrderRefundedAmount >= capturedAmount)
                {
                    order.MarkRefunded();
                }
                else
                {
                    order.MarkPartiallyRefunded();
                }

                await paymentRefundRepository.SaveChangesAsync(
                    ct);

                result = new RefundPaymentResult(
                    refund.Id,
                    payment.Id,
                    order.Id,
                    command.Amount,
                    paymentAmount,
                    newTotalRefundedAmount,
                    newRemainingRefundableAmount,
                    payment.Amount.Currency,
                    refund.Status,
                    payment.Status,
                    order.Status);
            },
            cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "Refund operation failed.");
    }
}