using QSR.Domain.Common;
using QSR.Domain.Enums;

namespace QSR.Domain.Entities.Branches.Kitchen;

public class KitchenTask : AuditableEntity
{
    private readonly List<KitchenTaskItem> _items = [];

    public Guid BranchId { get; private set; }

    public Guid OrderId { get; private set; }

    public Guid KitchenStationId { get; private set; }

    public KitchenTaskStatus Status { get; private set; }

    public DateTimeOffset? AcceptedAt { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? ReadyAt { get; private set; }

    public DateTimeOffset? HandedOffAt { get; private set; }

    public DateTimeOffset? CancelledAt { get; private set; }

    public string? CancellationReason { get; private set; }

    public IReadOnlyCollection<KitchenTaskItem> Items =>
        _items.AsReadOnly();

    private KitchenTask()
    {
    }

    public static KitchenTask Create(
        Guid branchId,
        Guid orderId,
        Guid kitchenStationId)
    {
        if (branchId == Guid.Empty)
            throw new ArgumentException(
                "Branch ID is required.");

        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        if (kitchenStationId == Guid.Empty)
            throw new ArgumentException(
                "Kitchen station ID is required.");

        return new KitchenTask
        {
            BranchId = branchId,
            OrderId = orderId,
            KitchenStationId = kitchenStationId,
            Status = KitchenTaskStatus.New
        };
    }

    public void AddItem(KitchenTaskItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (Status != KitchenTaskStatus.New)
            throw new InvalidOperationException(
                "Items can only be added while the kitchen task is New.");

        if (item.KitchenTaskId != Id)
            throw new InvalidOperationException(
                "Kitchen task item belongs to another task.");

        if (_items.Any(x => x.Id == item.Id))
            throw new InvalidOperationException(
                "Kitchen task item already exists.");

        _items.Add(item);
    }

    public void Accept(DateTimeOffset acceptedAt)
    {
        EnsureStatus(KitchenTaskStatus.New);

        if (_items.Count == 0)
            throw new InvalidOperationException(
                "Cannot accept an empty kitchen task.");

        Status = KitchenTaskStatus.Accepted;
        AcceptedAt = acceptedAt;
    }

    public void StartPreparing(DateTimeOffset startedAt)
    {
        EnsureStatus(KitchenTaskStatus.Accepted);

        Status = KitchenTaskStatus.Preparing;
        StartedAt = startedAt;
    }

    public void MarkReady(DateTimeOffset readyAt)
    {
        EnsureStatus(KitchenTaskStatus.Preparing);

        Status = KitchenTaskStatus.Ready;
        ReadyAt = readyAt;
    }

    public void HandOff(DateTimeOffset handedOffAt)
    {
        EnsureStatus(KitchenTaskStatus.Ready);

        Status = KitchenTaskStatus.HandedOff;
        HandedOffAt = handedOffAt;
    }

    public void Cancel(
        string reason,
        DateTimeOffset cancelledAt)
    {
        if (Status is KitchenTaskStatus.HandedOff
            or KitchenTaskStatus.Cancelled)
        {
            throw new InvalidOperationException(
                $"Cannot cancel kitchen task from status {Status}.");
        }

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "Cancellation reason is required.");

        Status = KitchenTaskStatus.Cancelled;
        CancellationReason = reason.Trim();
        CancelledAt = cancelledAt;
    }

    private void EnsureStatus(KitchenTaskStatus expectedStatus)
    {
        if (Status != expectedStatus)
        {
            throw new InvalidOperationException(
                $"Kitchen task must be in {expectedStatus} status.");
        }
    }
}