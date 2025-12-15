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

    /// <summary>
    /// GET /api/consultations/{id}/ordonnances
    /// Liste les ordonnances liées à une consultation
    /// </summary>
    public async Task<IEnumerable<OrdonnanceListDto>> GetOrdonnancesByConsultationIdAsync(int consultationId)
    {
        logger.LogInformation("Récupération des ordonnances de la consultation {ConsultationId}", consultationId);

        // Vérifier que la consultation existe
        if (!await repository.ExistsAsync(consultationId))
            return Enumerable.Empty<OrdonnanceListDto>();

        var ordonnances = await repository.GetOrdonnancesByConsultationIdAsync(consultationId);

        return ordonnances.Select(o => new OrdonnanceListDto(
            o.Id,
            $"{o.Doctor?.FirstName} {o.Doctor?.LastName}",
            $"{o.Patient?.FirstName} {o.Patient?.LastName}",
            o.Date,
            o.OrdonnanceLignes?.Count ?? 0
        ));
    }

    /// <summary>
    /// GET /api/consultations?doctorId=&patientId=&from=&to=
    /// Filtre les consultations. Au moins doctorId ou patientId doit être fourni.
    /// </summary>
    public async Task<(bool Success, string? Error, List<ConsultationListDto>? Consultations)> GetFilteredAsync(
        int? doctorId,
        int? patientId,
        DateOnly? from,
        DateOnly? to)
    {
        logger.LogInformation(
            "Filtrage consultations - DoctorId: {DoctorId}, PatientId: {PatientId}, From: {From}, To: {To}",
            doctorId, patientId, from, to);

        // Validation: au moins un ID doit être fourni
        if (!doctorId.HasValue && !patientId.HasValue)
        {
            return (false, "Au moins doctorId ou patientId doit être fourni", null);
        }

        // Vérifier que le docteur existe si fourni
        if (doctorId.HasValue && !await doctorRepository.ExistsAsync(doctorId.Value))
        {
            return (false, $"Le médecin {doctorId.Value} n'existe pas", null);
        }

        // Vérifier que le patient existe si fourni
        if (patientId.HasValue && !await patientRepository.ExistsAsync(patientId.Value))
        {
            return (false, $"Le patient {patientId.Value} n'existe pas", null);
        }

        var consultations = await repository.GetFilteredAsync(doctorId, patientId, from, to);
        return (true, null, consultations.Select(c => mapper.ToListDto(c)).ToList());
    }
}