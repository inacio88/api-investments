using core.Entities;
using core.Repositories;
using core.Services;

namespace application.Services
{
    public class InvestmentService : IInvestmentService
    {
        private readonly IInvestmentRepository _repository;
        private readonly IGainCalculationService _gainService;
        private readonly ITaxCalculationService _taxService;

        public InvestmentService(
            IInvestmentRepository repository,
            IGainCalculationService gainService,
            ITaxCalculationService taxService)
        {
            _repository = repository;
            _gainService = gainService;
            _taxService = taxService;
        }

        public async Task<Investment> CreateInvestmentAsync(string ownerId, decimal amount, DateTime creationDate)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                throw new ArgumentException("OwnerId é obrigatório.");
            if (amount <= 0)
                throw new ArgumentException("O valor do investimento deve ser maior que zero.");
            if (creationDate > DateTime.Today)
                throw new ArgumentException("A data de criação não pode ser futura.");

            var investment = new Investment
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                InitialAmount = amount,
                CreationDate = creationDate
            };

            await _repository.AddAsync(investment);
            return investment;
        }

        public async Task WithdrawInvestmentAsync(Guid investmentId, DateTime withdrawalDate)
        {
            var investment = await _repository.GetByIdAsync(investmentId);
            if (investment == null)
                throw new InvalidOperationException("Investimento não encontrado.");

            if (investment.IsWithdrawn)
                throw new InvalidOperationException("Investimento já foi resgatado.");

            if (withdrawalDate < investment.CreationDate)
                throw new ArgumentException("A data de resgate não pode ser anterior à data de criação.");
            if (withdrawalDate > DateTime.Today)
                throw new ArgumentException("A data de resgate não pode ser futura.");

            investment.WithdrawalDate = withdrawalDate;

            await _repository.UpdateAsync(investment);
        }

        public async Task<Investment?> GetInvestmentByIdAsync(Guid id)
            => await _repository.GetByIdAsync(id);

        public async Task<List<Investment>> GetInvestmentsByOwnerAsync(string ownerId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            return (await _repository.GetByOwnerIdAsync(ownerId, page, pageSize)).ToList();
        }
    }
}