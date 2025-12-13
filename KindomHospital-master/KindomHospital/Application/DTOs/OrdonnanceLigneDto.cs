namespace KingdomHospital.Application.DTOs;

/// <summary>
/// DTO pour une ligne d'ordonnance
/// Utilisé dans OrdonnanceDto
/// </summary>
public record OrdonnanceLigneDto(
    int Id,
    int MedicamentId,
    string MedicamentName,
    string Dosage,
    string Frequency,
    string Duration,
    int Quantity,
    string? Instructions
);

/// <summary>
/// DTO pour créer une ligne d'ordonnance
/// Utilisé dans OrdonnanceCreateDto
/// </summary>
public record OrdonnanceLigneCreateDto(
    int MedicamentId,
    string Dosage,
    string Frequency,
    string Duration,
    int Quantity,
    string? Instructions
);

/// <summary>
/// DTO pour mettre à jour une ligne d'ordonnance
/// PUT /api/ordonnances/{id}/lignes/{ligneId}
/// </summary>
public record OrdonnanceLigneUpdateDto(
    int MedicamentId,
    string Dosage,
    string Frequency,
    string Duration,
    int Quantity,
    string? Instructions
);