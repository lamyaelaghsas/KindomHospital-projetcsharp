using Microsoft.OpenApi.Validations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSerilog((services, lc) =>
    lc.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Ajouter les Mappers au DI
builder.Services.AddSingleton<KindomHospital.Application.Mappers.WeatherMapper>();
//Ajouter les service au DI
builder.Services.AddScoped<KindomHospital.Application.Services.WeatherService>();
//Ajouter les repositories au DI
var connectionString = builder.Configuration.GetConnectionString("HospitalConnection");
builder.Services.AddDbContext<KindomHospitalContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();

var app = builder.Build();

//Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<KindomHospitalContext>();
    SeedData.Initialize(db);
}

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
