using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KingdomHospital.Infrastructure.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Medicament
/// </summary>
public class MedicamentConfiguration : IEntityTypeConfiguration<Medicament>
{
    public void Configure(EntityTypeBuilder<Medicament> builder)
    {
        // Nom de la table
        builder.ToTable("Medicaments");
        
        // Clé primaire
        builder.HasKey(m => m.Id);
        
        // Configuration de l'Id
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();
        
        // Configuration Name
        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(true);
        
        // Configuration DosageForm (forme galénique)
        builder.Property(m => m.DosageForm)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(true);
        
        // Configuration Strength (dosage)
        builder.Property(m => m.Strength)
            .IsRequired()
            .HasMaxLength(30)
            .IsUnicode(false);  // Généralement des chiffres et unités
        
        // Configuration AtcCode
        builder.Property(m => m.AtcCode)
            .HasMaxLength(20)
            .IsUnicode(false);
        
        // Index unique pour éviter les doublons
        // Un médicament est unique par sa combinaison Name + DosageForm + Strength
        builder.HasIndex(m => new { m.Name, m.DosageForm, m.Strength })
            .IsUnique()
            .HasDatabaseName("IX_Medicament_Name_Form_Strength");
        
        // Relation One-to-Many: Un médicament -> Plusieurs lignes d'ordonnance
        builder.HasMany(m => m.OrdonnanceLignes)
            .WithOne(ol => ol.Medicament)
            .HasForeignKey(ol => ol.MedicamentId)
            .OnDelete(DeleteBehavior.Restrict);  // Protection de l'historique
    }
}
