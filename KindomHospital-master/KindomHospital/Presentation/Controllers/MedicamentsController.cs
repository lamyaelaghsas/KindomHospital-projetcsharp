using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingdomHospital.Presentation.Controllers;

/// <summary>
/// Contrôleur pour gérer les médicaments
/// Selon le cours: CRUD partiel (slide 167-171)
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class MedicamentsController(MedicamentService service) : ControllerBase
{
    /// <summary>
    /// GET /api/medicaments - Liste tous les médicaments
    /// Code HTTP: 200 OK
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MedicamentListDto>>> GetAll()
    {
        var medicaments = await service.GetAllAsync();
        return Ok(medicaments);
    }

    /// <summary>
    /// GET /api/medicaments/{id} - Détail d'un médicament
    /// Codes HTTP: 200 OK, 404 Not Found
    /// Selon le cours: NotFound() pour 404 (slide 169)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MedicamentDto>> GetById(int id)
    {
        var medicament = await service.GetByIdAsync(id);

        if (medicament == null)
            return NotFound(new { message = $"Médicament avec l'ID {id} introuvable" });

        return Ok(medicament);
    }

    /// <summary>
    /// POST /api/medicaments - Crée un médicament
    /// Codes HTTP: 201 Created, 400 Bad Request
    /// Selon le cours: CreatedAtAction() pour 201 (slide 171)
    /// Validation: unicité sur (Name, DosageForm, Strength)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MedicamentDto>> Create([FromBody] MedicamentCreateDto dto)
    {
        var (success, error, medicament) = await service.CreateAsync(dto);

        if (!success)
            return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetById), new { id = medicament!.Id }, medicament);
    }

    /// <summary>
    /// GET /api/medicaments/{id}/ordonnances - Liste des ordonnances où le médicament apparaît
    /// Codes HTTP: 200 OK, 404 Not Found
    /// Endpoint relationnel selon le projet (section 10)
    /// </summary>
    [HttpGet("{id}/ordonnances")]
    public async Task<ActionResult<IEnumerable<OrdonnanceListDto>>> GetOrdonnancesByMedicamentId(int id)
    {
        // Vérifier que le médicament existe
        var medicament = await service.GetByIdAsync(id);
        if (medicament == null)
            return NotFound(new { message = $"Médicament avec l'ID {id} introuvable" });

        var ordonnances = await service.GetOrdonnancesByMedicamentIdAsync(id);
        return Ok(ordonnances);
    }

}