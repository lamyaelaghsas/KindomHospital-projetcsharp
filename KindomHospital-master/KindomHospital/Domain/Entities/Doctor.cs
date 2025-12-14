namespace KingdomHospital.Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }
        public int SpecialtyID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Specialty Specialty { get; set; } = new Specialty();
        public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
        public ICollection<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();
    }
}