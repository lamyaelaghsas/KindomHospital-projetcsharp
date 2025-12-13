namespace KingdomHospital.Application.DTOs;

/// DTO pour transférer les données d'une spécialité
public record SpecialtyDto(int Id, string Name);

/// DTO pour créer une spécialité (sans Id car auto-généré)
/// Utilisé pour POST /api/specialties
public record SpecialtyCreateDto(string Name);