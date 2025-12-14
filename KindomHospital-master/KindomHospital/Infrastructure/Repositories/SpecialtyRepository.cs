using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

/// <summary>
/// Repository pour gérer l'accès aux données des spécialités
/// Selon le cours: Le Repository encapsule la logique d'accès aux données (slide 228)
/// </summary>
public class SpecialtyRepository(KingdomHospitalContext context)
{
    /// <summary>
    /// Récupère toutes les spécialités
    /// Selon le cours: ToListAsync() pour les opérations asynchrones (slide 31)
    /// </summary>
    public async Task<List<Specialty>> GetAllAsync()
    {
        return await context.Specialties
            .OrderBy(s => s.Name) // Tri alphabétique
            .ToListAsync();
    }

    /// <summary>
    /// Récupère une spécialité par son ID
    /// Selon le cours: FindAsync() pour rechercher par clé primaire (slide 31)
    /// </summary>
    public async Task<Specialty?> GetByIdAsync(int id)
    {
        return await context.Specialties.FindAsync(id);
    }

    /// <summary>
    /// Récupère une spécialité avec ses médecins
    /// Selon le cours: Include() pour le chargement eager (slide 35)
    /// </summary>
    public async Task<Specialty?> GetByIdWithDoctorsAsync(int id)
    {
        return await context.Specialties
            .Include(s => s.Doctors)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Vérifie si une spécialité existe
    /// Selon le cours: Any() pour vérifier l'existence (slide 31)
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Specialties.AnyAsync(s => s.Id == id);
    }

    /// <summary>
    /// Vérifie si un nom de spécialité existe déjà (pour unicité)
    /// </summary>
    public async Task<bool> NameExistsAsync(string name)
    {
        return await context.Specialties
            .AnyAsync(s => s.Name.ToLower() == name.ToLower());
    }
}