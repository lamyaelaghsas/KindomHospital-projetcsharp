namespace KingdomHospital.Domain.Entities;

public class Consultation
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Hour { get; set; }
    public string? Reason { get; set; }

    // Navigation properties
    public Doctor Doctor { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public ICollection<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();
}