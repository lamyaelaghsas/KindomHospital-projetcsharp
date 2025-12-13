using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

/// <summary>
/// Repository pour gérer l'accès aux données des médecins
/// Selon le cours: Repository pattern (slide 228)
/// </summary>
public class DoctorRepository(KingdomHospitalContext context)
{
    /// <summary>
    /// Récupère tous les médecins avec leur spécialité
    /// Selon le cours: Include() pour eager loading (slide 35)
    /// </summary>
    public async Task<List<Doctor>> GetAllAsync()
    {
        return await context.Doctors
            .Include(d => d.Specialty)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère un médecin par ID avec sa spécialité
    /// </summary>
    public async Task<Doctor?> GetByIdAsync(int id)
    {
        return await context.Doctors
            .Include(d => d.Specialty)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    /// <summary>
    /// Ajoute un nouveau médecin
    /// Selon le cours: Add() marque l'entité comme Added (slide 38)
    /// </summary>
    public async Task<Doctor> AddAsync(Doctor doctor)
    {
        context.Doctors.Add(doctor);
        await context.SaveChangesAsync();

        // Recharger avec la spécialité
        return (await GetByIdAsync(doctor.Id))!;
    }

    /// <summary>
    /// Met à jour un médecin existant
    /// Selon le cours: Update() marque l'entité comme Modified (slide 39)
    /// </summary>
    public async Task UpdateAsync(Doctor doctor)
    {
        context.Doctors.Update(doctor);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Vérifie si un médecin existe
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Doctors.AnyAsync(d => d.Id == id);
    }
}