using KingdomHospital.Application.DTOs;
using KingdomHospital.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingdomHospital.Presentation.Controllers;

/// <summary>
/// Contrôleur pour gérer les spécialités médicales
/// Selon le cours: Les contrôleurs reçoivent les requêtes HTTP (slide 166-168)
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class SpecialtiesController(SpecialtyService service) : ControllerBase
{
    private readonly SpecialtyService _service = service;

    /// <summary>
    /// GET /api/specialties - Liste toutes les spécialités
    /// Selon le cours: ActionResult<T> combine typage fort et flexibilité (slide 179-180)
    /// Code HTTP 200 OK
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<SpecialtyDto>>> GetAll()
    {
        var specialties = await _service.GetAllAsync();
        return Ok(specialties);
    }

    /// <summary>
    /// GET /api/specialties/{id} - Détail d'une spécialité
    /// Selon le cours: 
    /// - NotFound() retourne 404 (slide 169)
    /// - Ok() retourne 200 (slide 169)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SpecialtyDto>> GetById(int id)
    {
        var specialty = await _service.GetByIdAsync(id);

        if (specialty == null)
            return NotFound(new { message = $"Spécialité avec l'ID {id} introuvable" });

        return Ok(specialty);
    }

    /// <summary>
    /// GET /api/specialties/{id}/doctors
    /// Liste les médecins d'une spécialité
    /// </summary>
    [HttpGet("{id}/doctors")]
    public async Task<ActionResult<IEnumerable<DoctorListDto>>> GetDoctorsBySpecialty(int id)
    {
        var specialty = await _service.GetByIdAsync(id);
        if (specialty == null)
            return NotFound($"Spécialité {id} introuvable");

        var doctors = await _service.GetDoctorsBySpecialtyIdAsync(id);
        return Ok(doctors);
    }
}