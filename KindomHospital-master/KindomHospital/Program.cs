using KingdomHospital.Application.Mappers;
using KingdomHospital.Application.Services;
using KingdomHospital.Infrastructure;
using KingdomHospital.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURATION SERILOG =====
// Selon le cours: Serilog pour le logging avancé
builder.Services.AddSerilog((services, lc) =>
    lc.ReadFrom.Configuration(builder.Configuration));

// ===== CONFIGURATION CONTROLLERS =====
// Selon le cours: AddControllers pour enregistrer les contrôleurs
builder.Services.AddControllers();

// ===== CONFIGURATION OPENAPI & SWAGGER =====
// Selon le cours: OpenAPI pour la documentation de l'API
builder.Services.AddOpenApi();

// ===== CONFIGURATION ENTITY FRAMEWORK =====
// Selon le cours: AddDbContext pour configurer EF Core avec SQL Server
builder.Services.AddDbContext<KingdomHospitalContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== INJECTION DE DÉPENDANCES - MAPPERS =====
// Selon le cours: Mappers en Transient (stateless)
builder.Services.AddTransient<SpecialtyMapper>();
builder.Services.AddTransient<DoctorMapper>();
builder.Services.AddTransient<PatientMapper>();
builder.Services.AddTransient<ConsultationMapper>();
builder.Services.AddTransient<MedicamentMapper>();
builder.Services.AddTransient<OrdonnanceMapper>();
builder.Services.AddTransient<OrdonnanceLigneMapper>();

// ===== INJECTION DE DÉPENDANCES - REPOSITORIES =====
// Selon le cours: Repositories en Scoped (durée de vie de la requête)
builder.Services.AddScoped<SpecialtyRepository>();
builder.Services.AddScoped<DoctorRepository>();
builder.Services.AddScoped<PatientRepository>();
builder.Services.AddScoped<ConsultationRepository>();
builder.Services.AddScoped<MedicamentRepository>();
builder.Services.AddScoped<OrdonnanceRepository>();
builder.Services.AddScoped<OrdonnanceLigneRepository>();

// ===== INJECTION DE DÉPENDANCES - SERVICES =====
// Selon le cours: Services en Scoped
builder.Services.AddScoped<SpecialtyService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<ConsultationService>();
builder.Services.AddScoped<MedicamentService>();
builder.Services.AddScoped<OrdonnanceService>();
builder.Services.AddScoped<OrdonnanceLigneService>();

var app = builder.Build();

// ===== CONFIGURATION DU PIPELINE HTTP =====
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference(); // UI moderne pour tester l'API
}

// Selon le cours: UseSerilogRequestLogging pour logger les requêtes HTTP
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthorization();

// Selon le cours: MapControllers pour mapper les routes des contrôleurs
app.MapControllers();

// ===== INITIALISATION DES DONNÉES =====
// Selon le cours: Seed Data pour avoir des données par défaut
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<KingdomHospitalContext>();

    // Appliquer les migrations automatiquement en développement
    if (app.Environment.IsDevelopment())
    {
        context.Database.Migrate();
    }

    // Initialiser les données de démonstration
    SeedData.Initialize(context);
}

app.Run();