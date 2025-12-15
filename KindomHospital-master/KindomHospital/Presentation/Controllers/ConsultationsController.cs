using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingdomHospital.Presentation.Controllers;

/// <summary>
/// Contrôleur pour gérer les consultations
/// Selon le cours: CRUD complet (slide 167-173)
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ConsultationsController(ConsultationService service) : ControllerBase
{
    
    /// <summary>
    /// GET /api/consultations/{id} - Détail d'une consultation
    /// Codes HTTP: 200 OK, 404 Not Found
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ConsultationDto>> GetById(int id)
    {
        var consultation = await service.GetByIdAsync(id);

        if (consultation == null)
            return NotFound(new { message = $"Consultation avec l'ID {id} introuvable" });

        return Ok(consultation);
    }

    /// <summary>
    /// POST /api/consultations - Crée une consultation
    /// Codes HTTP: 201 Created, 400 Bad Request
    /// Selon le cours: CreatedAtAction() pour 201 (slide 171)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ConsultationDto>> Create([FromBody] ConsultationCreateDto dto)
    {
        var (success, error, consultation) = await service.CreateAsync(dto);

        if (!success)
            return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetById), new { id = consultation!.Id }, consultation);
    }

    /// <summary>
    /// PUT /api/consultations/{id} - Met à jour une consultation
    /// Codes HTTP: 204 No Content, 400 Bad Request, 404 Not Found
    /// Selon le cours: NoContent() pour 204 (slide 172)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ConsultationUpdateDto dto)
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
    /// DELETE /api/consultations/{id} - Supprime une consultation
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
    /// GET /api/consultations/{id}/ordonnances
    /// Liste les ordonnances liées à une consultation
    /// </summary>
    [HttpGet("{id}/ordonnances")]
    public async Task<ActionResult<IEnumerable<OrdonnanceListDto>>> GetConsultationOrdonnances(int id)
    {
        var consultation = await service.GetByIdAsync(id);
        if (consultation == null)
            return NotFound($"Consultation {id} introuvable");

        var ordonnances = await service.GetOrdonnancesByConsultationIdAsync(id);
        return Ok(ordonnances);
    }

    /// <summary>
    /// POST /api/consultations/{id}/ordonnances
    /// Crée une ordonnance rattachée à cette consultation
    /// </summary>
    [HttpPost("{id}/ordonnances")]
    public async Task<ActionResult<OrdonnanceDto>> CreateOrdonnanceForConsultation(
        int id,
        [FromBody] OrdonnanceCreateDto dto)
    {
        var consultation = await service.GetByIdAsync(id);
        if (consultation == null)
            return NotFound($"Consultation {id} introuvable");

        // Injecter OrdonnanceService
        var ordonnanceService = HttpContext.RequestServices.GetRequiredService<OrdonnanceService>();

        var (success, error, ordonnance) = await ordonnanceService.CreateForConsultationAsync(id, dto);

        if (!success)
            return BadRequest(new { message = error });

        return CreatedAtAction(
            nameof(OrdonnancesController.GetById),
            "Ordonnances",
            new { id = ordonnance!.Id },
            ordonnance);
    }
    /// <summary>
    /// GET /api/consultations - Liste toutes les consultations
    /// GET /api/consultations?doctorId=&patientId=&from=&to= - Liste filtrée
    /// Au moins doctorId ou patientId doit être fourni pour le filtre
    /// Codes HTTP: 200 OK, 400 Bad Request
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ConsultationListDto>>> GetAll(
        [FromQuery] int? doctorId,
        [FromQuery] int? patientId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        // Si des paramètres de filtre sont fournis, utiliser le filtre
        if (doctorId.HasValue || patientId.HasValue || from.HasValue || to.HasValue)
    {
            var (success, error, consultations) = await service.GetFilteredAsync(doctorId, patientId, from, to);

            if (!success)
                return BadRequest(new { message = error });

            return Ok(consultations);
        }

        // Sinon, retourner toutes les consultations
        var allConsultations = await service.GetAllAsync();
        return Ok(allConsultations);
    }
}