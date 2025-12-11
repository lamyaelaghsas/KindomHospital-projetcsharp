using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Domain.Entities;

namespace KingdomHospital.Application.Services;

public class WeatherService(WeatherMapper mapper, ILogger<WeatherService> logger)
{
    public IEnumerable<WeatherDto> Get()
    {
        logger.LogInformation("Getting weather forecasts");
        var ar =  Enumerable.Range(1, 5).Select(index => new WeatherForecast()
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
        .ToArray();
        return ar.Select(w => mapper.ToDto(w)).ToList();
    }
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];
}