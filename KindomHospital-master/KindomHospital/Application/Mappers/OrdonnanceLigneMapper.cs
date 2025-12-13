using KingdomHospital.Application.DTOs;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

/// <summary>
/// Mapper pour OrdonnanceLigne
/// Selon le cours: Mapperly pour le mapping performant (slide 223-225)
/// </summary>
[Mapper]
public partial class OrdonnanceLigneMapper
{
    /// <summary>
    /// Convertit une OrdonnanceLigne vers DTO
    /// </summary>
    public OrdonnanceLigneDto ToDto(OrdonnanceLigne entity)
    {
        return new OrdonnanceLigneDto(
            entity.Id,
            entity.MedicamentId,
            entity.Medicament?.Name ?? "Non défini",
            entity.Dosage,
            entity.Frequency,
            entity.Duration,
            entity.Quantity,
            entity.Instructions
        );
    }

    /// <summary>
    /// Convertit un DTO de création vers une entité
    /// </summary>
    public OrdonnanceLigne ToEntity(OrdonnanceLigneCreateDto dto)
    {
        return new OrdonnanceLigne
        {
            MedicamentId = dto.MedicamentId,
            Dosage = dto.Dosage,
            Frequency = dto.Frequency,
            Duration = dto.Duration,
            Quantity = dto.Quantity,
            Instructions = dto.Instructions
        };
    }

    /// <summary>
    /// Met à jour une entité existante
    /// </summary>
    public void UpdateEntity(OrdonnanceLigneUpdateDto dto, OrdonnanceLigne entity)
    {
        entity.MedicamentId = dto.MedicamentId;
        entity.Dosage = dto.Dosage;
        entity.Frequency = dto.Frequency;
        entity.Duration = dto.Duration;
        entity.Quantity = dto.Quantity;
        entity.Instructions = dto.Instructions;
    }
}