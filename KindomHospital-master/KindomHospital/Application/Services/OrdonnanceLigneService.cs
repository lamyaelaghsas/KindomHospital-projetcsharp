using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Infrastructure.Repositories;

namespace KingdomHospital.Application.Services;

/// <summary>
/// Service pour la logique métier des lignes d'ordonnance
/// Selon le cours: Les services orchestrent (slide 230)
/// </summary>
public class OrdonnanceLigneService(
    OrdonnanceLigneRepository repository,
    OrdonnanceRepository ordonnanceRepository,
    MedicamentRepository medicamentRepository,
    OrdonnanceLigneMapper mapper,
    ILogger<OrdonnanceLigneService> logger)
{
    /// <summary>
    /// GET /api/ordonnances/{id}/lignes
    /// </summary>
    public async Task<List<OrdonnanceLigneDto>> GetByOrdonnanceIdAsync(int ordonnanceId)
    {
        logger.LogInformation("Récupération des lignes de l'ordonnance {OrdonnanceId}", ordonnanceId);

        var lignes = await repository.GetByOrdonnanceIdAsync(ordonnanceId);
        return lignes.Select(l => mapper.ToDto(l)).ToList();
    }

    /// <summary>
    /// GET /api/ordonnances/{id}/lignes/{ligneId}
    /// </summary>
    public async Task<OrdonnanceLigneDto?> GetByIdAsync(int ordonnanceId, int ligneId)
    {
        logger.LogInformation("Récupération de la ligne {LigneId} de l'ordonnance {OrdonnanceId}",
            ligneId, ordonnanceId);

        // Vérifier que la ligne appartient à l'ordonnance
        if (!await repository.BelongsToOrdonnanceAsync(ligneId, ordonnanceId))
        {
            return null;
        }

        var ligne = await repository.GetByIdAsync(ligneId);
        return ligne != null ? mapper.ToDto(ligne) : null;
    }

    /// <summary>
    /// POST /api/ordonnances/{id}/lignes
    /// Ajoute une ou plusieurs lignes à une ordonnance existante
    /// </summary>
    public async Task<(bool Success, string? Error, List<OrdonnanceLigneDto>? Lignes)> AddLignesAsync(
        int ordonnanceId,
        List<OrdonnanceLigneCreateDto> dtos)
    {
        logger.LogInformation("Ajout de {Count} ligne(s) à l'ordonnance {OrdonnanceId}",
            dtos.Count, ordonnanceId);

        // Vérifier que l'ordonnance existe
        if (!await ordonnanceRepository.ExistsAsync(ordonnanceId))
        {
            return (false, $"Ordonnance {ordonnanceId} introuvable", null);
        }

        // Valider toutes les lignes
        foreach (var dto in dtos)
        {
            // Vérifier que le médicament existe
            if (!await medicamentRepository.ExistsAsync(dto.MedicamentId))
            {
                return (false, $"Le médicament {dto.MedicamentId} n'existe pas", null);
            }

            // Vérifier que la quantité est > 0
            if (dto.Quantity <= 0)
            {
                return (false, "La quantité doit être supérieure à 0", null);
            }
        }

        // Créer les lignes
        var lignes = dtos.Select(dto =>
        {
            var ligne = mapper.ToEntity(dto);
            ligne.OrdonnanceId = ordonnanceId;
            return ligne;
        }).ToList();

        var created = await repository.AddRangeAsync(lignes);

        return (true, null, created.Select(l => mapper.ToDto(l)).ToList());
    }

    /// <summary>
    /// PUT /api/ordonnances/{id}/lignes/{ligneId}
    /// </summary>
    public async Task<(bool Success, string? Error)> UpdateAsync(
        int ordonnanceId,
        int ligneId,
        OrdonnanceLigneUpdateDto dto)
    {
        logger.LogInformation("Mise à jour de la ligne {LigneId} de l'ordonnance {OrdonnanceId}",
            ligneId, ordonnanceId);

        // Vérifier que la ligne existe et appartient à l'ordonnance
        if (!await repository.BelongsToOrdonnanceAsync(ligneId, ordonnanceId))
        {
            return (false, $"Ligne {ligneId} introuvable dans l'ordonnance {ordonnanceId}");
        }

        var ligne = await repository.GetByIdAsync(ligneId);
        if (ligne == null)
        {
            return (false, $"Ligne {ligneId} introuvable");
        }

        // Vérifier que le médicament existe
        if (!await medicamentRepository.ExistsAsync(dto.MedicamentId))
        {
            return (false, $"Le médicament {dto.MedicamentId} n'existe pas");
        }

        // Vérifier que la quantité est > 0
        if (dto.Quantity <= 0)
        {
            return (false, "La quantité doit être supérieure à 0");
        }

        mapper.UpdateEntity(dto, ligne);
        await repository.UpdateAsync(ligne);

        return (true, null);
    }

    /// <summary>
    /// DELETE /api/ordonnances/{id}/lignes/{ligneId}
    /// </summary>
    public async Task<(bool Success, string? Error)> DeleteAsync(int ordonnanceId, int ligneId)
    {
        logger.LogInformation("Suppression de la ligne {LigneId} de l'ordonnance {OrdonnanceId}",
            ligneId, ordonnanceId);

        // Vérifier que la ligne existe et appartient à l'ordonnance
        if (!await repository.BelongsToOrdonnanceAsync(ligneId, ordonnanceId))
        {
            return (false, $"Ligne {ligneId} introuvable dans l'ordonnance {ordonnanceId}");
        }

        var ligne = await repository.GetByIdAsync(ligneId);
        if (ligne == null)
        {
            return (false, $"Ligne {ligneId} introuvable");
        }

        await repository.DeleteAsync(ligne);

        return (true, null);
    }
}