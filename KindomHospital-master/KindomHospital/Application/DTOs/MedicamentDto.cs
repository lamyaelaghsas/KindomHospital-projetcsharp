namespace KingdomHospital.Application.DTOs;

/// <summary>
/// DTO complet pour un médicament
/// GET /api/medicaments/{id}
/// </summary>
public record MedicamentDto(
    int Id,
    string Name,
    string DosageForm,
    string Strength,
    string? AtcCode
);

/// <summary>
/// DTO pour lister les médicaments
/// GET /api/medicaments
/// </summary>
public record MedicamentListDto(
    int Id,
    string Name,
    string DosageForm,
    string Strength
);

/// <summary>
/// DTO pour créer un médicament
/// POST /api/medicaments
/// </summary>
public record MedicamentCreateDto(
    string Name,
    string DosageForm,
    string Strength,
    string? AtcCode
);