using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingdomHospital.Presentation.Controllers;

/// <summary>
/// Contrôleur pour gérer les patients
/// Selon le cours: CRUD complet avec DELETE (slide 167-173)
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class PatientsController(PatientService service) : ControllerBase
{
    /// <summary>
    /// GET /api/patients - Liste tous les patients
    /// Code HTTP: 200 OK
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<PatientListDto>>> GetAll()
    {
        var patients = await service.GetAllAsync();
        return Ok(patients);
    }

    /// <summary>
    /// GET /api/patients/{id} - Détail d'un patient
    /// Codes HTTP: 200 OK, 404 Not Found
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetById(int id)
    {
        var patient = await service.GetByIdAsync(id);

        if (patient == null)
            return NotFound(new { message = $"Patient avec l'ID {id} introuvable" });

        return Ok(patient);
    }

    /// <summary>
    /// POST /api/patients - Crée un patient
    /// Codes HTTP: 201 Created, 400 Bad Request
    /// Selon le cours: CreatedAtAction() pour 201 (slide 171)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PatientDto>> Create([FromBody] PatientCreateDto dto)
    {
        var (success, error, patient) = await service.CreateAsync(dto);

        if (!success)
            return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetById), new { id = patient!.Id }, patient);
    }

    /// <summary>
    /// PUT /api/patients/{id} - Met à jour un patient
    /// Codes HTTP: 204 No Content, 400 Bad Request, 404 Not Found
    /// Selon le cours: NoContent() pour 204 (slide 172)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PatientUpdateDto dto)
    {
        var (success, error) = await service.UpdateAsync(id, dto);

        if (!success)
        {
            if (error!.Contains("introuvable"))
                return NotFound(new { message = error });

            return BadRequest(new { message = error });
        }

        return NoContent();
    }

    /// <summary>
    /// DELETE /api/patients/{id} - Supprime un patient
    /// Codes HTTP: 204 No Content, 400 Bad Request, 404 Not Found
    /// Selon le cours: Delete avec vérification des relations (slide 173)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await service.DeleteAsync(id);

        if (!success)
        {
            if (error!.Contains("introuvable"))
                return NotFound(new { message = error });

            return BadRequest(new { message = error });
        }

        return NoContent();
    }

    /// <summary>
    /// GET /api/patients/{id}/consultations
    /// Liste les consultations d'un patient
    /// </summary>
    [HttpGet("{id}/consultations")]
    public async Task<ActionResult<IEnumerable<ConsultationListDto>>> GetPatientConsultations(int id)
    {
        var patient = await service.GetByIdAsync(id);
        if (patient == null)
            return NotFound($"Patient {id} introuvable");

        var consultations = await service.GetConsultationsByPatientIdAsync(id);
        return Ok(consultations);
    }

    /// <summary>
    /// GET /api/patients/{id}/ordonnances
    /// Liste les ordonnances d'un patient
    /// </summary>
    [HttpGet("{id}/ordonnances")]
    public async Task<ActionResult<IEnumerable<OrdonnanceListDto>>> GetPatientOrdonnances(int id)
    {
        var patient = await service.GetByIdAsync(id);
        if (patient == null)
            return NotFound($"Patient {id} introuvable");

        var ordonnances = await service.GetOrdonnancesByPatientIdAsync(id);
        return Ok(ordonnances);
    }

}