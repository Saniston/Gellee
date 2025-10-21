using Gellee.Models;
using LiteDB;

namespace Gellee.Services
{
    public class DatabaseService
    {
        private readonly LiteDatabase _db;
        private readonly string _appKey = "161f2a34439d62bGelleeAppDatabase8a9e413bab624a1be";

        public DatabaseService()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, DefaultSettings.DbName);
            //_db = new LiteDatabase(dbPath);
            _db = new LiteDatabase($"Filename={dbPath};Password={_appKey};");
        }

        public ILiteCollection<Ingredient> Ingredients => _db.GetCollection<Ingredient>("ingredients");
        public ILiteCollection<Recipe> Recipes => _db.GetCollection<Recipe>("recipes");
        public ILiteCollection<UnitOfMeasurement> Units => _db.GetCollection<UnitOfMeasurement>("units");
    }

    public static class DefaultSettings
    {
        public static string DbName => "gelleedata.db";
    }
}
