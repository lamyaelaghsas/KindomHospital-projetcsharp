using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

/// <summary>
/// Repository pour gérer l'accès aux données des ordonnances
/// Selon le cours: Repository pattern (slide 228)
/// </summary>
public class OrdonnanceRepository(KingdomHospitalContext context)
{
    /// <summary>
    /// Récupère toutes les ordonnances avec relations
    /// Selon le cours: Include() pour eager loading (slide 35)
    /// </summary>
    public async Task<List<Ordonnance>> GetAllAsync()
    {
        return await context.Ordonnances
            .Include(o => o.Doctor)
            .Include(o => o.Patient)
            .Include(o => o.OrdonnanceLignes)
            .OrderByDescending(o => o.Date)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère une ordonnance par ID avec toutes ses relations
    /// </summary>
    public async Task<Ordonnance?> GetByIdAsync(int id)
    {
        return await context.Ordonnances
            .Include(o => o.Doctor)
            .Include(o => o.Patient)
            .Include(o => o.Consultation)
            .Include(o => o.OrdonnanceLignes)
                .ThenInclude(l => l.Medicament)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    /// <summary>
    /// Ajoute une nouvelle ordonnance avec ses lignes
    /// Selon le cours: Add() marque l'entité comme Added (slide 38)
    /// La relation en cascade ajoute aussi les lignes
    /// </summary>
    public async Task<Ordonnance> AddAsync(Ordonnance ordonnance)
    {
        context.Ordonnances.Add(ordonnance);
        await context.SaveChangesAsync();

        // Recharger avec toutes les relations
        return (await GetByIdAsync(ordonnance.Id))!;
    }

    /// <summary>
    /// Met à jour une ordonnance (sans les lignes)
    /// Selon le cours: Update() marque l'entité comme Modified (slide 39)
    /// </summary>
    public async Task UpdateAsync(Ordonnance ordonnance)
    {
        context.Ordonnances.Update(ordonnance);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime une ordonnance
    /// Selon le cours: Remove() marque l'entité comme Deleted (slide 42)
    /// La suppression en cascade supprime aussi les lignes (configuré dans OrdonnanceConfiguration)
    /// </summary>
    public async Task DeleteAsync(Ordonnance ordonnance)
    {
        context.Ordonnances.Remove(ordonnance);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Vérifie si une ordonnance existe
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Ordonnances.AnyAsync(o => o.Id == id);
    }
}