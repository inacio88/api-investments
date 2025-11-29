namespace core.Services
{
    public interface ITaxCalculationService
    {
        decimal CalculateTax(decimal gain, DateTime creationDate, DateTime withdrawalDate);
    }
}