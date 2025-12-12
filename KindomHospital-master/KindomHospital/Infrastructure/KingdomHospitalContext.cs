using KingdomHospital.Domain.Entities;
using KingdomHospital.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure;

/// <summary>
/// Contexte de base de données pour Kingdom Hospital
/// Selon le cours: DbContext représente une session avec la base de données
/// </summary>
public class KingdomHospitalContext : DbContext
{
    public KingdomHospitalContext(DbContextOptions<KingdomHospitalContext> options)
        : base(options)
    {
    }

    // DbSet représentent les tables de la base de données
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Consultation> Consultations { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Ordonnance> Ordonnances { get; set; }
    public DbSet<OrdonnanceLigne> OrdonnanceLignes { get; set; }

    /// <summary>
    /// Configuration du modèle via Fluent API
    /// Selon le cours: OnModelCreating permet de configurer les entités avec Fluent API
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Application de toutes les configurations : donc jindique a EF cmt configurer les tables
        // Selon le cours: ApplyConfiguration applique les configurations IEntityTypeConfiguration
        modelBuilder.ApplyConfiguration(new SpecialtyConfiguration());
        modelBuilder.ApplyConfiguration(new DoctorConfiguration());
        modelBuilder.ApplyConfiguration(new PatientConfiguration());
        modelBuilder.ApplyConfiguration(new ConsultationConfiguration());
        modelBuilder.ApplyConfiguration(new MedicamentConfiguration());
        modelBuilder.ApplyConfiguration(new OrdonnanceConfiguration());
        modelBuilder.ApplyConfiguration(new OrdonnanceLigneConfiguration());
    }
}
