namespace KingdomHospital.Domain.Entities;

public class Ordonnance
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int? ConsultationId { get; set; }
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Doctor? Doctor { get; set; } 
    public Patient? Patient { get; set; }  
    public Consultation? Consultation { get; set; }
    public ICollection<OrdonnanceLigne> OrdonnanceLignes { get; set; } = new List<OrdonnanceLigne>();
}