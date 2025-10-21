using Gellee.Services;
using Gellee.Services.Repositories;

namespace Gellee
{
    public static class ServiceRegister
    {
        public static void RegisterAppServices(this IServiceCollection services)
        {
            services.AddSingleton(new DatabaseService());
            services.AddSingleton<UnitOfMeasurementService>();
            services.AddSingleton<IngredientService>();
            services.AddSingleton<RecipeService>();

            services.AddTransient<Pages.Units.UnitsPage>();
            services.AddTransient<Pages.Ingredients.IngredientsPage>();
            services.AddTransient<Pages.Recipes.RecipesPage>();
        }
    }
}
