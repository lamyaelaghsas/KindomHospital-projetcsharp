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
    /// <summary>
    /// GET /api/doctors - Liste tous les médecins
    /// Code HTTP: 200 OK
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<DoctorListDto>>> GetAll()
    {
        var doctors = await service.GetAllAsync();
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
        var doctor = await service.GetByIdAsync(id);

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
        var (success, error, doctor) = await service.CreateAsync(dto);

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
        var (success, error) = await service.UpdateAsync(id, dto);

        if (!success)
        {
            // Différencier 404 et 400
            if (error!.Contains("introuvable"))
                return NotFound(new { message = error });

            return BadRequest(new { message = error });
        }

        return NoContent();
    }
}