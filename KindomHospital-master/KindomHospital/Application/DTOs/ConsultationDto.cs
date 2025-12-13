namespace KingdomHospital.Application.DTOs;

/// <summary>
/// DTO complet pour une consultation
/// GET /api/consultations/{id}
/// </summary>
public record ConsultationDto(
    int Id,
    int DoctorId,
    string DoctorName,
    int PatientId,
    string PatientName,
    DateOnly Date,
    TimeOnly Hour,
    string? Reason
);

/// <summary>
/// DTO pour lister les consultations
/// GET /api/consultations
/// </summary>
public record ConsultationListDto(
    int Id,
    string DoctorName,
    string PatientName,
    DateOnly Date,
    TimeOnly Hour,
    string? Reason
);

/// <summary>
/// DTO pour créer une consultation
/// POST /api/consultations
/// </summary>
public record ConsultationCreateDto(
    int DoctorId,
    int PatientId,
    DateOnly Date,
    TimeOnly Hour,
    string? Reason
);

/// <summary>
/// DTO pour mettre à jour une consultation
/// PUT /api/consultations/{id}
/// </summary>
public record ConsultationUpdateDto(
    int DoctorId,
    int PatientId,
    DateOnly Date,
    TimeOnly Hour,
    string? Reason
);