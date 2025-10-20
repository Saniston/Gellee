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
            _databaseService.Ingredients.Upsert(ingredient);
        }

        public IEnumerable<Ingredient> GetPaginated(PageFilter filter = null)
        {
            if (filter is not null)
            {
                filter.AdjustFilter();
                
                return _databaseService.Ingredients.Find(
                    i => string.IsNullOrEmpty(filter.SearchTerm) || i.Name.ToLower().Contains(filter.SearchTerm.ToLower()),
                    skip: filter.Skip,
                    limit: filter.Take
                );
            }

            return _databaseService.Ingredients.FindAll();
        }

        public Ingredient? GetById(int id)
        {
            return _databaseService.Ingredients.FindById(id);
        }

        public void Delete(int id)
        {
            _databaseService.Ingredients.Delete(id);
        }
    }
}
