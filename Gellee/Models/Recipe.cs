using LiteDB;

namespace Gellee.Models
{
    public class Recipe : ILocalEntity
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<RecipeIngredient> Ingredients { get; set; } = [];

        public Recipe()
        {
            Id = Guid.NewGuid();
            CreateDate = DateTime.Now;
        }

        public void GenerateId()
        {
            this.Id = Guid.NewGuid();
            this.CreateDate = DateTime.Now;
        }
    }

    public class RecipeIngredient
    {
        [BsonIgnore]
        public string Name => Ingredient?.Name ?? "";
        public decimal Quantity { get; set; }
        [BsonIgnore]
        public string UnitName => UnitOfMeasurement?.Name ?? "";
        public Guid IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = new Ingredient();
        public Guid UnitOfMeasurementId { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; } = new UnitOfMeasurement();
    }
}
