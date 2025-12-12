namespace KingdomHospital.Domain.Entities;

public class Medicament
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DosageForm { get; set; } = string.Empty;
    public string Strength { get; set; } = string.Empty;
    public string? AtcCode { get; set; }

    // Navigation property
    public ICollection<OrdonnanceLigne> OrdonnanceLignes { get; set; } = new List<OrdonnanceLigne>();
}