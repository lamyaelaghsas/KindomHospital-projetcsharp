using KingdomHospital.Application.DTOs;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

/// <summary>
/// Mapper pour Consultation
/// Selon le cours: Mapperly pour le mapping performant (slide 223-225)
/// </summary>
[Mapper]
public partial class ConsultationMapper
{
    /// <summary>
    /// Convertit une Consultation (avec Doctor et Patient chargés) vers ConsultationDto
    /// </summary>
    public ConsultationDto ToDto(Consultation entity)
    {
        return new ConsultationDto(
            entity.Id,
            entity.DoctorId,
            $"{entity.Doctor?.FirstName} {entity.Doctor?.LastName}",
            entity.PatientId,
            $"{entity.Patient?.FirstName} {entity.Patient?.LastName}",
            entity.Date,
            entity.Hour,
            entity.Reason
        );
    }

    /// <summary>
    /// Convertit pour la liste
    /// </summary>
    public ConsultationListDto ToListDto(Consultation entity)
    {
        return new ConsultationListDto(
            entity.Id,
            $"{entity.Doctor?.FirstName} {entity.Doctor?.LastName}",
            $"{entity.Patient?.FirstName} {entity.Patient?.LastName}",
            entity.Date,
            entity.Hour,
            entity.Reason
        );
    }

    /// <summary>
    /// Convertit un DTO de création vers une entité
    /// </summary>
    public partial Consultation ToEntity(ConsultationCreateDto dto);

    /// <summary>
    /// Met à jour une entité existante
    /// </summary>
    public void UpdateEntity(ConsultationUpdateDto dto, Consultation entity)
    {
        entity.DoctorId = dto.DoctorId;
        entity.PatientId = dto.PatientId;
        entity.Date = dto.Date;
        entity.Hour = dto.Hour;
        entity.Reason = dto.Reason;
    }
}