namespace KingdomHospital.Application.DTOs;

/// <summary>
/// DTO complet pour un patient
/// GET /api/patients/{id}
/// </summary>
public record PatientDto(
    int Id,
    string FirstName,
    string LastName,
    DateOnly BirthDate
);

/// <summary>
/// DTO pour lister les patients
/// GET /api/patients
/// </summary>
public record PatientListDto(
    int Id,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    int Age // Calculé automatiquement
);

/// <summary>
/// DTO pour créer un patient
/// POST /api/patients
/// </summary>
public record PatientCreateDto(
    string FirstName,
    string LastName,
    DateOnly BirthDate
);

/// <summary>
/// DTO pour mettre à jour un patient
/// PUT /api/patients/{id}
/// </summary>
public record PatientUpdateDto(
    string FirstName,
    string LastName,
    DateOnly BirthDate
);