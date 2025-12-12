using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Consultation
/// </summary>
public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
{
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        // Nom de la table
        builder.ToTable("Consultations");

        // Clé primaire
        builder.HasKey(c => c.Id);

        // Configuration de l'Id
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        // Configuration Date - mapper DateOnly vers date SQL
        builder.Property(c => c.Date)
            .IsRequired()
            .HasColumnType("date");

        // Configuration Hour - mapper TimeOnly vers time SQL
        builder.Property(c => c.Hour)
            .IsRequired()
            .HasColumnType("time");

        // Configuration Reason (optionnel)
        builder.Property(c => c.Reason)
            .HasMaxLength(100)
            .IsUnicode(true);

        // Index unique pour éviter le double-booking
        // Un médecin ne peut pas avoir 2 consultations au même moment
        builder.HasIndex(c => new { c.DoctorId, c.Date, c.Hour })
            .IsUnique()
            .HasDatabaseName("IX_Consultation_Doctor_Date_Hour");

        // Relation Many-to-One: Plusieurs consultations -> Un médecin = chaque docteur peut avoir plusieurs consultations
        builder.HasOne(c => c.Doctor)
            .WithMany(d => d.Consultations)
            .HasForeignKey(c => c.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);  // Empêche la suppression du médecin

        // Relation Many-to-One: Plusieurs consultations -> Un patient
        builder.HasOne(c => c.Patient)
            .WithMany(p => p.Consultations)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);  // Empêche la suppression du patient

        // Relation One-to-Many: Une consultation -> Plusieurs ordonnances = une consultation peut avoir plusieurs ordonnances
        builder.HasMany(c => c.Ordonnances)
            .WithOne(o => o.Consultation)
            .HasForeignKey(o => o.ConsultationId)
            .OnDelete(DeleteBehavior.SetNull);  // Si consultation supprimée, ordonnance garde les données mais perd le lien
    }
}