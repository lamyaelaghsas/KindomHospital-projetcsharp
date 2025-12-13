using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

/// <summary>
/// Repository pour gérer l'accès aux données des lignes d'ordonnance
/// Selon le cours: Repository pattern (slide 228)
/// </summary>
public class OrdonnanceLigneRepository(KingdomHospitalContext context)
{
    /// <summary>
    /// Récupère toutes les lignes d'une ordonnance
    /// Selon le cours: Include() pour eager loading (slide 35)
    /// </summary>
    public async Task<List<OrdonnanceLigne>> GetByOrdonnanceIdAsync(int ordonnanceId)
    {
        return await context.OrdonnanceLignes
            .Include(l => l.Medicament)
            .Where(l => l.OrdonnanceId == ordonnanceId)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère une ligne par ID
    /// </summary>
    public async Task<OrdonnanceLigne?> GetByIdAsync(int id)
    {
        return await context.OrdonnanceLignes
            .Include(l => l.Medicament)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    /// <summary>
    /// Ajoute une nouvelle ligne d'ordonnance
    /// Selon le cours: Add() marque l'entité comme Added (slide 38)
    /// </summary>
    public async Task<OrdonnanceLigne> AddAsync(OrdonnanceLigne ligne)
    {
        context.OrdonnanceLignes.Add(ligne);
        await context.SaveChangesAsync();

        // Recharger avec le médicament
        return (await GetByIdAsync(ligne.Id))!;
    }

    /// <summary>
    /// Ajoute plusieurs lignes en une seule fois
    /// </summary>
    public async Task<List<OrdonnanceLigne>> AddRangeAsync(List<OrdonnanceLigne> lignes)
    {
        context.OrdonnanceLignes.AddRange(lignes);
        await context.SaveChangesAsync();

        // Recharger avec les médicaments
        var ids = lignes.Select(l => l.Id).ToList();
        return await context.OrdonnanceLignes
            .Include(l => l.Medicament)
            .Where(l => ids.Contains(l.Id))
            .ToListAsync();
    }

    /// <summary>
    /// Met à jour une ligne d'ordonnance
    /// Selon le cours: Update() marque l'entité comme Modified (slide 39)
    /// </summary>
    public async Task UpdateAsync(OrdonnanceLigne ligne)
    {
        context.OrdonnanceLignes.Update(ligne);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime une ligne d'ordonnance
    /// Selon le cours: Remove() marque l'entité comme Deleted (slide 42)
    /// </summary>
    public async Task DeleteAsync(OrdonnanceLigne ligne)
    {
        context.OrdonnanceLignes.Remove(ligne);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Vérifie si une ligne existe
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await context.OrdonnanceLignes.AnyAsync(l => l.Id == id);
    }

    /// <summary>
    /// Vérifie si une ligne appartient à une ordonnance donnée
    /// </summary>
    public async Task<bool> BelongsToOrdonnanceAsync(int ligneId, int ordonnanceId)
    {
        return await context.OrdonnanceLignes
            .AnyAsync(l => l.Id == ligneId && l.OrdonnanceId == ordonnanceId);
    }
}