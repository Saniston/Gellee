using Gellee.Models;

namespace Gellee.Services.Repositories
{
    public class RecipeService
    {
        private readonly DatabaseService _databaseService;

        public RecipeService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void Save(Recipe ingredient)
        {
            _databaseService.Recipes.Upsert(ingredient);
        }

        public IEnumerable<Recipe> GetPaginated(PageFilter filter = null)
        {
            if (filter is not null)
            {
                filter.AdjustFilter();

                return _databaseService.Recipes.Find(
                    i => string.IsNullOrEmpty(filter.SearchTerm) || i.Name.ToLower().Contains(filter.SearchTerm.ToLower()),
                    skip: filter.Skip,
                    limit: filter.Take
                );
            }

            return _databaseService.Recipes.FindAll();
        }

        public Recipe? GetById(int id)
        {
            return _databaseService.Recipes.FindById(id);
        }

        public void Delete(int id)
        {
            _databaseService.Units.Delete(id);
        }
    }
}
