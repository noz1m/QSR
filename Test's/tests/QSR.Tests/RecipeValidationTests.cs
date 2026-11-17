using QSR.Application.Features.Recipes.CreateRecipe;
using QSR.Domain.Entities.Branches.Recipes;

namespace QSR.Tests;

public class RecipeValidationTests
{
    [Fact]
    public async Task EmptyRecipeIsRejectedBeforePersistence()
    {
        var handler = new CreateRecipeHandler(null!, null!, null!);
        var command = new CreateRecipeCommand(Guid.NewGuid(), Guid.NewGuid(),
            1, DateTimeOffset.UtcNow, null, []);

        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task DuplicateIngredientIsRejectedBeforePersistence()
    {
        var handler = new CreateRecipeHandler(null!, null!, null!);
        var ingredient = new CreateRecipeIngredient(
            Guid.NewGuid(), Guid.NewGuid(), 1, "KG");
        var command = new CreateRecipeCommand(Guid.NewGuid(), Guid.NewGuid(),
            1, DateTimeOffset.UtcNow, null, [ingredient, ingredient]);

        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public void RecipeCannotContainIngredientFromAnotherRecipe()
    {
        var recipe = BranchRecipe.Create(Guid.NewGuid(), Guid.NewGuid(),
            1, DateTimeOffset.UtcNow);
        var ingredient = BranchRecipeIngredient.Create(Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), 1, "KG");

        Assert.Throws<InvalidOperationException>(() => recipe.AddIngredient(ingredient));
        Assert.Empty(recipe.Ingredients);
    }
}
