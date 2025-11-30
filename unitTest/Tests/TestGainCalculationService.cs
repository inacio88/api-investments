using core.Services;

namespace unitTest.Tests
{
    public class TestGainCalculationService
    {
        private GainCalculationService _service;

        [SetUp]
        public void Setup()
        {
            _service = new GainCalculationService();
        }

        [Test]
        public void CalculateGains_ReturnsZero_WhenInitialAmountIsZeroOrNegative()
        {
            var result1 = _service.CalculateGains(0, DateTime.Now, DateTime.Now.AddMonths(5));
            var result2 = _service.CalculateGains(-100, DateTime.Now, DateTime.Now.AddMonths(5));

            Assert.That(result1, Is.EqualTo(0));
            Assert.That(result2, Is.EqualTo(0));
        }

        [Test]
        public void CalculateGains_ReturnsZero_WhenUntilDateIsBeforeCreationDate()
        {
            var result = _service.CalculateGains(1000, new DateTime(2024, 5, 10), new DateTime(2024, 5, 5));

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void CalculateGains_ReturnsZero_WhenNoFullMonthsHavePassed()
        {
            var result = _service.CalculateGains(
                1000,
                new DateTime(2024, 5, 10),
                new DateTime(2024, 5, 30)
            );

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void CalculateGains_ReturnsCorrectValue_ForOneFullMonth()
        {
            var result = _service.CalculateGains(
                1000,
                new DateTime(2024, 1, 10),
                new DateTime(2024, 2, 10)
            );

            Assert.That(result, Is.EqualTo(5.20m));
        }

        [Test]
        public void CalculateGains_ReturnsCorrectValue_ForMultipleMonths()
        {
            var result = _service.CalculateGains(
                1000,
                new DateTime(2024, 1, 10),
                new DateTime(2024, 4, 10)
            );

            Assert.That(result, Is.EqualTo(15.68m));
        }

        [Test]
        public void CalculateGains_HandlesEndDateEarlierInMonthCorrectly()
        {
            var result = _service.CalculateGains(
                1000,
                new DateTime(2024, 1, 15),
                new DateTime(2024, 2, 14)
            );

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void CalculateGains_HandlesExactMonthBoundary()
        {
            var result = _service.CalculateGains(
                2000,
                new DateTime(2024, 2, 20),
                new DateTime(2024, 3, 20)
            );


            Assert.That(result, Is.EqualTo(10.40m));
        }
    }
}
