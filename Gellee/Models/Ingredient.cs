namespace Gellee.Models
{
    public class Ingredient : ILocalEntity
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Name { get; set; } = string.Empty; // Ex: "Açúcar"
        public Ingredient()
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
}
