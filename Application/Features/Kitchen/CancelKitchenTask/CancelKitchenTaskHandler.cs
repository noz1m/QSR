using QSR.Application.Interfaces.Repositories.Kitchen;

namespace QSR.Application.Features.Kitchen.CancelKitchenTask;

public class CancelKitchenTaskHandler(
    IKitchenTaskRepository kitchenTaskRepository)
{
    public async Task HandleAsync(
        Guid kitchenTaskId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        if (kitchenTaskId == Guid.Empty)
            throw new ArgumentException(
                "Kitchen task ID is required.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "Cancellation reason is required.");

        var kitchenTask =
            await kitchenTaskRepository.GetByIdAsync(
                kitchenTaskId,
                cancellationToken);

        if (kitchenTask is null)
            throw new InvalidOperationException(
                "Kitchen task was not found.");

        kitchenTask.Cancel(
            reason,
            DateTimeOffset.UtcNow);

        await kitchenTaskRepository.SaveChangesAsync(
            cancellationToken);
    }
}