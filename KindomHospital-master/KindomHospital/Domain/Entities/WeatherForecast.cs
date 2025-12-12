namespace KingdomHospital.Domain.Entities;

public class WeatherForecast
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }

    //Foreign key to Region
    public int RegionId { get; set; }
    public Region? Region { get; set; }
}