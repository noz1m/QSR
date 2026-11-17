using QSR.Application.Interfaces.Repositories.Orders;
using QSR.Application.Interfaces.Repositories.Payments;
using QSR.Domain.Entities.Branches.Payments;
using QSR.Domain.Enums;
using QSR.Domain.ValueObjects;

namespace QSR.Application.Features.Payments.AddPayment;

public class AddPaymentHandler(
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository)
{
    public async Task<AddPaymentResult> HandleAsync(
        AddPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.OrderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        if (command.Amount <= 0 || decimal.Round(command.Amount, 2) != command.Amount)
            throw new ArgumentException(
                "Payment amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(command.Currency))
            throw new ArgumentException(
                "Currency is required.");

        var order = await orderRepository.GetByIdAsync(
            command.OrderId,
            cancellationToken);

        if (order is null)
            throw new InvalidOperationException(
                "Order was not found.");

        if (!string.Equals(
                order.TotalAmount.Currency,
                command.Currency,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Payment currency must match order currency.");
        }

        var existingPayments =
            await paymentRepository.GetByOrderIdAsync(
                order.Id,
                cancellationToken);

        var previous = command.ExternalTransactionId is null ? null :
            existingPayments.SingleOrDefault(x =>
                x.ExternalTransactionId == command.ExternalTransactionId);

        if (previous is not null)
        {
            if (previous.Method != command.Method || previous.Amount.Amount != command.Amount)
                throw new InvalidOperationException("Payment reference already used.");

            return new AddPaymentResult(previous.Id, previous.OrderId, previous.Method,
                previous.Status, previous.Amount.Amount, previous.Amount.Currency, order.Status);
        }

        var alreadyPaid = existingPayments
            .Where(x => x.Status == PaymentStatus.Completed)
            .Sum(x => x.Amount.Amount);

        var remainingAmount =
            order.TotalAmount.Amount - alreadyPaid;

        if (remainingAmount <= 0)
        {
            throw new InvalidOperationException(
                "Order is already fully paid.");
        }

        if (command.Amount > remainingAmount)
        {
            throw new InvalidOperationException(
                $"Payment exceeds remaining amount. Remaining: {remainingAmount} {order.TotalAmount.Currency}.");
        }

        var payment = Payment.Create(
            order.Id,
            command.Method,
            new Money(
                command.Amount,
                command.Currency));

        payment.Complete(
            command.ExternalTransactionId);

        await paymentRepository.AddAsync(
            payment,
            cancellationToken);

        var totalPaid =
            alreadyPaid + payment.Amount.Amount;

        if (totalPaid < order.TotalAmount.Amount)
        {
            order.MarkPartiallyPaid();
        }
        else
        {
            order.MarkPaid();
        }

        await paymentRepository.SaveChangesAsync(
            cancellationToken);

        return new AddPaymentResult(
            payment.Id,
            payment.OrderId,
            payment.Method,
            payment.Status,
            payment.Amount.Amount,
            payment.Amount.Currency,
            order.Status);
    }
}