namespace KingdomHospital.Application.DTOs;

/// <summary>
/// DTO complet pour une ordonnance avec ses lignes
/// GET /api/ordonnances/{id}
/// </summary>
public record OrdonnanceDto(
    int Id,
    int DoctorId,
    string DoctorName,
    int PatientId,
    string PatientName,
    int? ConsultationId,
    DateOnly Date,
    string? Notes,
    List<OrdonnanceLigneDto> Lignes
);

/// <summary>
/// DTO pour lister les ordonnances (sans les lignes)
/// GET /api/ordonnances
/// </summary>
public record OrdonnanceListDto(
    int Id,
    string DoctorName,
    string PatientName,
    DateOnly Date,
    int NombreLignes
);

/// <summary>
/// DTO pour créer une ordonnance
/// POST /api/ordonnances
/// </summary>
public record OrdonnanceCreateDto(
    int DoctorId,
    int PatientId,
    int? ConsultationId,
    DateOnly Date,
    string? Notes,
    List<OrdonnanceLigneCreateDto> Lignes
);

/// <summary>
/// DTO pour mettre à jour une ordonnance
/// PUT /api/ordonnances/{id}
/// </summary>
public record OrdonnanceUpdateDto(
    int DoctorId,
    int PatientId,
    int? ConsultationId,
    DateOnly Date,
    string? Notes
);