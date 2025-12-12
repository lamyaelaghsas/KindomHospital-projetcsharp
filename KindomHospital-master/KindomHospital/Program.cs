using KingdomHospital.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ========== CONFIGURATION DES SERVICES (Builder Phase) ==========

// 1. Configuration Serilog pour le logging
// Selon le cours: Serilog offre un logging structuré avancé
builder.Services.AddSerilog((services, lc) =>
    lc.ReadFrom.Configuration(builder.Configuration));

// 2. Ajout des contrôleurs
// Selon le cours: AddControllers enregistre les services pour les contrôleurs API
builder.Services.AddControllers();

// 3. Configuration OpenAPI/Swagger
// Selon le cours: OpenAPI génère la documentation automatique de l'API
builder.Services.AddOpenApi();

// 4. Configuration du DbContext avec SQL Server
// Selon le cours: AddDbContext enregistre le contexte EF Core avec injection de dépendances
var connectionString = builder.Configuration.GetConnectionString("HospitalConnection");
builder.Services.AddDbContext<KingdomHospitalContext>(options =>
    options.UseSqlServer(connectionString));

// TODO: Ajouter les Repositories (Scoped)
// TODO: Ajouter les Mappers (Transient)
// TODO: Ajouter les Services (Scoped)

var app = builder.Build();

// ========== CONFIGURATION DU PIPELINE HTTP (Application Phase) ==========

// 1. Initialisation des données (Seed)
// Selon le cours: Seed Data permet d'avoir des données de démonstration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<KingdomHospitalContext>();

    // Appliquer les migrations automatiquement en développement
    if (app.Environment.IsDevelopment())
    {
        context.Database.Migrate();
    }

    SeedData.Initialize(context);
}

// 2. Logging des requêtes HTTP avec Serilog
app.UseSerilogRequestLogging();

// 3. Configuration OpenAPI en développement
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Scalar UI - interface moderne pour tester l'API
    // Accessible à: https://localhost:7006/scalar/v1
    app.MapScalarApiReference();
}

// 4. Redirection HTTPS
app.UseHttpsRedirection();

// 5. Autorisation
app.UseAuthorization();

// 6. Mapping des contrôleurs
// Selon le cours: MapControllers établit les routes vers les contrôleurs
app.MapControllers();

// Démarrage de l'application
app.Run();
