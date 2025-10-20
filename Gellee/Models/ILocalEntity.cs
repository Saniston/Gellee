namespace Gellee.Models
{
    public interface ILocalEntity
    {
        Guid Id { get; set; }
        DateTime CreateDate { get; set; }
        void GenerateId();
    }
}
