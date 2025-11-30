using application.DTOs;
using application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using common.TypeExtentions;
using core.Filters;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvestmentController : ControllerBase
    {
        private readonly IInvestmentService _investmentService;

        public InvestmentController(IInvestmentService investmentService)
        {
            _investmentService = investmentService;
        }

        /// <remarks>
        /// ### Exemplo de requisição
        /// ```json
        /// {
        ///   "amount": 1500.00,
        ///   "creationDate": "2023-04-10"
        /// }
        /// ```
        ///
        /// ### Regras:
        /// - O valor inicial **não pode ser negativo**
        /// - A data de criação pode ser **hoje ou no passado**
        /// - O dono é atribuído internamente a partir do contexto do usuário autenticado
        ///
        /// ### Ganhos:
        /// - O investimento rende **0.52% ao mês**
        /// - Regra de **juros compostos**
        /// </remarks>
        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Cria um novo investimento",
            Description = "Cria um investimento definindo dono, data de criação e valor inicial.",
            OperationId = "CreateInvestment",
            Tags = new[] { "Investments" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Investimento criado com sucesso", typeof(InvestmentDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro de validação ou dados inválidos")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado")]
        public async Task<ActionResult<InvestmentDto>> CreateInvestment(
            [FromBody, SwaggerRequestBody("Dados para criação de um novo investimento")] CreateInvestmentInput input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.UserId();
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Usuário não autenticado.");

            input.SetOwner(userId);

            try
            {
                var investment = await _investmentService.CreateInvestmentAsync(input);
                return Ok(investment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <remarks>
        /// ### Retorno inclui:
        /// - Valor inicial  
        /// - Data de criação  
        /// - Saldo esperado (valor + ganhos)  
        /// - Data de retirada (quando existir)  
        /// - Valor líquido recebido após impostos (se retirado)  
        ///
        /// ### Regras de ganhos:
        /// - 0.52% ao mês  
        /// - Juros compostos  
        ///
        /// Se o investimento já foi retirado:
        /// - O saldo reflete somente os ganhos até a data do saque  
        /// - O campo **NetWithdrawAmount** no retorno representa o valor final após impostos, só é preenchido caso tenha sido retirado.
        /// </remarks>
        [Authorize]
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Obtém um investimento pelo ID",
            Description = "Retorna os detalhes completos do investimento.",
            OperationId = "GetInvestmentById",
            Tags = new[] { "Investments" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Investimento encontrado", typeof(InvestmentDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Investimento não encontrado")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado")]
        public async Task<ActionResult<InvestmentDto>> GetInvestmentById(Guid id)
        {
            var userId = User.UserId();
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Usuário não autenticado.");

            var investment = await _investmentService.GetInvestmentByIdAsync(
                new core.Filters.InvestmentFilter(userId) { Id = id });

            if (investment == null)
                return NotFound();

            return Ok(investment);
        }

        /// <remarks>
        /// ### Exemplo:
        ///
        ///     GET /api/investment/owner?page=1&amp;pageSize=10
        ///
        /// ### Detalhes:
        /// - Lista **todos os investimentos do usuário atual**
        /// - Resposta é **paginada**
        /// - Inclui total de itens e total de páginas
        /// </remarks>
        [Authorize]
        [HttpGet("owner")]
        [SwaggerOperation(
            Summary = "Lista investimentos do usuário",
            Description = "Retorna uma lista paginada de investimentos pertencentes ao usuário atual.",
            OperationId = "GetInvestmentsByOwner",
            Tags = new[] { "Investments" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Lista retornada com sucesso", typeof(PaginatedResult<InvestmentDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado")]
        public async Task<ActionResult<PaginatedResult<InvestmentDto>>> GetInvestmentsByOwner(
            [FromQuery, SwaggerParameter("Página atual (mínimo 1)")] int page = 1,
            [FromQuery, SwaggerParameter("Quantidade de itens por página")] int pageSize = 10)
        {
            var userId = User.UserId();
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Usuário não autenticado.");

            var result = await _investmentService.GetInvestmentsByOwnerAsync(
                new InvestmentFilter(userId) { Page = page, PageSize = pageSize });

            return Ok(result);
        }

        // ---------------------------------------------------------
        // POST /api/investment/{id}/withdraw
        // Withdraw investment
        // ---------------------------------------------------------

        /// <remarks>
        /// ### Exemplo de requisição
        /// ```json
        /// {
        ///   "withdrawalDate": "2025-11-30T18:11:06.825Z"
        /// }
        /// ```
        ///
        /// ### Regras:
        /// - O saque é sempre **total (100%)**  
        /// - A data de saque não pode ser:  
        ///   - antes da criação  
        ///   - no futuro  
        ///
        /// ### Tributação aplicada ao ganho:
        /// - Menos de 1 ano → **22.5%**  
        /// - Entre 1 e 2 anos → **18.5%**  
        /// - Mais de 2 anos → **15%**  
        ///
        /// </remarks>
        [Authorize]
        [HttpPost("{id:guid}/withdraw")]
        [SwaggerOperation(
            Summary = "Realiza o saque de um investimento",
            Description = "Efetua o saque total e aplica impostos sobre os ganhos conforme regras de idade do investimento.",
            OperationId = "WithdrawInvestment",
            Tags = new[] { "Investments" }
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Saque realizado com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Regras de data inválidas ou investimento já retirado")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário não autenticado")]
        public async Task<IActionResult> WithdrawInvestment(
            Guid id,
            [FromBody, SwaggerRequestBody("Data de retirada do investimento")] WithdrawInvestmentInput input)
        {
            var userId = User.UserId();
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Usuário não autenticado.");

            try
            {
                await _investmentService.WithdrawInvestmentAsync(id, userId, input.WithdrawalDate);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
