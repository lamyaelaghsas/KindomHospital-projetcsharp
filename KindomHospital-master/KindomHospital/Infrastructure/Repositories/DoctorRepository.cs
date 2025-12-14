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

    /// <summary>
    /// Met à jour la spécialité d'un médecin
    /// </summary>
    public async Task<bool> UpdateSpecialtyAsync(int doctorId, int newSpecialtyId)
    {
        var doctor = await context.Doctors.FindAsync(doctorId);
        if (doctor == null)
            return false;

        // Vérifier que la nouvelle spécialité existe
        var specialtyExists = await context.Specialties.AnyAsync(s => s.Id == newSpecialtyId);
        if (!specialtyExists)
            return false;

        doctor.SpecialtyID = newSpecialtyId;
        await context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Récupère les consultations d'un médecin avec filtre de dates optionnel
    /// </summary>
    public async Task<IEnumerable<Consultation>> GetConsultationsByDoctorIdAsync(
        int doctorId,
        DateOnly? from = null,
        DateOnly? to = null)
    {
        var query = context.Consultations
            .Include(c => c.Doctor)
            .Include(c => c.Patient)
            .Where(c => c.DoctorId == doctorId);

        if (from.HasValue)
            query = query.Where(c => c.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(c => c.Date <= to.Value);

        return await query
            .OrderByDescending(c => c.Date)
            .ThenByDescending(c => c.Hour)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère les patients distincts déjà consultés par un médecin
    /// </summary>
    public async Task<IEnumerable<Patient>> GetPatientsByDoctorIdAsync(int doctorId)
    {
        return await context.Consultations
            .Where(c => c.DoctorId == doctorId)
            .Select(c => c.Patient)
            .Distinct()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère les ordonnances d'un médecin avec filtre de dates optionnel
    /// </summary>
    public async Task<IEnumerable<Ordonnance>> GetOrdonnancesByDoctorIdAsync(
        int doctorId,
        DateOnly? from = null,
        DateOnly? to = null)
    {
        var query = context.Ordonnances
            .Include(o => o.Doctor)
            .Include(o => o.Patient)
            .Include(o => o.OrdonnanceLignes)
            .Where(o => o.DoctorId == doctorId);

        if (from.HasValue)
            query = query.Where(o => o.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(o => o.Date <= to.Value);

        return await query
            .OrderByDescending(o => o.Date)
            .ToListAsync();
    }
}