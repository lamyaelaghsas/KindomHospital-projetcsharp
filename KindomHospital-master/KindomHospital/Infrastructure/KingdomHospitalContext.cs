using KingdomHospital.Domain.Entities;
using KingdomHospital.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure;

public class KindomHospitalContext(BdContextOptions<KindomHospitalContext> options) : DbContext(options)
{
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    public DbSet<Region> Regions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new WeatherForecastConfiguration());
        modelBuilder.ApplyConfiguration(new RegionConfiguration());
        
    }
}
