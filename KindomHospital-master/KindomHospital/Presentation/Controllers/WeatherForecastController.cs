using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingdomHospital.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(WeatherService service, ILogger<WeatherForecastController> logger) : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet(Name = "GetWeatherForecast")]
        public ActionResult<IEnumerable<WeatherDto>> Get()
        {
            return service.Get().ToList();
        }
    }
}
