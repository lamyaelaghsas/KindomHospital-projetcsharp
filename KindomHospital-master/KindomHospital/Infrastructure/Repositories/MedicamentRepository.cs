using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

/// <summary>
/// Repository pour gérer l'accès aux données des médicaments
/// Selon le cours: Repository pattern (slide 228)
/// </summary>
public class MedicamentRepository(KingdomHospitalContext context)
{
    /// <summary>
    /// Récupère tous les médicaments
    /// Selon le cours: ToListAsync() pour les opérations asynchrones (slide 31)
    /// </summary>
    public async Task<List<Medicament>> GetAllAsync()
    {
        return await context.Medicaments
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère un médicament par ID
    /// Selon le cours: FindAsync() pour rechercher par clé primaire (slide 31)
    /// </summary>
    public async Task<Medicament?> GetByIdAsync(int id)
    {
        return await context.Medicaments.FindAsync(id);
    }

    /// <summary>
    /// Ajoute un nouveau médicament
    /// Selon le cours: Add() marque l'entité comme Added (slide 38)
    /// </summary>
    public async Task<Medicament> AddAsync(Medicament medicament)
    {
        context.Medicaments.Add(medicament);
        await context.SaveChangesAsync();
        return medicament;
    }

    /// <summary>
    /// Vérifie si un médicament existe
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Medicaments.AnyAsync(m => m.Id == id);
    }

    /// <summary>
    /// Vérifie si un médicament identique existe déjà (Name + DosageForm + Strength)
    /// Selon l'énoncé: unicité sur ces 3 champs
    /// </summary>
    public async Task<bool> DuplicateExistsAsync(string name, string dosageForm, string strength, int? excludeId = null)
    {
        var query = context.Medicaments
            .Where(m =>
                m.Name.ToLower() == name.ToLower() &&
                m.DosageForm.ToLower() == dosageForm.ToLower() &&
                m.Strength.ToLower() == strength.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}