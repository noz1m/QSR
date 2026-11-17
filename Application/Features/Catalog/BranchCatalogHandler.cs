using QSR.Domain.Entities.Branches.Kitchen;
using QSR.Domain.Entities.Branches.Menu;
using QSR.Domain.Enums;
using QSR.Domain.ValueObjects;

namespace QSR.Application.Features.Catalog;

public class BranchCatalogHandler(IBranchCatalogRepository repository)
{
    public async Task<Guid> CreateMenuAsync(Guid branchId, CreateMenuCommand command,
        CancellationToken cancellationToken)
    {
        var menu = BranchMenu.Create(branchId, command.SourceTemplateId,
            command.Name, command.SourceVersion);
        await repository.AddMenuAsync(menu, cancellationToken);
        await repository.SaveAsync(cancellationToken);
        return menu.Id;
    }

    public async Task<Guid> CreateCategoryAsync(Guid branchId, Guid menuId,
        CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var menu = await repository.GetMenuAsync(menuId, cancellationToken);
        if (menu is null || menu.BranchId != branchId)
            throw new KeyNotFoundException("Menu was not found.");
        var category = BranchCategory.Create(menu.Id, command.SourceTemplateId,
            command.Name, command.DisplayOrder);
        menu.AddCategory(category);
        await repository.AddCategoryAsync(category, cancellationToken);
        await repository.SaveAsync(cancellationToken);
        return category.Id;
    }

    public async Task<Guid> CreateProductAsync(Guid branchId, Guid categoryId,
        CreateProductCommand command, CancellationToken cancellationToken)
    {
        var category = await repository.GetCategoryAsync(categoryId, cancellationToken)
            ?? throw new KeyNotFoundException("Category was not found.");
        var menu = await repository.GetMenuAsync(category.BranchMenuId, cancellationToken);
        if (menu is null || menu.BranchId != branchId)
            throw new KeyNotFoundException("Category was not found.");
        var station = await repository.GetStationAsync(command.KitchenStationId, cancellationToken);
        if (station is null || station.BranchId != branchId || !station.IsActive)
            throw new ArgumentException("An active station in this branch is required.");
        if (command.Currency is not { Length: 3 } ||
            !command.Currency.All(char.IsAsciiLetter) ||
            decimal.Round(command.Price, 2) != command.Price)
            throw new ArgumentException("Valid currency and price precision are required.");

        var product = BranchProduct.Create(category.Id, command.SourceTemplateId,
            command.Name, new Money(command.Price, command.Currency.ToUpperInvariant()),
            command.DisplayOrder, command.Description);
        product.UpdateKitchenSettings(station.Id, command.PreparationTimeMinutes);
        category.AddProduct(product);
        await repository.AddProductAsync(product, cancellationToken);
        await repository.SaveAsync(cancellationToken);
        return product.Id;
    }

    public async Task<Guid> CreateStationAsync(Guid branchId, CreateStationCommand command,
        CancellationToken cancellationToken)
    {
        var station = KitchenStation.Create(branchId, command.Name, command.Code,
            command.Type, command.DisplayOrder);
        await repository.AddStationAsync(station, cancellationToken);
        await repository.SaveAsync(cancellationToken);
        return station.Id;
    }
}

public sealed record CreateMenuCommand(Guid SourceTemplateId, string Name, int SourceVersion);
public sealed record CreateCategoryCommand(Guid SourceTemplateId, string Name, int DisplayOrder);
public sealed record CreateProductCommand(Guid SourceTemplateId, string Name, decimal Price,
    string Currency, Guid KitchenStationId, int PreparationTimeMinutes,
    int DisplayOrder, string? Description);
public sealed record CreateStationCommand(string Name, string Code,
    KitchenStationType Type, int DisplayOrder);
