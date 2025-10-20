using Gellee.Models;

namespace Gellee.Services
{
    public static class RecipeCalculator
    {
        public static List<RecipeIngredient> Recalculate(List<RecipeIngredient> ingredients, string baseIngredientName, decimal newQuantity)
        {
            var baseIngredient = ingredients.First(i => i.Ingredient.Name.Equals(baseIngredientName, StringComparison.OrdinalIgnoreCase));
            decimal ratio = newQuantity / baseIngredient.Quantity;

            return ingredients.Select(i => new RecipeIngredient
            {
                Ingredient = i.Ingredient,
                Quantity = i.Quantity * ratio,
                UnitOfMeasurement = i.UnitOfMeasurement
            }).ToList();
        }
    }
}
