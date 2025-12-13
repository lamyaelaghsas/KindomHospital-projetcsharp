using KingdomHospital.Application.DTOs;
using KingdomHospital.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace KingdomHospital.Application.Mappers;

/// <summary>
/// Mapper pour Patient avec calcul automatique de l'âge
/// Selon le cours: Mapperly pour le mapping performant (slide 223-225)
/// </summary>
[Mapper]
public partial class PatientMapper
{
    /// <summary>
    /// Convertit un Patient vers PatientDto
    /// </summary>
    public partial PatientDto ToDto(Patient entity);

    /// <summary>
    /// Convertit pour la liste avec calcul de l'âge
    /// </summary>
    public PatientListDto ToListDto(Patient entity)
    {
        return new PatientListDto(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.BirthDate,
            CalculateAge(entity.BirthDate)
        );
    }

    /// <summary>
    /// Convertit un DTO de création vers une entité
    /// </summary>
    public partial Patient ToEntity(PatientCreateDto dto);

    /// <summary>
    /// Met à jour une entité existante avec les données du DTO
    /// </summary>
    public void UpdateEntity(PatientUpdateDto dto, Patient entity)
    {
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.BirthDate = dto.BirthDate;
    }

    /// <summary>
    /// Calcule l'âge à partir de la date de naissance
    /// </summary>
    private static int CalculateAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        int age = today.Year - birthDate.Year;

        // Ajuster si l'anniversaire n'est pas encore passé cette année
        if (birthDate > today.AddYears(-age))
            age--;

        return age;
    }
}