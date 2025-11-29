namespace core.Filters
{
    public class InvestmentFilter
    {
        public string OwnerId { get; private set; } = string.Empty;
        public Guid? Id { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;


        public int Skip => (Math.Max(1, Page) - 1) * PageSizeClamped;
        public int PageSizeClamped => Math.Clamp(PageSize, 1, 100);

        public InvestmentFilter(string ownerId)
        {
            OwnerId = ownerId;
        }

        public void SetOwner(string ownerId)
        {
            OwnerId = ownerId;
        }

    }
}