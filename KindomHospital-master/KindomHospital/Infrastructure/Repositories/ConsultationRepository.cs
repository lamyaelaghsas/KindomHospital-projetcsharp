using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingdomHospital.Infrastructure.Repositories;

/// <summary>
/// Repository pour gérer l'accès aux données des consultations
/// Selon le cours: Repository pattern (slide 228)
/// </summary>
public class ConsultationRepository(KingdomHospitalContext context)
{
    /// <summary>
    /// Récupère toutes les consultations avec Doctor et Patient
    /// Selon le cours: Include() pour eager loading (slide 35)
    /// </summary>
    public async Task<List<Consultation>> GetAllAsync()
    {
        return await context.Consultations
            .Include(c => c.Doctor)
            .Include(c => c.Patient)
            .OrderByDescending(c => c.Date)
            .ThenByDescending(c => c.Hour)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère une consultation par ID
    /// </summary>
    public async Task<Consultation?> GetByIdAsync(int id)
    {
        return await context.Consultations
            .Include(c => c.Doctor)
            .Include(c => c.Patient)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Ajoute une nouvelle consultation
    /// Selon le cours: Add() marque l'entité comme Added (slide 38)
    /// </summary>
    public async Task<Consultation> AddAsync(Consultation consultation)
    {
        context.Consultations.Add(consultation);
        await context.SaveChangesAsync();

        // Recharger avec les relations
        return (await GetByIdAsync(consultation.Id))!;
    }

    /// <summary>
    /// Met à jour une consultation
    /// Selon le cours: Update() marque l'entité comme Modified (slide 39)
    /// </summary>
    public async Task UpdateAsync(Consultation consultation)
    {
        context.Consultations.Update(consultation);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime une consultation
    /// Selon le cours: Remove() marque l'entité comme Deleted (slide 42)
    /// </summary>
    public async Task DeleteAsync(Consultation consultation)
    {
        context.Consultations.Remove(consultation);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Vérifie si une consultation existe
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Consultations.AnyAsync(c => c.Id == id);
    }

    /// <summary>
    /// Vérifie si le médecin a déjà une consultation à cette date/heure
    /// Règle métier: pas de double-booking
    /// Selon l'énoncé: un médecin ne peut pas avoir 2 consultations au même moment
    /// </summary>
    public async Task<bool> DoctorHasConflictAsync(int doctorId, DateOnly date, TimeOnly hour, int? excludeConsultationId = null)
    {
        var query = context.Consultations
            .Where(c => c.DoctorId == doctorId && c.Date == date && c.Hour == hour);

        // Exclure la consultation actuelle lors d'une mise à jour
        if (excludeConsultationId.HasValue)
        {
            query = query.Where(c => c.Id != excludeConsultationId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Vérifie si une consultation a des ordonnances
    /// </summary>
    public async Task<bool> HasOrdonnancesAsync(int id)
    {
        return await context.Ordonnances.AnyAsync(o => o.ConsultationId == id);
    }
}