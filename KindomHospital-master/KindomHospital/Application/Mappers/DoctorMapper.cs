using KingdomHospital.Application.DTOs;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

/// <summary>
/// Mapper pour Doctor avec mapping manuel des propriétés complexes
/// Selon le cours: Mapperly pour le mapping performant (slide 223-225)
/// </summary>
[Mapper]
public partial class DoctorMapper
{
    /// <summary>
    /// Convertit un Doctor (avec Specialty chargée) vers DoctorDto
    /// Mapping manuel nécessaire pour SpecialtyName
    /// </summary>
    public DoctorDto ToDto(Doctor entity)
    {
        return new DoctorDto(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.SpecialtyID,
            entity.Specialty?.Name ?? "Non définie"
        );
    }

    /// <summary>
    /// Convertit pour la liste (plus léger)
    /// </summary>
    public DoctorListDto ToListDto(Doctor entity)
    {
        return new DoctorListDto(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.Specialty?.Name ?? "Non définie"
        );
    }

    /// <summary>
    /// Convertit un DTO de création vers une entité
    /// </summary>
    public partial Doctor ToEntity(DoctorCreateDto dto);

    /// <summary>
    /// Met à jour une entité existante avec les données du DTO
    /// </summary>
    public void UpdateEntity(DoctorUpdateDto dto, Doctor entity)
    {
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.SpecialtyID = dto.SpecialtyId;
    }
}