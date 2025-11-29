using core.Entities;
using core.Repositories;
using core.Services;
using application.DTOs;
using common.TypeExtentions;

namespace application.Services;

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

    public async Task<InvestmentDto> CreateInvestmentAsync(CreateInvestmentInput input)
    {
        ValidateCreateInput(input.OwnerId, input.Amount, input.CreationDate);

        var investment = new Investment
        {
            Id = Guid.NewGuid(),
            OwnerId = input.OwnerId,
            InitialAmount = input.Amount,
            CreationDate = input.CreationDate.ToUniversalTime()
        };

        await _repository.AddAsync(investment);
        return MapToDtoAsync(investment);
    }

    public async Task WithdrawInvestmentAsync(Guid investmentId, DateTime withdrawalDate)
    {
        withdrawalDate = withdrawalDate.ToUniversalTime();
        var investment = await _repository.GetByIdAsync(investmentId) ?? throw new InvalidOperationException("Investimento não encontrado.");
        if (investment.IsWithdrawn)
            throw new InvalidOperationException("Investimento já foi resgatado.");

        if (withdrawalDate.IsDateLessThan(investment.CreationDate))
            throw new ArgumentException("A data de resgate não pode ser anterior à data de criação.");
        if (withdrawalDate.IsDateGreaterThan(DateTime.Today))
            throw new ArgumentException("A data de resgate não pode ser futura.");

        investment.WithdrawalDate = withdrawalDate;
        await _repository.UpdateAsync(investment);
    }

    public async Task<InvestmentDto?> GetInvestmentByIdAsync(Guid id)
    {
        var investment = await _repository.GetByIdAsync(id);
        return investment == null ? null : MapToDtoAsync(investment);
    }

    public async Task<PaginatedResult<InvestmentDto>> GetInvestmentsByOwnerAsync(string ownerId, int page, int pageSize)
    {
        if (string.IsNullOrWhiteSpace(ownerId))
            throw new ArgumentException("OwnerId é obrigatório.");


        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var investmentsTask = _repository.GetByOwnerIdAsync(ownerId, page, pageSize);
        var totalCountTask = _repository.CountByOwnerIdAsync(ownerId);
        await Task.WhenAll(investmentsTask, totalCountTask);
        var investments = investmentsTask.Result;
        var totalCount = totalCountTask.Result;
        var items = new List<InvestmentDto>();
        foreach (var investment in investments)
        {
            items.Add(MapToDtoAsync(investment));
        }

        return new PaginatedResult<InvestmentDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    private static void ValidateCreateInput(string ownerId, decimal amount, DateTime creationDate)
    {
        if (string.IsNullOrWhiteSpace(ownerId))
            throw new ArgumentException("OwnerId é obrigatório.");
        if (amount <= 0)
            throw new ArgumentException("O valor do investimento deve ser maior que zero.");
        if (creationDate.IsDateGreaterThan(DateTime.Today))
            throw new ArgumentException("A data de criação não pode ser futura.");
    }

    private InvestmentDto MapToDtoAsync(Investment investment)
    {
        DateTime calculationDate = investment.IsWithdrawn
            ? investment.WithdrawalDate!.Value.ToUniversalTime()
            : DateTime.Today.ToUniversalTime();

        decimal gains = _gainService.CalculateGains(
            investment.InitialAmount,
            investment.CreationDate,
            calculationDate
        );

        decimal expectedBalance = investment.InitialAmount + gains;
        decimal? netWithdrawAmount = null;

        if (investment.IsWithdrawn)
        {
            decimal tax = _taxService.CalculateTax(
                gains,
                investment.CreationDate,
                investment.WithdrawalDate!.Value
            );
            netWithdrawAmount = expectedBalance - tax;
        }

        return new InvestmentDto
        {
            Id = investment.Id,
            OwnerId = investment.OwnerId,
            CreationDate = investment.CreationDate,
            InitialAmount = investment.InitialAmount,
            WithdrawalDate = investment.WithdrawalDate,
            ExpectedBalance = Math.Round(expectedBalance, 2),
            NetWithdrawAmount = netWithdrawAmount.HasValue ? Math.Round(netWithdrawAmount.Value, 2) : null
        };
    }
}