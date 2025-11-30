using application.DTOs;
using application.Services;
using core.Entities;
using core.Filters;
using core.Repositories;
using core.Services;
using Moq;

namespace unitTest.Tests
{
    public class TestInvestmentService
    {
        private Mock<IInvestmentRepository> _repoMock = null!;
        private IGainCalculationService _gainMock = null!;
        private ITaxCalculationService _taxMock = null!;
        private InvestmentService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IInvestmentRepository>();
            _gainMock = new GainCalculationService();
            _taxMock = new TaxCalculationService();

            _service = new InvestmentService(
                _repoMock.Object,
                _gainMock,
                _taxMock
            );
        }


        [Test]
        public async Task CreateInvestmentAsync_ShouldCreateCorrectly()
        {
            var input = new CreateInvestmentInput
            {
                Amount = 1000,
                CreationDate = DateTime.Today
            };
            input.SetOwner("user-123");

            Investment? savedInvestment = null!;
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Investment>()))
                     .Callback<Investment>(inv => savedInvestment = inv)
                     .Returns(Task.CompletedTask);

            var result = await _service.CreateInvestmentAsync(input);

            Assert.That(savedInvestment, Is.Not.Null);
            Assert.That(savedInvestment.OwnerId, Is.EqualTo("user-123"));
            Assert.That(savedInvestment.InitialAmount, Is.EqualTo(1000));
            Assert.That(result.InitialAmount, Is.EqualTo(1000));
        }

        [Test]
        public void CreateInvestmentAsync_ShouldThrow_WhenOwnerIdMissing()
        {
            var input = new CreateInvestmentInput
            {
                Amount = 1000,
                CreationDate = DateTime.Today
            };

            Assert.ThrowsAsync<ArgumentException>(() => _service.CreateInvestmentAsync(input));
        }


        [Test]
        public async Task WithdrawInvestmentAsync_ShouldUpdateWithdrawalDate()
        {
            var id = Guid.NewGuid();
            var investment = new Investment
            {
                Id = id,
                OwnerId = "user-1",
                CreationDate = DateTime.Today.AddDays(-10),
                InitialAmount = 500
            };

            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<InvestmentFilter>()))
                     .ReturnsAsync(investment);

            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Investment>()))
                     .Returns(Task.CompletedTask);

            var withdrawalDate = DateTime.Today;

            await _service.WithdrawInvestmentAsync(id, "user-1", withdrawalDate);

            Assert.That(investment.WithdrawalDate, Is.Not.Null);
            Assert.That(investment.WithdrawalDate!.Value.Date, Is.EqualTo(withdrawalDate.Date));
        }

        [Test]
        public void WithdrawInvestmentAsync_ShouldThrow_WhenInvestmentNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<InvestmentFilter>()))
                     .ReturnsAsync((Investment?)null);

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.WithdrawInvestmentAsync(Guid.NewGuid(), "user-1", DateTime.Today));
        }

        [Test]
        public void WithdrawInvestmentAsync_ShouldThrow_WhenAlreadyWithdrawn()
        {
            var investment = new Investment
            {
                Id = Guid.NewGuid(),
                OwnerId = "user-1",
                CreationDate = DateTime.Today.AddDays(-10),
                WithdrawalDate = DateTime.Today
            };

            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<InvestmentFilter>()))
                     .ReturnsAsync(investment);

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.WithdrawInvestmentAsync(investment.Id, "user-1", DateTime.Today));
        }

        [Test]
        public void WithdrawInvestmentAsync_ShouldThrow_WhenWithdrawalDateBeforeCreation()
        {
            var inv = new Investment
            {
                Id = Guid.NewGuid(),
                OwnerId = "user-1",
                CreationDate = DateTime.Today,
                InitialAmount = 100
            };

            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<InvestmentFilter>()))
                     .ReturnsAsync(inv);

            Assert.ThrowsAsync<ArgumentException>(() =>
                _service.WithdrawInvestmentAsync(inv.Id, "user-1", DateTime.Today.AddDays(-1)));
        }

        [Test]
        public void WithdrawInvestmentAsync_ShouldThrow_WhenWithdrawalDateInFuture()
        {
            var inv = new Investment
            {
                Id = Guid.NewGuid(),
                OwnerId = "user-1",
                CreationDate = DateTime.Today.AddDays(-5),
                InitialAmount = 100
            };

            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<InvestmentFilter>()))
                     .ReturnsAsync(inv);

            Assert.ThrowsAsync<ArgumentException>(() =>
                _service.WithdrawInvestmentAsync(inv.Id, "user-1", DateTime.Today.AddDays(1)));
        }


        [Test]
        public async Task GetInvestmentByIdAsync_ShouldReturnDto()
        {
            var id = Guid.NewGuid();
            var inv = new Investment
            {
                Id = id,
                OwnerId = "user-1",
                CreationDate = DateTime.Today,
                InitialAmount = 100
            };

            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<InvestmentFilter>()))
                     .ReturnsAsync(inv);


            var result = await _service.GetInvestmentByIdAsync(new InvestmentFilter("user-1"){ Id = id});

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ExpectedBalance, Is.EqualTo(100));
        }


        [Test]
        public async Task GetInvestmentsByOwnerAsync_ShouldReturnPaginatedResult()
        {
            var invList = new List<Investment>
            {
                new Investment { Id = Guid.NewGuid(), OwnerId = "user-1", InitialAmount = 100, CreationDate = DateTime.Today }
            };

            _repoMock.Setup(r => r.GetByOwnerIdAsync(It.IsAny<InvestmentFilter>()))
                     .ReturnsAsync(invList);

            _repoMock.Setup(r => r.CountByOwnerIdAsync(It.IsAny<InvestmentFilter>()))
                     .ReturnsAsync(1);


            var result = await _service.GetInvestmentsByOwnerAsync(new InvestmentFilter("user-1"));

            Assert.That(result.Items.Count, Is.EqualTo(1));
            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.That(result.Items[0].ExpectedBalance, Is.EqualTo(100));
        }

        [Test]
        public async Task MapToDtoAsync_ShouldCalculateGainsAndTaxes()
        {
            var inv = new Investment
            {
                Id = Guid.NewGuid(),
                OwnerId = "u1",
                InitialAmount = 1000,
                CreationDate = DateTime.Today.AddYears(-2),
                WithdrawalDate = DateTime.Today
            };

            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<InvestmentFilter>()))
                     .ReturnsAsync(inv);

            var result = await _service.GetInvestmentByIdAsync(new InvestmentFilter("u1"));

            Assert.That(result!.ExpectedBalance, Is.EqualTo(1132.56m));
            Assert.That(result.NetWithdrawAmount, Is.EqualTo(1112.68m));
        }
    }
}
