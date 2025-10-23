using Gellee.Models;

namespace Gellee.Services.Repositories
{
    public class UnitOfMeasurementService
    {
        private readonly DatabaseService _databaseService;

        public UnitOfMeasurementService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void Save(UnitOfMeasurement ingredient)
        {
            _databaseService.Upsert(ingredient);
        }

        public IEnumerable<UnitOfMeasurement> GetPaginated(PageFilter filter = null)
        {
            filter ??= new PageFilter();

            return _databaseService.GetByFilter<UnitOfMeasurement>(
                filter,
                i => string.IsNullOrEmpty(filter.SearchTerm) || i.Name.ToLower().Contains(filter.SearchTerm.ToLower())
            );
        }

        public UnitOfMeasurement? GetById(Guid id)
        {
            return _databaseService.GetById<UnitOfMeasurement>(id);
        }

        public void Delete(Guid id)
        {
            _databaseService.Delete<UnitOfMeasurement>(id);
        }
    }
}
