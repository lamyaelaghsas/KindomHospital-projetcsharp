using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

/// <summary>
/// Service pour la logique métier des consultations
/// Selon le cours: Les services orchestrent (slide 230)
/// </summary>
public class ConsultationService(
    ConsultationRepository repository,
    DoctorRepository doctorRepository,
    PatientRepository patientRepository,
    ConsultationMapper mapper,
    ILogger<ConsultationService> logger)
{
    /// <summary>
    /// GET /api/consultations
    /// </summary>
    public async Task<List<ConsultationListDto>> GetAllAsync()
    {
        logger.LogInformation("Récupération de toutes les consultations");

        var consultations = await repository.GetAllAsync();
        return consultations.Select(c => mapper.ToListDto(c)).ToList();
    }

    /// <summary>
    /// GET /api/consultations/{id}
    /// </summary>
    public async Task<ConsultationDto?> GetByIdAsync(int id)
    {
        logger.LogInformation("Récupération de la consultation {Id}", id);

        var consultation = await repository.GetByIdAsync(id);
        return consultation != null ? mapper.ToDto(consultation) : null;
    }

    /// <summary>
    /// POST /api/consultations
    /// Validations:
    /// - Le médecin doit exister
    /// - Le patient doit exister
    /// - Pas de double-booking (règle métier)
    /// </summary>
    public async Task<(bool Success, string? Error, ConsultationDto? Consultation)> CreateAsync(ConsultationCreateDto dto)
    {
        logger.LogInformation("Création d'une consultation pour patient {PatientId} avec médecin {DoctorId}",
            dto.PatientId, dto.DoctorId);

        // Validation: le médecin doit exister
        if (!await doctorRepository.ExistsAsync(dto.DoctorId))
        {
            return (false, $"Le médecin {dto.DoctorId} n'existe pas", null);
        }

        // Validation: le patient doit exister
        if (!await patientRepository.ExistsAsync(dto.PatientId))
        {
            return (false, $"Le patient {dto.PatientId} n'existe pas", null);
        }

        // Validation: pas de double-booking
        if (await repository.DoctorHasConflictAsync(dto.DoctorId, dto.Date, dto.Hour))
        {
            return (false, $"Le médecin a déjà une consultation le {dto.Date:dd/MM/yyyy} à {dto.Hour:HH:mm}", null);
        }

        var consultation = mapper.ToEntity(dto);
        var created = await repository.AddAsync(consultation);

        return (true, null, mapper.ToDto(created));
    }

    /// <summary>
    /// PUT /api/consultations/{id}
    /// </summary>
    public async Task<(bool Success, string? Error)> UpdateAsync(int id, ConsultationUpdateDto dto)
    {
        logger.LogInformation("Mise à jour de la consultation {Id}", id);

        // Vérifier que la consultation existe
        var consultation = await repository.GetByIdAsync(id);
        if (consultation == null)
        {
            return (false, $"Consultation {id} introuvable");
        }

        // Validation: le médecin doit exister
        if (!await doctorRepository.ExistsAsync(dto.DoctorId))
        {
            return (false, $"Le médecin {dto.DoctorId} n'existe pas");
        }

        // Validation: le patient doit exister
        if (!await patientRepository.ExistsAsync(dto.PatientId))
        {
            return (false, $"Le patient {dto.PatientId} n'existe pas");
        }

        // Validation: pas de double-booking (exclure la consultation actuelle)
        if (await repository.DoctorHasConflictAsync(dto.DoctorId, dto.Date, dto.Hour, id))
        {
            return (false, $"Le médecin a déjà une consultation le {dto.Date:dd/MM/yyyy} à {dto.Hour:HH:mm}");
        }

        mapper.UpdateEntity(dto, consultation);
        await repository.UpdateAsync(consultation);

        return (true, null);
    }

    /// <summary>
    /// DELETE /api/consultations/{id}
    /// Selon l'énoncé: Une consultation peut être supprimée si elle n'a pas d'ordonnances liées
    /// </summary>
    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        logger.LogInformation("Suppression de la consultation {Id}", id);

        // Vérifier que la consultation existe
        var consultation = await repository.GetByIdAsync(id);
        if (consultation == null)
        {
            return (false, $"Consultation {id} introuvable");
        }

        // Vérifier qu'elle n'a pas d'ordonnances
        if (await repository.HasOrdonnancesAsync(id))
        {
            return (false, "Impossible de supprimer une consultation ayant des ordonnances (traçabilité historique)");
        }

        await repository.DeleteAsync(consultation);

        return (true, null);
    }
}