using QSR.Application.Interfaces.Repositories.Kitchen;

namespace QSR.Application.Features.Kitchen.HandOffKitchenTask;

public class HandOffKitchenTaskHandler(
    IKitchenTaskRepository kitchenTaskRepository)
{
    public async Task HandleAsync(
        Guid kitchenTaskId,
        CancellationToken cancellationToken = default)
    {
        if (kitchenTaskId == Guid.Empty)
            throw new ArgumentException(
                "Kitchen task ID is required.");

        var kitchenTask =
            await kitchenTaskRepository.GetByIdAsync(
                kitchenTaskId,
                cancellationToken);

        if (kitchenTask is null)
            throw new InvalidOperationException(
                "Kitchen task was not found.");

        kitchenTask.HandOff(
            DateTimeOffset.UtcNow);

        await kitchenTaskRepository.SaveChangesAsync(
            cancellationToken);
    }
}