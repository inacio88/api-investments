namespace core.Entities
{
    public class Investment
    {
        public Guid Id { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreationDate { get; set; }
        public decimal InitialAmount { get; set; }
        public DateTime? WithdrawalDate { get; set; }

        
        public decimal CurrentGains => CalculateGains();
        public decimal ExpectedBalance => InitialAmount + CurrentGains;
        public bool IsWithdrawn => WithdrawalDate.HasValue;

        private decimal CalculateGains()
        {
            if (IsWithdrawn)
                return CalculateGainsUpTo(WithdrawalDate.Value);

            return CalculateGainsUpTo(DateTime.Today);
        }

        private decimal CalculateGainsUpTo(DateTime untilDate)
        {
            if (untilDate < CreationDate)
                return 0;

            var months = GetFullMonthsBetween(CreationDate, untilDate);
            var balance = InitialAmount;

            for (int i = 0; i < months; i++)
            {
                balance *= 1.0052m;
            }

            return balance - InitialAmount;
        }

        private int GetFullMonthsBetween(DateTime start, DateTime end)
        {
            int months = (end.Year - start.Year) * 12 + (end.Month - start.Month);

            
            if (end.Day < start.Day)
                months--;

            return Math.Max(0, months);
        }

        public decimal GetTaxedWithdrawAmount()
        {
            if (!IsWithdrawn)
                throw new InvalidOperationException("Investment has not been withdrawn.");

            var totalGain = CurrentGains;
            var taxRate = GetTaxRate(WithdrawalDate.Value);
            var tax = totalGain * taxRate;
            return ExpectedBalance - tax;
        }

        private decimal GetTaxRate(DateTime withdrawalDate)
        {
            var ageInYears = (withdrawalDate - CreationDate).TotalDays / 365.25;

            if (ageInYears < 1)
                return 0.225m;
            else if (ageInYears < 2)
                return 0.185m;
            else
                return 0.15m;
        }
    }
}