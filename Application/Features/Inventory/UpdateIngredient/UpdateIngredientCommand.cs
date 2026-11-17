namespace QSR.Application.Features.Inventory.UpdateIngredient;

public sealed record UpdateIngredientCommand(
    Guid IngredientId,
    string Name,
    decimal MinimumStock);