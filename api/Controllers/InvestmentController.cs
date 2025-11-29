using application.DTOs;
using application.Services;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost]
        public async Task<ActionResult<InvestmentDto>> CreateInvestment([FromBody] CreateInvestmentInput input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var investment = await _investmentService.CreateInvestmentAsync(input);
                return CreatedAtAction(nameof(GetInvestmentById), new { id = investment.Id }, investment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves an investment by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<InvestmentDto>> GetInvestmentById(Guid id)
        {
            var investment = await _investmentService.GetInvestmentByIdAsync(id);
            if (investment == null)
                return NotFound();

            return Ok(investment);
        }

        /// <summary>
        /// Lists all investments for a given owner (with pagination).
        /// </summary>
        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<PaginatedResult<InvestmentDto>>> GetInvestmentsByOwner(
            string ownerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                return BadRequest("OwnerId is required.");

            var result = await _investmentService.GetInvestmentsByOwnerAsync(ownerId, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Withdraws an investment (full withdrawal only).
        /// </summary>
        [HttpPost("{id:guid}/withdraw")]
        public async Task<IActionResult> WithdrawInvestment(Guid id, [FromBody] WithdrawInvestmentInput input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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