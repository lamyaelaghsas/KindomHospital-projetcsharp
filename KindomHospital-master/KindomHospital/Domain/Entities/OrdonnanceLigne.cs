namespace KingdomHospital.Domain.Entities
{
    public class OrdonnanceLigne
    {
        public int Id { get; set; }
        public int OrdonnanceId { get; set; }
        public int MedicamentId { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? Instructions { get; set; }

        // Navigation properties
        public Ordonnance? Ordonnance { get; set; } 
        public Medicament? Medicament { get; set; } 
    }
}