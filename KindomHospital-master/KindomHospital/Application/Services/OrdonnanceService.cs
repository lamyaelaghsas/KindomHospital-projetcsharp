using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

/// <summary>
/// Service pour la logique métier des ordonnances
/// Selon le cours: Les services orchestrent (slide 230)
/// </summary>
public class OrdonnanceService(
    OrdonnanceRepository repository,
    DoctorRepository doctorRepository,
    PatientRepository patientRepository,
    ConsultationRepository consultationRepository,
    MedicamentRepository medicamentRepository,
    OrdonnanceMapper mapper,
    ILogger<OrdonnanceService> logger)
{
    /// <summary>
    /// GET /api/ordonnances
    /// </summary>
    public async Task<List<OrdonnanceListDto>> GetAllAsync()
    {
        logger.LogInformation("Récupération de toutes les ordonnances");

        var ordonnances = await repository.GetAllAsync();
        return ordonnances.Select(o => mapper.ToListDto(o)).ToList();
    }

    /// <summary>
    /// GET /api/ordonnances/{id}
    /// </summary>
    public async Task<OrdonnanceDto?> GetByIdAsync(int id)
    {
        logger.LogInformation("Récupération de l'ordonnance {Id}", id);

        var ordonnance = await repository.GetByIdAsync(id);
        return ordonnance != null ? mapper.ToDto(ordonnance) : null;
    }

    /// <summary>
    /// POST /api/ordonnances
    /// Validations métier selon l'énoncé:
    /// - Au moins 1 ligne d'ordonnance
    /// - Médecin, patient et médicaments doivent exister
    /// - Si consultation liée, vérifier cohérence médecin/patient
    /// </summary>
    public async Task<(bool Success, string? Error, OrdonnanceDto? Ordonnance)> CreateAsync(OrdonnanceCreateDto dto)
    {
        logger.LogInformation("Création d'une ordonnance pour patient {PatientId} par médecin {DoctorId}",
            dto.PatientId, dto.DoctorId);

        // Validation: au moins 1 ligne (règle métier de l'énoncé)
        if (dto.Lignes == null || dto.Lignes.Count == 0)
        {
            return (false, "Une ordonnance doit contenir au moins 1 ligne", null);
        }

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

        // Validation: si consultation liée, elle doit exister et correspondre
        if (dto.ConsultationId.HasValue)
        {
            var consultation = await consultationRepository.GetByIdAsync(dto.ConsultationId.Value);
            if (consultation == null)
            {
                return (false, $"La consultation {dto.ConsultationId} n'existe pas", null);
            }

            // Vérifier cohérence médecin/patient
            if (consultation.DoctorId != dto.DoctorId)
            {
                return (false, "Le médecin de l'ordonnance doit correspondre au médecin de la consultation", null);
            }
            if (consultation.PatientId != dto.PatientId)
            {
                return (false, "Le patient de l'ordonnance doit correspondre au patient de la consultation", null);
            }
        }

        // Validation: tous les médicaments doivent exister
        foreach (var ligne in dto.Lignes)
        {
            if (!await medicamentRepository.ExistsAsync(ligne.MedicamentId))
            {
                return (false, $"Le médicament {ligne.MedicamentId} n'existe pas", null);
            }

            // Validation: quantité > 0
            if (ligne.Quantity <= 0)
            {
                return (false, "La quantité doit être supérieure à 0", null);
            }
        }

        var ordonnance = mapper.ToEntity(dto);
        var created = await repository.AddAsync(ordonnance);

        return (true, null, mapper.ToDto(created));
    }

    /// <summary>
    /// PUT /api/ordonnances/{id}
    /// Met à jour l'ordonnance (sans les lignes)
    /// </summary>
    public async Task<(bool Success, string? Error)> UpdateAsync(int id, OrdonnanceUpdateDto dto)
    {
        logger.LogInformation("Mise à jour de l'ordonnance {Id}", id);

        // Vérifier que l'ordonnance existe
        var ordonnance = await repository.GetByIdAsync(id);
        if (ordonnance == null)
        {
            return (false, $"Ordonnance {id} introuvable");
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

        // Validation: si consultation liée, vérifier cohérence
        if (dto.ConsultationId.HasValue)
        {
            var consultation = await consultationRepository.GetByIdAsync(dto.ConsultationId.Value);
            if (consultation == null)
            {
                return (false, $"La consultation {dto.ConsultationId} n'existe pas");
            }

            if (consultation.DoctorId != dto.DoctorId || consultation.PatientId != dto.PatientId)
            {
                return (false, "La consultation ne correspond pas au médecin/patient de l'ordonnance");
            }
        }

        mapper.UpdateEntity(dto, ordonnance);
        await repository.UpdateAsync(ordonnance);

        return (true, null);
    }

    /// <summary>
    /// DELETE /api/ordonnances/{id}
    /// Selon le cours: la suppression en cascade supprime aussi les lignes (slide 44)
    /// </summary>
    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        logger.LogInformation("Suppression de l'ordonnance {Id}", id);

        // Vérifier que l'ordonnance existe
        var ordonnance = await repository.GetByIdAsync(id);
        if (ordonnance == null)
        {
            return (false, $"Ordonnance {id} introuvable");
        }

        // La suppression en cascade supprime automatiquement les lignes
        await repository.DeleteAsync(ordonnance);

        return (true, null);
    }
}