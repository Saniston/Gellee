using Gellee.Models;
using LiteDB;
using System.Linq.Expressions;

namespace Gellee.Services
{
    public class DatabaseService
    {
        private readonly string _dbPath;
        private readonly string _appKey = "161f2a34439d62bGelleeAppDatabase8a9e413bab624a1be";

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, DefaultSettings.DbName);
        }

        public LiteDatabase GetDatabase()
        {
            return new LiteDatabase(new ConnectionString
            {
                Filename = _dbPath,
                Password = _appKey,
                Connection = ConnectionType.Shared
            });
        }

        public ILiteCollection<Ingredient> Ingredients
        {
            get
            {
                using (var liteDb = GetDatabase())
                {
                    return liteDb.GetCollection<Ingredient>("ingredients");
                }
            }
        }

        public ILiteCollection<Recipe> Recipes
        {
            get
            {
                using (var liteDb = GetDatabase())
                {
                    return liteDb.GetCollection<Recipe>("recipes");
                }
            }
        }

        public ILiteCollection<UnitOfMeasurement> Units
        {
            get
            {
                using (var liteDb = GetDatabase())
                {
                    return liteDb.GetCollection<UnitOfMeasurement>("units");
                }
            }
        }

        public T GetById<T>(Guid id) where T : ILocalEntity, new()
        {
            using (var liteDb = GetDatabase())
            {
                var collection = liteDb.GetCollection<T>();
                return collection.FindById(id);
            }
        }

        public List<T> GetAll<T>() where T : ILocalEntity, new()
        {
            using (var liteDb = GetDatabase())
            {
                var collection = liteDb.GetCollection<T>();
                return [.. collection.FindAll()];
            }
        }

        public List<T> GetByFilter<T>(PageFilter filter, Expression<Func<T, bool>> predicate) where T : ILocalEntity, new()
        {
            if (filter is not null)
            {
                filter.AdjustFilter();

                using (var liteDb = GetDatabase())
                {
                    return [.. liteDb.GetCollection<T>().Find(predicate, filter.Skip, filter.Take)];
                }
            }
            else
            {
                return GetAll<T>();
            }
        }

        public void Delete<T>(Guid id) where T : ILocalEntity, new()
        {
            using (var liteDb = GetDatabase())
            {
                var collection = liteDb.GetCollection<T>();
                collection.Delete(id);
            }
        }

        public void Upsert<T>(T entity) where T : ILocalEntity, new()
        {
            using (var liteDb = GetDatabase())
            {
                var collection = liteDb.GetCollection<T>();
                collection.Upsert(entity);
            }
        }
    }

    public static class DefaultSettings
    {
        public static string DbName => "gelleedata.db";
    }
}
