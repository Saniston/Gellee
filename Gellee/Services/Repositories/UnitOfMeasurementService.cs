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
            _databaseService.Units.Upsert(ingredient);
        }

        public IEnumerable<UnitOfMeasurement> GetPaginated(PageFilter filter = null)
        {
            if (filter is not null)
            {
                filter.AdjustFilter();

                return _databaseService.Units.Find(
                    i => string.IsNullOrEmpty(filter.SearchTerm) || i.Name.ToLower().Contains(filter.SearchTerm.ToLower()),
                    skip: filter.Skip,
                    limit: filter.Take
                );
            }

            return _databaseService.Units.FindAll();
        }

        public UnitOfMeasurement? GetById(int id)
        {
            return _databaseService.Units.FindById(id);
        }

        public void Delete(int id)
        {
            _databaseService.Units.Delete(id);
        }
    }
}
