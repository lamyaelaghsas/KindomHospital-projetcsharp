using KingdomHospital.Application.DTOs;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

/// Mapper pour convertir entre Specialty (entité) et SpecialtyDto

[Mapper]
public partial class SpecialtyMapper
{

    public partial SpecialtyDto ToDto(Specialty entity);

    /// <summary>
    /// Convertit un DTO de création vers une entité
    /// Mapperly génère: new Specialty { Name = dto.Name }
    /// </summary>
    public partial Specialty ToEntity(SpecialtyCreateDto dto);
}