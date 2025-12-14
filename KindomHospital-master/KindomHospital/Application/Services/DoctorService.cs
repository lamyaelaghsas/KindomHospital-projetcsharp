using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

/// <summary>
/// Service pour la logique métier des médecins
/// Selon le cours: Les services orchestrent (slide 230)
/// </summary>
public class DoctorService(
    DoctorRepository repository,
    SpecialtyRepository specialtyRepository,
    DoctorMapper mapper,
    ILogger<DoctorService> logger)
{
    /// <summary>
    /// GET /api/doctors
    /// </summary>
    public async Task<List<DoctorListDto>> GetAllAsync()
    {
        logger.LogInformation("Récupération de tous les médecins");

        var doctors = await repository.GetAllAsync();
        return doctors.Select(d => mapper.ToListDto(d)).ToList();
    }

    /// <summary>
    /// GET /api/doctors/{id}
    /// </summary>
    public async Task<DoctorDto?> GetByIdAsync(int id)
    {
        logger.LogInformation("Récupération du médecin {Id}", id);

        var doctor = await repository.GetByIdAsync(id);
        return doctor != null ? mapper.ToDto(doctor) : null;
    }

    /// <summary>
    /// POST /api/doctors
    /// Validation: la spécialité doit exister
    /// </summary>
    public async Task<(bool Success, string? Error, DoctorDto? Doctor)> CreateAsync(DoctorCreateDto dto)
    {
        logger.LogInformation("Création d'un médecin: {FirstName} {LastName}", dto.FirstName, dto.LastName);

        // Validation: la spécialité doit exister
        if (!await specialtyRepository.ExistsAsync(dto.SpecialtyId))
        {
            return (false, $"La spécialité {dto.SpecialtyId} n'existe pas", null);
        }

        var doctor = mapper.ToEntity(dto);
        var created = await repository.AddAsync(doctor);

        return (true, null, mapper.ToDto(created));
    }

    /// <summary>
    /// PUT /api/doctors/{id}
    /// </summary>
    public async Task<(bool Success, string? Error)> UpdateAsync(int id, DoctorUpdateDto dto)
    {
        logger.LogInformation("Mise à jour du médecin {Id}", id);

        // Vérifier que le médecin existe
        var doctor = await repository.GetByIdAsync(id);
        if (doctor == null)
        {
            return (false, $"Médecin {id} introuvable");
        }

        // Vérifier que la spécialité existe
        if (!await specialtyRepository.ExistsAsync(dto.SpecialtyId))
        {
            return (false, $"La spécialité {dto.SpecialtyId} n'existe pas");
        }

        mapper.UpdateEntity(dto, doctor);
        await repository.UpdateAsync(doctor);

        return (true, null);
    }

    /// <summary>
    /// GET /api/doctors/{id}/specialty
    /// Renvoie la spécialité d'un médecin
    /// </summary>
    public async Task<SpecialtyDto?> GetSpecialtyByDoctorIdAsync(int doctorId)
    {
        logger.LogInformation("Récupération de la spécialité du médecin {DoctorId}", doctorId);

        var doctor = await repository.GetByIdAsync(doctorId);
        if (doctor?.Specialty == null)
            return null;

        return new SpecialtyDto(doctor.Specialty.Id, doctor.Specialty.Name);
    }

    /// <summary>
    /// PUT /api/doctors/{id}/specialty/{specialtyId}
    /// Change la spécialité d'un médecin
    /// </summary>
    public async Task<(bool Success, string? Error)> UpdateDoctorSpecialtyAsync(int doctorId, int specialtyId)
    {
        logger.LogInformation("Changement de spécialité du médecin {DoctorId} vers {SpecialtyId}", doctorId, specialtyId);

        // Vérifier que le médecin existe
        if (!await repository.ExistsAsync(doctorId))
        {
            return (false, $"Médecin {doctorId} introuvable");
        }

        // Vérifier que la spécialité existe
        if (!await specialtyRepository.ExistsAsync(specialtyId))
        {
            return (false, $"Spécialité {specialtyId} introuvable");
        }

        var success = await repository.UpdateSpecialtyAsync(doctorId, specialtyId);
        return success
            ? (true, null)
            : (false, "Erreur lors de la mise à jour");
    }

    /// <summary>
    /// GET /api/doctors/{id}/consultations?from=&to=
    /// Liste les consultations d'un médecin avec filtre de dates optionnel
    /// </summary>
    public async Task<IEnumerable<ConsultationListDto>> GetConsultationsByDoctorIdAsync(
        int doctorId,
        DateOnly? from = null,
        DateOnly? to = null)
    {
        logger.LogInformation("Récupération des consultations du médecin {DoctorId}", doctorId);

        // Vérifier que le médecin existe
        if (!await repository.ExistsAsync(doctorId))
            return Enumerable.Empty<ConsultationListDto>();

        var consultations = await repository.GetConsultationsByDoctorIdAsync(doctorId, from, to);

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
    /// GET /api/doctors/{id}/patients
    /// Liste les patients déjà consultés par un médecin
    /// </summary>
    public async Task<IEnumerable<PatientListDto>> GetPatientsByDoctorIdAsync(int doctorId)
    {
        logger.LogInformation("Récupération des patients du médecin {DoctorId}", doctorId);

        // Vérifier que le médecin existe
        if (!await repository.ExistsAsync(doctorId))
            return Enumerable.Empty<PatientListDto>();

        var patients = await repository.GetPatientsByDoctorIdAsync(doctorId);

        return patients.Select(p => new PatientListDto(
            p.Id,
            p.FirstName,
            p.LastName,
            p.BirthDate,
            DateTime.Now.Year - p.BirthDate.Year // Calcul simple de l'âge
        ));
    }

    /// <summary>
    /// GET /api/doctors/{id}/ordonnances?from=&to=
    /// Liste les ordonnances émises par un médecin avec filtre de dates optionnel
    /// </summary>
    public async Task<IEnumerable<OrdonnanceListDto>> GetOrdonnancesByDoctorIdAsync(
        int doctorId,
        DateOnly? from = null,
        DateOnly? to = null)
    {
        logger.LogInformation("Récupération des ordonnances du médecin {DoctorId}", doctorId);

        // Vérifier que le médecin existe
        if (!await repository.ExistsAsync(doctorId))
            return Enumerable.Empty<OrdonnanceListDto>();

        var ordonnances = await repository.GetOrdonnancesByDoctorIdAsync(doctorId, from, to);

        return ordonnances.Select(o => new OrdonnanceListDto(
            o.Id,
            $"{o.Doctor?.FirstName} {o.Doctor?.LastName}",
            $"{o.Patient?.FirstName} {o.Patient?.LastName}",
            o.Date,
            o.OrdonnanceLignes?.Count ?? 0
        ));
    }
}