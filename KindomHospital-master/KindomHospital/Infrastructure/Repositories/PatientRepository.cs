using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

/// <summary>
/// Repository pour gérer l'accès aux données des patients
/// Selon le cours: Repository pattern (slide 228)
/// </summary>
public class PatientRepository(KingdomHospitalContext context)
{
    /// <summary>
    /// Récupère tous les patients
    /// Selon le cours: ToListAsync() pour les opérations asynchrones (slide 31)
    /// </summary>
    public async Task<List<Patient>> GetAllAsync()
    {
        return await context.Patients
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère un patient par ID
    /// Selon le cours: FindAsync() pour rechercher par clé primaire (slide 31)
    /// </summary>
    public async Task<Patient?> GetByIdAsync(int id)
    {
        return await context.Patients.FindAsync(id);
    }

    /// <summary>
    /// Ajoute un nouveau patient
    /// Selon le cours: Add() marque l'entité comme Added (slide 38)
    /// </summary>
    public async Task<Patient> AddAsync(Patient patient)
    {
        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        return patient;
    }

    /// <summary>
    /// Met à jour un patient existant
    /// Selon le cours: Update() marque l'entité comme Modified (slide 39)
    /// </summary>
    public async Task UpdateAsync(Patient patient)
    {
        context.Patients.Update(patient);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime un patient
    /// Selon le cours: Remove() marque l'entité comme Deleted (slide 42)
    /// </summary>
    public async Task DeleteAsync(Patient patient)
    {
        context.Patients.Remove(patient);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Vérifie si un patient existe
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Patients.AnyAsync(p => p.Id == id);
    }

    /// <summary>
    /// Vérifie si un patient a des consultations
    /// Utilisé avant suppression pour éviter les orphelins
    /// </summary>
    public async Task<bool> HasConsultationsAsync(int id)
    {
        return await context.Consultations.AnyAsync(c => c.PatientId == id);
    }

    /// <summary>
    /// Vérifie si un patient a des ordonnances
    /// Utilisé avant suppression pour éviter les orphelins
    /// </summary>
    public async Task<bool> HasOrdonnancesAsync(int id)
    {
        return await context.Ordonnances.AnyAsync(o => o.PatientId == id);
    }
}