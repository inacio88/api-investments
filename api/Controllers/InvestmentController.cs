using application.DTOs;
using application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using common.TypeExtentions;

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

        /// <summary>
        /// Creates a new investment.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<InvestmentDto>> CreateInvestment([FromBody] CreateInvestmentInput input)
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

        // /// <summary>
        // /// Retrieves an investment by ID.
        // /// </summary>
        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<InvestmentDto>> GetInvestmentById(Guid id)
        {
            var userId = User.UserId();
            var investment = await _investmentService.GetInvestmentByIdAsync(id);
            if (investment == null)
                return NotFound();

            return Ok(investment);
        }

        /// <summary>
        /// Lists all investments for a given owner (with pagination).
        /// </summary>
        [Authorize]
        [HttpGet("owner")]
        public async Task<ActionResult<PaginatedResult<InvestmentDto>>> GetInvestmentsByOwner([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.UserId();
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Usuário não autenticado.");

            var result = await _investmentService.GetInvestmentsByOwnerAsync(userId, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Withdraws an investment (full withdrawal only).
        /// </summary>
        [Authorize]
        [HttpPost("{id:guid}/withdraw")]
        public async Task<IActionResult> WithdrawInvestment(Guid id, [FromBody] WithdrawInvestmentInput input)
        {
            var userId = User.UserId();

            try
            {
                await _investmentService.WithdrawInvestmentAsync(id, input.WithdrawalDate);
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