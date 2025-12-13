using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

/// <summary>
/// Service d'application pour les spécialités
/// Selon le cours: Les services orchestrent la logique métier (slide 230)
/// </summary>
public class SpecialtyService(
    SpecialtyRepository repository,
    SpecialtyMapper mapper,
    ILogger<SpecialtyService> logger)
{
    /// <summary>
    /// Récupère toutes les spécialités
    /// Endpoint: GET /api/specialties
    /// </summary>
    public async Task<List<SpecialtyDto>> GetAllAsync()
    {
        logger.LogInformation("Récupération de toutes les spécialités");

        var specialties = await repository.GetAllAsync();
        return specialties.Select(s => mapper.ToDto(s)).ToList();
    }

    /// <summary>
    /// Récupère une spécialité par son ID
    /// Endpoint: GET /api/specialties/{id}
    /// </summary>
    public async Task<SpecialtyDto?> GetByIdAsync(int id)
    {
        logger.LogInformation("Récupération de la spécialité {Id}", id);

        var specialty = await repository.GetByIdAsync(id);
        return specialty != null ? mapper.ToDto(specialty) : null;
    }
}