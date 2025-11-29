namespace application.DTOs
{
    public class InvestmentDto
    {
        public Guid Id { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreationDate { get; set; }
        public decimal InitialAmount { get; set; }
        public DateTime? WithdrawalDate { get; set; }
        public decimal ExpectedBalance { get; set; }
        public decimal? NetWithdrawAmount { get; set; }
        public bool IsWithdrawn => WithdrawalDate.HasValue;
    }
}