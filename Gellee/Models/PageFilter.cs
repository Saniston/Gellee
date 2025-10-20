namespace Gellee.Models
{
    public class PageFilter
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;

        public virtual void AdjustFilter()
        {
            if (this.Page <= 1)
                this.Skip = 0;
            else
                this.Skip = (this.Page * this.Take) - this.Take;
        }
    }
}
