using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Services
{
    public class TaxCalculationService : ITaxCalculationService
    {
        public decimal CalculateTax(decimal gain, DateTime creationDate, DateTime withdrawalDate)
        {
            if (gain <= 0)
                return 0;

            var ageInYears = (withdrawalDate - creationDate).TotalDays / 365.25;

            decimal taxRate = ageInYears switch
            {
                < 1 => 0.225m,
                < 2 => 0.185m,
                _ => 0.15m
            };

            return Math.Round(gain * taxRate, 2);
        }
    }
}