using Gellee.Models;

namespace Gellee.Services.Repositories
{
    public class IngredientService
    {
        private readonly DatabaseService _databaseService;

        public IngredientService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void Save(Ingredient ingredient)
        {
            _databaseService.Upsert(ingredient);
        }

        public IEnumerable<Ingredient> GetPaginated(PageFilter filter = null)
        {
            filter ??= new PageFilter();

            return _databaseService.GetByFilter<Ingredient>(
                filter,
                i => string.IsNullOrEmpty(filter.SearchTerm) || i.Name.ToLower().Contains(filter.SearchTerm.ToLower())
            );
        }

        public Ingredient? GetById(Guid id)
        {
            return _databaseService.GetById<Ingredient>(id);
        }

        public void Delete(Guid id)
        {
            _databaseService.Delete<Ingredient>(id);
        }
    }
}
