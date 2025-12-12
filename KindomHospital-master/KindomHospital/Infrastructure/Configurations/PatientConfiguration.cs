using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Patient
/// </summary>
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        // Nom de la table
        builder.ToTable("Patients");
        
        // Clé primaire
        builder.HasKey(p => p.Id);
        
        // Configuration de l'Id
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();
        
        // Configuration FirstName
        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(true);
        
        // Configuration LastName
        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(true);
        
        // Configuration BirthDate - mapper DateOnly vers date SQL
        builder.Property(p => p.BirthDate)
            .IsRequired()
            .HasColumnType("date");  // Type SQL Server pour DateOnly
        
        // Index de recherche sur (LastName, FirstName, BirthDate)
        builder.HasIndex(p => new { p.LastName, p.FirstName, p.BirthDate })
            .HasDatabaseName("IX_Patient_LastName_FirstName_BirthDate");
        
        // Relation One-to-Many: Un patient -> Plusieurs consultations
        builder.HasMany(p => p.Consultations)
            .WithOne(c => c.Patient)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);  // Traçabilité historique
        
        // Relation One-to-Many: Un patient -> Plusieurs ordonnances
        builder.HasMany(p => p.Ordonnances)
            .WithOne(o => o.Patient)
            .HasForeignKey(o => o.PatientId)
            .OnDelete(DeleteBehavior.Restrict);  // Traçabilité historique
    }
}
