using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

/// <summary>
/// Service pour la logique métier des médicaments
/// Selon le cours: Les services orchestrent (slide 230)
/// </summary>
public class MedicamentService(
    MedicamentRepository repository,
    MedicamentMapper mapper,
    ILogger<MedicamentService> logger)
{
    /// <summary>
    /// GET /api/medicaments
    /// </summary>
    public async Task<List<MedicamentListDto>> GetAllAsync()
    {
        logger.LogInformation("Récupération de tous les médicaments");

        var medicaments = await repository.GetAllAsync();
        return medicaments.Select(m => mapper.ToListDto(m)).ToList();
    }

    /// <summary>
    /// GET /api/medicaments/{id}
    /// </summary>
    public async Task<MedicamentDto?> GetByIdAsync(int id)
    {
        logger.LogInformation("Récupération du médicament {Id}", id);

        var medicament = await repository.GetByIdAsync(id);
        return medicament != null ? mapper.ToDto(medicament) : null;
    }

    /// <summary>
    /// POST /api/medicaments
    /// Validation: vérifier l'unicité (Name + DosageForm + Strength)
    /// </summary>
    public async Task<(bool Success, string? Error, MedicamentDto? Medicament)> CreateAsync(MedicamentCreateDto dto)
    {
        logger.LogInformation("Création d'un médicament: {Name} {DosageForm} {Strength}",
            dto.Name, dto.DosageForm, dto.Strength);

        // Validation: vérifier l'unicité selon l'énoncé
        if (await repository.DuplicateExistsAsync(dto.Name, dto.DosageForm, dto.Strength))
        {
            return (false,
                $"Un médicament identique existe déjà ({dto.Name} {dto.DosageForm} {dto.Strength})",
                null);
        }

        var medicament = mapper.ToEntity(dto);
        var created = await repository.AddAsync(medicament);

        return (true, null, mapper.ToDto(created));
    }
}