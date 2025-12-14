using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingdomHospital.Presentation.Controllers;

/// <summary>
/// Contrôleur pour gérer les médecins
/// Selon le cours: Routes avec [HttpGet], [HttpPost], [HttpPut] (slide 167)
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class DoctorsController(DoctorService service) : ControllerBase
{
    private readonly DoctorService _service = service;

    /// <summary>
    /// GET /api/doctors - Liste tous les médecins
    /// Code HTTP: 200 OK
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<DoctorListDto>>> GetAll()
    {
        var doctors = await _service.GetAllAsync();
        return Ok(doctors);
    }

    /// <summary>
    /// GET /api/doctors/{id} - Détail d'un médecin
    /// Codes HTTP: 200 OK, 404 Not Found
    /// Selon le cours: NotFound() pour 404 (slide 169)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorDto>> GetById(int id)
    {
        var doctor = await _service.GetByIdAsync(id);

        if (doctor == null)
            return NotFound(new { message = $"Médecin avec l'ID {id} introuvable" });

        return Ok(doctor);
    }

    /// <summary>
    /// POST /api/doctors - Crée un médecin
    /// Codes HTTP: 201 Created, 400 Bad Request
    /// Selon le cours: CreatedAtAction() pour 201 (slide 169)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DoctorDto>> Create([FromBody] DoctorCreateDto dto)
    {
        var (success, error, doctor) = await _service.CreateAsync(dto);

        if (!success)
            return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetById), new { id = doctor!.Id }, doctor);
    }

    /// <summary>
    /// PUT /api/doctors/{id} - Met à jour un médecin
    /// Codes HTTP: 204 No Content, 400 Bad Request, 404 Not Found
    /// Selon le cours: NoContent() pour 204 (slide 169)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DoctorUpdateDto dto)
    {
        var (success, error) = await _service.UpdateAsync(id, dto);

        if (!success)
        {
            // Différencier 404 et 400
            if (error!.Contains("introuvable"))
                return NotFound(new { message = error });

            return BadRequest(new { message = error });
        }

        return NoContent();
    }

    /// <summary>
    /// GET /api/doctors/{id}/specialty
    /// Renvoie la spécialité d'un médecin
    /// </summary>
    [HttpGet("{id}/specialty")]
    public async Task<ActionResult<SpecialtyDto>> GetDoctorSpecialty(int id)
    {
        var specialty = await _service.GetSpecialtyByDoctorIdAsync(id);
        if (specialty == null)
            return NotFound($"Médecin {id} introuvable ou sans spécialité");

        return Ok(specialty);
    }

    /// <summary>
    /// PUT /api/doctors/{id}/specialty/{specialtyId}
    /// Change la spécialité d'un médecin
    /// </summary>
    [HttpPut("{id}/specialty/{specialtyId}")]
    public async Task<IActionResult> UpdateDoctorSpecialty(int id, int specialtyId)
    {
        var (success, error) = await _service.UpdateDoctorSpecialtyAsync(id, specialtyId);

        if (!success)
            return BadRequest(error);

        return NoContent();
    }
    /// <summary>
    /// GET /api/doctors/{id}/consultations?from=&to=
    /// Liste les consultations d'un médecin (avec filtre de dates optionnel)
    /// </summary>
    [HttpGet("{id}/consultations")]
    public async Task<ActionResult<IEnumerable<ConsultationListDto>>> GetDoctorConsultations(
        int id,
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null)
    {
        var doctor = await _service.GetByIdAsync(id);
        if (doctor == null)
            return NotFound($"Médecin {id} introuvable");

        var consultations = await _service.GetConsultationsByDoctorIdAsync(id, from, to);
        return Ok(consultations);
    }

    /// <summary>
    /// GET /api/doctors/{id}/patients
    /// Liste les patients déjà consultés par un médecin
    /// </summary>
    [HttpGet("{id}/patients")]
    public async Task<ActionResult<IEnumerable<PatientListDto>>> GetDoctorPatients(int id)
    {
        var doctor = await _service.GetByIdAsync(id);
        if (doctor == null)
            return NotFound($"Médecin {id} introuvable");

        var patients = await _service.GetPatientsByDoctorIdAsync(id);
        return Ok(patients);
    }

    /// <summary>
    /// GET /api/doctors/{id}/ordonnances?from=&to=
    /// Liste les ordonnances émises par un médecin (avec filtre de dates optionnel)
    /// </summary>
    [HttpGet("{id}/ordonnances")]
    public async Task<ActionResult<IEnumerable<OrdonnanceListDto>>> GetDoctorOrdonnances(
        int id,
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null)
    {
        var doctor = await _service.GetByIdAsync(id);
        if (doctor == null)
            return NotFound($"Médecin {id} introuvable");

        var ordonnances = await _service.GetOrdonnancesByDoctorIdAsync(id, from, to);
        return Ok(ordonnances);
    }

}