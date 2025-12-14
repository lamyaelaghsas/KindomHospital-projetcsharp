using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Specialty
/// Selon le cours: utilisation de IEntityTypeConfiguration pour séparer les configurations
/// </summary>
public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
{
    public void Configure(EntityTypeBuilder<Specialty> builder)
    {
        // Nom de la table
        builder.ToTable("Specialties");
        
        // Clé primaire
        builder.HasKey(s => s.Id);
        
        // Configuration de l'Id - auto-incrémenté
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();
        
        // Configuration du Name
        builder.Property(s => s.Name)
            .IsRequired()                    // Required
            .HasMaxLength(30)                // MaxLength: 30
            .IsUnicode(true);                // Supporte les caractères unicode
        
        // Index unique sur Name - une spécialité ne peut exister qu'une fois
        builder.HasIndex(s => s.Name)
            .IsUnique()
            .HasDatabaseName("IX_Specialty_Name");
        
        // Relation One-to-Many: Une spécialité -> Plusieurs médecins
        builder.HasMany(s => s.Doctors)
            .WithOne(d => d.Specialty)
            .HasForeignKey(d => d.Id)
            .OnDelete(DeleteBehavior.Restrict);  // Empêche la suppression si des médecins sont liés
    }
}
