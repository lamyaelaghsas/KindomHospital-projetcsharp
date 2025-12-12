using KingdomHospital.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations
{
    public class WeatherForecastConfiguration : IEntityTypeConfiguration<WeatherForecast>
    {
        public void Configure(EntityTypeBuilder<WeatherForecast> builder)
        {
            builder.ToTable("WeatherForecasts");
            builder.Haskey(w=>w.Id);
            builder.Property(w => w.Id).ValueGeneratedOnAdd();

            //Map DateOnly to date column
            builder.Property(w => w.Date)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(w => w.TemperatureC)
                .IsRequired();

            builder.Property(w => w.Summary)
                .HasMaxLength(200)
                .IsUnicode(false);

            //Relationship to Region
            builder.HasOne(w => w.Region)
                .WithMany(r => r.WeatherForecasts)
                .HasForeignKey(w => RegionId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
