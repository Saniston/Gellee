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
            _databaseService.Upsert(ingredient);
        }

        public IEnumerable<Recipe> GetPaginated(PageFilter filter = null)
        {
            filter ??= new PageFilter();

            return _databaseService.GetByFilter<Recipe>(
                filter,
                i => string.IsNullOrEmpty(filter.SearchTerm) || i.Name.ToLower().Contains(filter.SearchTerm.ToLower())
            );
        }

        public Recipe? GetById(Guid id)
        {
            return _databaseService.GetById<Recipe>(id);
        }

        public void Delete(Guid id)
        {
            _databaseService.Delete<Recipe>(id);
        }
    }
}
