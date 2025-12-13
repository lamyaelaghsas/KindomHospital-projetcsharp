using KingdomHospital.Application.DTOs;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

/// <summary>
/// Mapper pour Medicament
/// Selon le cours: Mapperly pour le mapping performant (slide 223-225)
/// </summary>
[Mapper]
public partial class MedicamentMapper
{
    /// <summary>
    /// Convertit un Medicament vers MedicamentDto
    /// </summary>
    public partial MedicamentDto ToDto(Medicament entity);

    /// <summary>
    /// Convertit pour la liste (version allégée)
    /// </summary>
    public MedicamentListDto ToListDto(Medicament entity)
    {
        return new MedicamentListDto(
            entity.Id,
            entity.Name,
            entity.DosageForm,
            entity.Strength
        );
    }

    /// <summary>
    /// Convertit un DTO de création vers une entité
    /// </summary>
    public partial Medicament ToEntity(MedicamentCreateDto dto);
}