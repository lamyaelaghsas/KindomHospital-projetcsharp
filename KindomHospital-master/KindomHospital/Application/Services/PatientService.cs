using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

/// <summary>
/// Service pour la logique métier des patients
/// Selon le cours: Les services orchestrent (slide 230)
/// </summary>
public class PatientService(
    PatientRepository repository,
    PatientMapper mapper,
    ILogger<PatientService> logger)
{
    /// <summary>
    /// GET /api/patients
    /// </summary>
    public async Task<List<PatientListDto>> GetAllAsync()
    {
        logger.LogInformation("Récupération de tous les patients");

        var patients = await repository.GetAllAsync();
        return patients.Select(p => mapper.ToListDto(p)).ToList();
    }

    /// <summary>
    /// GET /api/patients/{id}
    /// </summary>
    public async Task<PatientDto?> GetByIdAsync(int id)
    {
        logger.LogInformation("Récupération du patient {Id}", id);

        var patient = await repository.GetByIdAsync(id);
        return patient != null ? mapper.ToDto(patient) : null;
    }

    /// <summary>
    /// POST /api/patients
    /// Validation: vérifier que la date de naissance est cohérente
    /// </summary>
    public async Task<(bool Success, string? Error, PatientDto? Patient)> CreateAsync(PatientCreateDto dto)
    {
        logger.LogInformation("Création d'un patient: {FirstName} {LastName}", dto.FirstName, dto.LastName);

        // Validation: date de naissance doit être dans le passé
        if (dto.BirthDate >= DateOnly.FromDateTime(DateTime.Today))
        {
            return (false, "La date de naissance doit être dans le passé", null);
        }

        // Validation: date de naissance plausible (pas avant 1900)
        if (dto.BirthDate.Year < 1900)
        {
            return (false, "La date de naissance doit être après 1900", null);
        }

        var patient = mapper.ToEntity(dto);
        var created = await repository.AddAsync(patient);

        return (true, null, mapper.ToDto(created));
    }

    /// <summary>
    /// PUT /api/patients/{id}
    /// </summary>
    public async Task<(bool Success, string? Error)> UpdateAsync(int id, PatientUpdateDto dto)
    {
        logger.LogInformation("Mise à jour du patient {Id}", id);

        // Vérifier que le patient existe
        var patient = await repository.GetByIdAsync(id);
        if (patient == null)
        {
            return (false, $"Patient {id} introuvable");
        }

        // Validation: date de naissance doit être dans le passé
        if (dto.BirthDate >= DateOnly.FromDateTime(DateTime.Today))
        {
            return (false, "La date de naissance doit être dans le passé");
        }

        // Validation: date de naissance plausible
        if (dto.BirthDate.Year < 1900)
        {
            return (false, "La date de naissance doit être après 1900");
        }

        mapper.UpdateEntity(dto, patient);
        await repository.UpdateAsync(patient);

        return (true, null);
    }

    /// <summary>
    /// DELETE /api/patients/{id}
    /// Selon le cours: Empêcher la suppression si données liées (slide 44)
    /// </summary>
    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        logger.LogInformation("Suppression du patient {Id}", id);

        // Vérifier que le patient existe
        var patient = await repository.GetByIdAsync(id);
        if (patient == null)
        {
            return (false, $"Patient {id} introuvable");
        }

        // Vérifier qu'il n'a pas de consultations
        if (await repository.HasConsultationsAsync(id))
        {
            return (false, "Impossible de supprimer un patient ayant des consultations (traçabilité historique)");
        }

        // Vérifier qu'il n'a pas d'ordonnances
        if (await repository.HasOrdonnancesAsync(id))
        {
            return (false, "Impossible de supprimer un patient ayant des ordonnances (traçabilité historique)");
        }

        await repository.DeleteAsync(patient);

        return (true, null);
    }

    /// <summary>
    /// GET /api/patients/{id}/consultations
    /// Liste les consultations d'un patient
    /// </summary>
    public async Task<IEnumerable<ConsultationListDto>> GetConsultationsByPatientIdAsync(int patientId)
    {
        logger.LogInformation("Récupération des consultations du patient {PatientId}", patientId);

        // Vérifier que le patient existe
        if (!await repository.ExistsAsync(patientId))
            return Enumerable.Empty<ConsultationListDto>();

        var consultations = await repository.GetConsultationsByPatientIdAsync(patientId);

        return consultations.Select(c => new ConsultationListDto(
            c.Id,
            $"{c.Doctor?.FirstName} {c.Doctor?.LastName}",
            $"{c.Patient?.FirstName} {c.Patient?.LastName}",
            c.Date,
            c.Hour,
            c.Reason
        ));
    }

    /// <summary>
    /// GET /api/patients/{id}/ordonnances
    /// Liste les ordonnances d'un patient
    /// </summary>
    public async Task<IEnumerable<OrdonnanceListDto>> GetOrdonnancesByPatientIdAsync(int patientId)
    {
        logger.LogInformation("Récupération des ordonnances du patient {PatientId}", patientId);

        // Vérifier que le patient existe
        if (!await repository.ExistsAsync(patientId))
            return Enumerable.Empty<OrdonnanceListDto>();

        var ordonnances = await repository.GetOrdonnancesByPatientIdAsync(patientId);

        return ordonnances.Select(o => new OrdonnanceListDto(
            o.Id,
            $"{o.Doctor?.FirstName} {o.Doctor?.LastName}",
            $"{o.Patient?.FirstName} {o.Patient?.LastName}",
            o.Date,
            o.OrdonnanceLignes?.Count ?? 0
        ));
    }
}