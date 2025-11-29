namespace application.DTOs
{
    public class CreateInvestmentInput
    {
        public string OwnerId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreationDate { get; set; }
    }
}