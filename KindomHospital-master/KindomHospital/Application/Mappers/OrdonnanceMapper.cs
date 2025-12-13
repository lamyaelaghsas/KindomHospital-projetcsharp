using KingdomHospital.Application.DTOs;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

/// <summary>
/// Mapper pour Ordonnance
/// Selon le cours: Mapperly pour le mapping performant (slide 223-225)
/// </summary>
[Mapper]
public partial class OrdonnanceMapper
{
    // Injection du mapper de lignes
    private readonly OrdonnanceLigneMapper _ligneMapper = new();

    /// <summary>
    /// Convertit une Ordonnance complète vers DTO
    /// </summary>
    public OrdonnanceDto ToDto(Ordonnance entity)
    {
        return new OrdonnanceDto(
            entity.Id,
            entity.DoctorId,
            $"{entity.Doctor?.FirstName} {entity.Doctor?.LastName}",
            entity.PatientId,
            $"{entity.Patient?.FirstName} {entity.Patient?.LastName}",
            entity.ConsultationId,
            entity.Date,
            entity.Notes,
            entity.OrdonnanceLignes?.Select(l => _ligneMapper.ToDto(l)).ToList() ?? new List<OrdonnanceLigneDto>()
        );
    }

    /// <summary>
    /// Convertit pour la liste (sans les lignes)
    /// </summary>
    public OrdonnanceListDto ToListDto(Ordonnance entity)
    {
        return new OrdonnanceListDto(
            entity.Id,
            $"{entity.Doctor?.FirstName} {entity.Doctor?.LastName}",
            $"{entity.Patient?.FirstName} {entity.Patient?.LastName}",
            entity.Date,
            entity.OrdonnanceLignes?.Count ?? 0
        );
    }

    /// <summary>
    /// Convertit un DTO de création vers une entité
    /// Les lignes sont mappées séparément
    /// </summary>
    public Ordonnance ToEntity(OrdonnanceCreateDto dto)
    {
        return new Ordonnance
        {
            DoctorId = dto.DoctorId,
            PatientId = dto.PatientId,
            ConsultationId = dto.ConsultationId,
            Date = dto.Date,
            Notes = dto.Notes,
            OrdonnanceLignes = dto.Lignes.Select(l => _ligneMapper.ToEntity(l)).ToList()
        };
    }

    /// <summary>
    /// Met à jour une entité existante (sans les lignes)
    /// </summary>
    public void UpdateEntity(OrdonnanceUpdateDto dto, Ordonnance entity)
    {
        entity.DoctorId = dto.DoctorId;
        entity.PatientId = dto.PatientId;
        entity.ConsultationId = dto.ConsultationId;
        entity.Date = dto.Date;
        entity.Notes = dto.Notes;
    }
}