namespace application.DTOs
{
    public class CreateInvestmentInput
    {
        public string OwnerId { get; private set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreationDate { get; set; }

        public void SetOwner(string ownerId)
        {
            OwnerId = ownerId;
        }
    }
}