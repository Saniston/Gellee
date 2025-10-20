namespace Gellee.Models
{
    public class UnitOfMeasurement : ILocalEntity
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Name { get; set; } = string.Empty; // Ex: "Copo"
        public decimal BaseValue { get; set; } // Ex: 240

        public UnitOfMeasurement()
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
