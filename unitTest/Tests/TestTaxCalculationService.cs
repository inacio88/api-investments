using core.Services;

namespace unitTest.Tests
{
    public class TestTaxCalculationService
    {
        private TaxCalculationService _service;

        [SetUp]
        public void Setup()
        {
            _service = new TaxCalculationService();
        }

        [Test]
        public void CalculateTax_ReturnsZero_WhenGainIsZeroOrNegative()
        {
            Assert.That(_service.CalculateTax(0, DateTime.Now.AddYears(-1), DateTime.Now), Is.EqualTo(0));
            Assert.That(_service.CalculateTax(-50, DateTime.Now.AddYears(-1), DateTime.Now), Is.EqualTo(0));
        }

        [Test]
        public void CalculateTax_Uses_22_5_Percent_WhenAgeIsLessThanOneYear()
        {
            var creation = new DateTime(2024, 1, 10);
            var withdrawal = new DateTime(2024, 6, 10);

            var result = _service.CalculateTax(200m, creation, withdrawal);

            Assert.That(result, Is.EqualTo(45.00m));
        }

        [Test]
        public void CalculateTax_Uses_18_5_Percent_WhenAgeIsBetweenOneAndTwoYears()
        {
            var creation = new DateTime(2023, 1, 10);
            var withdrawal = new DateTime(2024, 6, 10);

            var result = _service.CalculateTax(200m, creation, withdrawal);

            Assert.That(result, Is.EqualTo(37.00m));
        }

        [Test]
        public void CalculateTax_Uses_15_Percent_WhenAgeIsTwoYearsOrMore()
        {
            var creation = new DateTime(2022, 1, 10);
            var withdrawal = new DateTime(2024, 6, 10);

            var result = _service.CalculateTax(200m, creation, withdrawal);

            Assert.That(result, Is.EqualTo(30.00m));
        }

        [Test]
        public void CalculateTax_HandlesBoundaryAtExactlyOneYear()
        {
            var creation = new DateTime(2023, 1, 1);
            var withdrawal = new DateTime(2024, 1, 1);

            var result = _service.CalculateTax(100m, creation, withdrawal);

            Assert.That(result, Is.EqualTo(18.50m)); 
        }

        [Test]
        public void CalculateTax_HandlesBoundaryAtExactlyTwoYears()
        {
            var creation = new DateTime(2022, 1, 1);
            var withdrawal = new DateTime(2024, 1, 1);

            var result = _service.CalculateTax(100m, creation, withdrawal);

            Assert.That(result, Is.EqualTo(15.00m));
        }

        [Test]
        public void CalculateTax_RoundsToTwoDecimalPlaces()
        {
            var creation = new DateTime(2024, 1, 10);
            var withdrawal = new DateTime(2024, 6, 10);

            var result = _service.CalculateTax(123.456m, creation, withdrawal);

            Assert.That(result, Is.EqualTo(Math.Round(123.456m * 0.225m, 2)));
        }
    }
}
