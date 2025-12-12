using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Doctor
/// </summary>
public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        // Nom de la table
        builder.ToTable("Doctors");
        
        // Clé primaire
        builder.HasKey(d => d.Id);
        
        // Configuration de l'Id
        builder.Property(d => d.Id)
            .ValueGeneratedOnAdd();
        
        // Configuration FirstName
        builder.Property(d => d.FirstName)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(true);
        
        // Configuration LastName
        builder.Property(d => d.LastName)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(true);
        
        // Index de recherche sur (LastName, FirstName) pour optimiser les requêtes
        builder.HasIndex(d => new { d.LastName, d.FirstName })
            .HasDatabaseName("IX_Doctor_LastName_FirstName");
        
        // Relation Many-to-One: Plusieurs médecins -> Une spécialité
        // (déjà définie dans SpecialtyConfiguration)
        
        // Relation One-to-Many: Un médecin -> Plusieurs consultations
        builder.HasMany(d => d.Consultations)
            .WithOne(c => c.Doctor)
            .HasForeignKey(c => c.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);  // Empêche la suppression si consultations existent
        
        // Relation One-to-Many: Un médecin -> Plusieurs ordonnances
        builder.HasMany(d => d.Ordonnances)
            .WithOne(o => o.Doctor)
            .HasForeignKey(o => o.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);  // Empêche la suppression si ordonnances existent
    }
}
