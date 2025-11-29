namespace core.Services
{
    public class GainCalculationService : IGainCalculationService
    {
        public decimal CalculateGains(decimal initialAmount, DateTime creationDate, DateTime untilDate)
        {
            if (untilDate < creationDate || initialAmount <= 0)
                return 0;

            var months = GetFullMonthsBetween(creationDate, untilDate);
            var balance = initialAmount;

            for (int i = 0; i < months; i++)
            {
                balance *= 1.0052m; // juros compostos de 0.52% ao mês
            }

            var gains = balance - initialAmount;
            return Math.Round(gains, 2);
        }

        private int GetFullMonthsBetween(DateTime start, DateTime end)
        {
            int months = (end.Year - start.Year) * 12 + (end.Month - start.Month);
            if (end.Day < start.Day)
                months--;
            return Math.Max(0, months);
        }
    }
}