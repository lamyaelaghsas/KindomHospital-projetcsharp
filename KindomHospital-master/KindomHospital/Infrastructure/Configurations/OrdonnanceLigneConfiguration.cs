using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité OrdonnanceLigne
/// </summary>
public class OrdonnanceLigneConfiguration : IEntityTypeConfiguration<OrdonnanceLigne>
{
    public void Configure(EntityTypeBuilder<OrdonnanceLigne> builder)
    {
        // Nom de la table
        builder.ToTable("OrdonnanceLignes");
        
        // Clé primaire
        builder.HasKey(ol => ol.Id);
        
        // Configuration de l'Id
        builder.Property(ol => ol.Id)
            .ValueGeneratedOnAdd();
        
        // Configuration Dosage
        builder.Property(ol => ol.Dosage)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);
        
        // Configuration Frequency
        builder.Property(ol => ol.Frequency)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(true);
        
        // Configuration Duration
        builder.Property(ol => ol.Duration)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(true);
        
        // Configuration Quantity - doit être > 0
        builder.Property(ol => ol.Quantity)
            .IsRequired();
        
        // Configuration Instructions
        builder.Property(ol => ol.Instructions)
            .HasMaxLength(255)
            .IsUnicode(true);
        
        // Index pour éviter les doublons exacts dans une même ordonnance
        builder.HasIndex(ol => new { 
            ol.OrdonnanceId, 
            ol.MedicamentId, 
            ol.Dosage, 
            ol.Frequency, 
            ol.Duration 
        })
        .IsUnique()
        .HasDatabaseName("IX_OrdonnanceLigne_Unique");
        
        // Les relations sont déjà définies dans OrdonnanceConfiguration et MedicamentConfiguration
    }
}
