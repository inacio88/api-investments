namespace core.Services
{
    public interface IGainCalculationService
    {
        decimal CalculateGains(decimal initialAmount, DateTime creationDate, DateTime untilDate);
    }
}