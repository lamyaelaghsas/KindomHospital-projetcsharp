namespace KingdomHospital.Application.DTOs;

/// <summary>
/// DTO complet pour un médecin avec sa spécialité
/// GET /api/doctors/{id}
/// </summary>
public record DoctorDto(
    int Id,
    string FirstName,
    string LastName,
    int SpecialtyId,
    string SpecialtyName
);

/// <summary>
/// DTO simple pour lister les médecins
/// GET /api/doctors
/// </summary>
public record DoctorListDto(
    int Id,
    string FirstName,
    string LastName,
    string SpecialtyName
);

/// <summary>
/// DTO pour créer un médecin
/// POST /api/doctors
/// </summary>
public record DoctorCreateDto(
    string FirstName,
    string LastName,
    int SpecialtyId
);

/// <summary>
/// DTO pour mettre à jour un médecin
/// PUT /api/doctors/{id}
/// </summary>
public record DoctorUpdateDto(
    string FirstName,
    string LastName,
    int SpecialtyId
);