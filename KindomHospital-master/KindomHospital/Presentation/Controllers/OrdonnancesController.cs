using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingdomHospital.Presentation.Controllers;

/// <summary>
/// Contrôleur pour gérer les ordonnances et leurs lignes
/// Selon le cours: CRUD complet (slide 167-173)
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class OrdonnancesController(
    OrdonnanceService service,
    OrdonnanceLigneService ligneService) : ControllerBase
{

    /// <summary>
    /// GET /api/ordonnances/{id} - Détail d'une ordonnance
    /// Codes HTTP: 200 OK, 404 Not Found
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrdonnanceDto>> GetById(int id)
    {
        var ordonnance = await service.GetByIdAsync(id);

        if (ordonnance == null)
            return NotFound(new { message = $"Ordonnance avec l'ID {id} introuvable" });

        return Ok(ordonnance);
    }

    /// <summary>
    /// POST /api/ordonnances - Crée une ordonnance avec ses lignes
    /// Codes HTTP: 201 Created, 400 Bad Request
    /// Selon le cours: CreatedAtAction() pour 201 (slide 171)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrdonnanceDto>> Create([FromBody] OrdonnanceCreateDto dto)
    {
        var (success, error, ordonnance) = await service.CreateAsync(dto);

        if (!success)
            return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetById), new { id = ordonnance!.Id }, ordonnance);
    }

    /// <summary>
    /// PUT /api/ordonnances/{id} - Met à jour une ordonnance (sans les lignes)
    /// Codes HTTP: 204 No Content, 400 Bad Request, 404 Not Found
    /// Selon le cours: NoContent() pour 204 (slide 172)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] OrdonnanceUpdateDto dto)
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
    /// DELETE /api/ordonnances/{id} - Supprime une ordonnance
    /// Codes HTTP: 204 No Content, 404 Not Found
    /// Selon le cours: Delete en cascade (slide 173)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await service.DeleteAsync(id);

        if (!success)
            return NotFound(new { message = error });

        return NoContent();
    }

    // ===== ENDPOINTS POUR LES LIGNES D'ORDONNANCE =====

    /// <summary>
    /// GET /api/ordonnances/{id}/lignes - Liste toutes les lignes d'une ordonnance
    /// Code HTTP: 200 OK
    /// </summary>
    [HttpGet("{id}/lignes")]
    public async Task<ActionResult<List<OrdonnanceLigneDto>>> GetLignes(int id)
    {
        var lignes = await ligneService.GetByOrdonnanceIdAsync(id);
        return Ok(lignes);
    }

    /// <summary>
    /// GET /api/ordonnances/{id}/lignes/{ligneId} - Détail d'une ligne d'ordonnance
    /// Codes HTTP: 200 OK, 404 Not Found
    /// </summary>
    [HttpGet("{id}/lignes/{ligneId}")]
    public async Task<ActionResult<OrdonnanceLigneDto>> GetLigneById(int id, int ligneId)
    {
        var ligne = await ligneService.GetByIdAsync(id, ligneId);

        if (ligne == null)
            return NotFound(new { message = $"Ligne {ligneId} introuvable dans l'ordonnance {id}" });

        return Ok(ligne);
    }

    /// <summary>
    /// POST /api/ordonnances/{id}/lignes - Ajoute une ou plusieurs lignes
    /// Codes HTTP: 201 Created, 400 Bad Request
    /// </summary>
    [HttpPost("{id}/lignes")]
    public async Task<ActionResult<List<OrdonnanceLigneDto>>> AddLignes(
        int id,
        [FromBody] List<OrdonnanceLigneCreateDto> dtos)
    {
        var (success, error, lignes) = await ligneService.AddLignesAsync(id, dtos);

        if (!success)
            return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetLignes), new { id }, lignes);
    }

    /// <summary>
    /// PUT /api/ordonnances/{id}/lignes/{ligneId} - Met à jour une ligne
    /// Codes HTTP: 204 No Content, 400 Bad Request, 404 Not Found
    /// </summary>
    [HttpPut("{id}/lignes/{ligneId}")]
    public async Task<IActionResult> UpdateLigne(
        int id,
        int ligneId,
        [FromBody] OrdonnanceLigneUpdateDto dto)
    {
        var (success, error) = await ligneService.UpdateAsync(id, ligneId, dto);

        if (!success)
        {
            if (error!.Contains("introuvable"))
                return NotFound(new { message = error });

            return BadRequest(new { message = error });
        }

        return NoContent();
    }

    /// <summary>
    /// DELETE /api/ordonnances/{id}/lignes/{ligneId} - Supprime une ligne
    /// Codes HTTP: 204 No Content, 404 Not Found
    /// </summary>
    [HttpDelete("{id}/lignes/{ligneId}")]
    public async Task<IActionResult> DeleteLigne(int id, int ligneId)
    {
        var (success, error) = await ligneService.DeleteAsync(id, ligneId);

        if (!success)
            return NotFound(new { message = error });

        return NoContent();
    }

    /// <summary>
    /// PUT /api/ordonnances/{id}/consultation/{consultationId}
    /// Rattache une ordonnance à une consultation
    /// </summary>
    [HttpPut("{id}/consultation/{consultationId}")]
    public async Task<IActionResult> AttachToConsultation(int id, int consultationId)
    {
        var (success, error) = await service.AttachToConsultationAsync(id, consultationId);

        if (!success)
            return BadRequest(new { message = error });

        return NoContent();
    }

    /// <summary>
    /// DELETE /api/ordonnances/{id}/consultation
    /// Détache l'ordonnance de sa consultation
    /// </summary>
    [HttpDelete("{id}/consultation")]
    public async Task<IActionResult> DetachFromConsultation(int id)
    {
        var (success, error) = await service.DetachFromConsultationAsync(id);

        if (!success)
            return BadRequest(new { message = error });

        return NoContent();
    }

    /// <summary>
    /// GET /api/ordonnances - Liste toutes les ordonnances
    /// GET /api/ordonnances?doctorId=&patientId=&from=&to= - Liste filtrée
    /// Au moins doctorId ou patientId doit être fourni pour le filtre
    /// Codes HTTP: 200 OK, 400 Bad Request
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<OrdonnanceListDto>>> GetAll(
        [FromQuery] int? doctorId,
        [FromQuery] int? patientId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        // Si des paramètres de filtre sont fournis, utiliser le filtre
        if (doctorId.HasValue || patientId.HasValue || from.HasValue || to.HasValue)
    {
            var (success, error, ordonnances) = await service.GetFilteredAsync(doctorId, patientId, from, to);

            if (!success)
                return BadRequest(new { message = error });

            return Ok(ordonnances);
        }

        // Sinon, retourner toutes les ordonnances
        var allOrdonnances = await service.GetAllAsync();
        return Ok(allOrdonnances);
    }
}