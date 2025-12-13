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
}