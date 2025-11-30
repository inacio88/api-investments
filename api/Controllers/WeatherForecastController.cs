using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild",
            "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <remarks>
        /// Exemplo de chamada:
        ///
        ///     GET /WeatherForecast?days=5
        ///
        /// </remarks>
        [Authorize]
        [HttpGet(Name = "GetWeatherForecast")]
        [SwaggerOperation(
            Summary = "Obtém previsões do tempo",
            Description = "Retorna dados aleatórios de previsão dos próximos dias.",
            OperationId = "GetWeatherForecast",
            Tags = new[] { "Weather" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Previsão retornada com sucesso", typeof(IEnumerable<WeatherForecast>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Requer autenticação")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError,"Erro interno no servidor")]
        public IEnumerable<WeatherForecast> Get( [SwaggerParameter("Número de dias de previsão entre 1 e 14")] int days = 5)
        {
            if (days < 1 || days > 14)
                throw new ArgumentOutOfRangeException(nameof(days),
                    "O número de dias deve ser entre 1 e 14.");

            return Enumerable.Range(1, days).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

    }

    public class WeatherForecast
    {
        /// <example>2025-01-30</example>
        public DateOnly Date { get; set; }

        /// <example>23</example>
        public int TemperatureC { get; set; }

        /// <example>73</example>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <example>Mild</example>
        public string? Summary { get; set; }
    }
}
