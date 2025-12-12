using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Ordonnance
/// </summary>
public class OrdonnanceConfiguration : IEntityTypeConfiguration<Ordonnance>
{
    public void Configure(EntityTypeBuilder<Ordonnance> builder)
    {
        // Nom de la table
        builder.ToTable("Ordonnances");
        
        // Clé primaire
        builder.HasKey(o => o.Id);
        
        // Configuration de l'Id
        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();
        
        // Configuration Date
        builder.Property(o => o.Date)
            .IsRequired()
            .HasColumnType("date");
        
        // Configuration Notes
        builder.Property(o => o.Notes)
            .HasMaxLength(255)
            .IsUnicode(true);
        
        // Les relations Many-to-One vers Doctor et Patient sont déjà définies
        // La relation vers Consultation est définie dans ConsultationConfiguration
        
        // Relation One-to-Many: Une ordonnance -> Plusieurs lignes
        builder.HasMany(o => o.OrdonnanceLignes)
            .WithOne(ol => ol.Ordonnance)
            .HasForeignKey(ol => ol.OrdonnanceId)
            .OnDelete(DeleteBehavior.Cascade);  // Suppression en cascade des lignes
    }
}
