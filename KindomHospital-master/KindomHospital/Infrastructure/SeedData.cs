using KingdomHospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace KingdomHospital.Infrastructure;

/// <summary>
/// Classe responsable de l'initialisation des données de démonstration
/// Selon le cours: Seed Data permet d'avoir des données par défaut
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Initialise toutes les données de la base
    /// </summary>
    public static void Initialize(KingdomHospitalContext context)
    {
        // S'assurer que la base est créée
        context.Database.EnsureCreated();

        // Seed dans l'ordre des dépendances
        SeedSpecialties(context);
        SeedMedicaments(context);
        SeedDoctors(context);
        SeedPatients(context);
        SeedConsultations(context);
        SeedOrdonnances(context);
    }

    /// <summary>
    /// Charge les spécialités depuis le fichier CSV
    /// </summary>
    private static void SeedSpecialties(KingdomHospitalContext context)
    {
        if (context.Specialties.Any()) // verifie si des spécialités existent déjà
            return;

        // construit le chemin vers le fichier CSV
        var csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infrastructure", "Data", "specialties.csv");

        if (!File.Exists(csvPath))
        {
            Console.WriteLine($"ATTENTION: Fichier {csvPath} non trouvé !");
            return;
        }

        var lines = File.ReadAllLines(csvPath).Skip(1); //Lit toutes les lignes du CSV et saute la première ligne (en-tête)
        var specialties = new List<Specialty>();

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length >= 2) 
            {
                specialties.Add(new Specialty
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1].Trim()
                });
            }
        }

        context.Specialties.AddRange(specialties); // Ajoute les spécialités à la base de données
        context.SaveChanges();
        Console.WriteLine($"{specialties.Count} spécialités chargées");
    }

    /// <summary>
    /// Charge les médicaments depuis le fichier CSV
    /// </summary>
    private static void SeedMedicaments(KingdomHospitalContext context)
    {
        if (context.Medicaments.Any())
            return; // Déjà initialisé

        var csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infrastructure", "Data", "medicaments.csv");

        if (!File.Exists(csvPath))
        {
            Console.WriteLine($"ATTENTION: Fichier {csvPath} non trouvé !");
            return;
        }

        var lines = File.ReadAllLines(csvPath).Skip(1); // Skip header
        var medicaments = new List<Medicament>();

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length >= 5)
            {
                medicaments.Add(new Medicament
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1].Trim(),
                    DosageForm = parts[2].Trim(),
                    Strength = parts[3].Trim(),
                    AtcCode = parts[4].Trim()
                });
            }
        }

        context.Medicaments.AddRange(medicaments);
        context.SaveChanges();
        Console.WriteLine($"{medicaments.Count} médicaments chargés");
    }

    /// <summary>
    /// Crée les médecins de démonstration
    /// </summary>
    private static void SeedDoctors(KingdomHospitalContext context)
    {
        if (context.Doctors.Any())
            return; // Déjà initialisé

        var doctors = new List<Doctor>
    {
        new() { FirstName = "Jean", LastName = "Dupont", SpecialtyID = 10 }, // Cardiologie
        new() { FirstName = "Marie", LastName = "Martin", SpecialtyID = 21 }, // Pédiatrie
        new() { FirstName = "Pierre", LastName = "Bernard", SpecialtyID = 16 }, // Neurologie
        new() { FirstName = "Sophie", LastName = "Dubois", SpecialtyID = 25 }, // Dermatologie
        new() { FirstName = "Luc", LastName = "Thomas", SpecialtyID = 12 }, // Gastro-entérologie
        new() { FirstName = "Emma", LastName = "Robert", SpecialtyID = 23 }, // Ophtalmologie
    };

        context.Doctors.AddRange(doctors);
        context.SaveChanges();
        Console.WriteLine($"{doctors.Count} médecins créés");
    }

    /// <summary>
    /// Crée les patients de démonstration
    /// </summary>
    private static void SeedPatients(KingdomHospitalContext context)
    {
        if (context.Patients.Any())
            return; // Déjà initialisé

        var patients = new List<Patient>
        {
            new() { FirstName = "Paul", LastName = "Leroy", BirthDate = new DateOnly(1985, 3, 15) },
            new() { FirstName = "Claire", LastName = "Moreau", BirthDate = new DateOnly(1992, 7, 22) },
            new() { FirstName = "Jacques", LastName = "Simon", BirthDate = new DateOnly(1978, 11, 8) },
            new() { FirstName = "Nathalie", LastName = "Laurent", BirthDate = new DateOnly(1965, 5, 30) },
            new() { FirstName = "Antoine", LastName = "Lefebvre", BirthDate = new DateOnly(2010, 1, 12) },
        };

        context.Patients.AddRange(patients);
        context.SaveChanges();
        Console.WriteLine($"{patients.Count} patients créés");
    }

    /// <summary>
    /// Crée les consultations de démonstration
    /// </summary>
    private static void SeedConsultations(KingdomHospitalContext context)
    {
        if (context.Consultations.Any())
            return; // Déjà initialisé

        var today = DateOnly.FromDateTime(DateTime.Today);

        var consultations = new List<Consultation>
        {
            // Patient 1 - 2 consultations
            new() { DoctorId = 1, PatientId = 1, Date = today.AddDays(-10), Hour = new TimeOnly(9, 0), Reason = "Douleurs thoraciques" },
            new() { DoctorId = 3, PatientId = 1, Date = today.AddDays(-5), Hour = new TimeOnly(14, 30), Reason = "Suivi neurologique" },
            
            // Patient 2 - 3 consultations
            new() { DoctorId = 2, PatientId = 2, Date = today.AddDays(-15), Hour = new TimeOnly(10, 0), Reason = "Vaccination" },
            new() { DoctorId = 4, PatientId = 2, Date = today.AddDays(-8), Hour = new TimeOnly(11, 0), Reason = "Problème de peau" },
            new() { DoctorId = 1, PatientId = 2, Date = today.AddDays(-2), Hour = new TimeOnly(15, 0), Reason = "Contrôle cardiaque" },
            
            // Patient 3 - 1 consultation
            new() { DoctorId = 5, PatientId = 3, Date = today.AddDays(-7), Hour = new TimeOnly(9, 30), Reason = "Troubles digestifs" },
            
            // Patient 4 - 3 consultations
            new() { DoctorId = 6, PatientId = 4, Date = today.AddDays(-12), Hour = new TimeOnly(16, 0), Reason = "Examen de la vue" },
            new() { DoctorId = 1, PatientId = 4, Date = today.AddDays(-6), Hour = new TimeOnly(10, 30), Reason = "Hypertension" },
            new() { DoctorId = 3, PatientId = 4, Date = today.AddDays(-3), Hour = new TimeOnly(13, 0), Reason = "Maux de tête récurrents" },
            
            // Patient 5 - 1 consultation
            new() { DoctorId = 2, PatientId = 5, Date = today.AddDays(-4), Hour = new TimeOnly(14, 0), Reason = "Contrôle pédiatrique" },
            
            // Même médecin, même jour, heures différentes (règle métier)
            new() { DoctorId = 1, PatientId = 1, Date = today.AddDays(-1), Hour = new TimeOnly(9, 0), Reason = "Suivi cardiologique" },
            new() { DoctorId = 1, PatientId = 3, Date = today.AddDays(-1), Hour = new TimeOnly(10, 30), Reason = "Consultation cardiaque" },
        };

        context.Consultations.AddRange(consultations);
        context.SaveChanges();
        Console.WriteLine($"{consultations.Count} consultations créées");
    }

    /// <summary>
    /// Crée les ordonnances et lignes d'ordonnance
    /// </summary>
    private static void SeedOrdonnances(KingdomHospitalContext context)
    {
        if (context.Ordonnances.Any())
            return; // Déjà initialisé

        var today = DateOnly.FromDateTime(DateTime.Today);

        // Ordonnance 1 - Patient 1, consultation cardiologie, 2 médicaments
        var ord1 = new Ordonnance
        {
            DoctorId = 1,
            PatientId = 1,
            ConsultationId = 1,
            Date = today.AddDays(-10),
            Notes = "Traitement pour hypertension"
        };

        ord1.OrdonnanceLignes.Add(new OrdonnanceLigne
        {
            MedicamentId = 8, // Losartan
            Dosage = "50mg",
            Frequency = "1 fois par jour",
            Duration = "30 jours",
            Quantity = 30,
            Instructions = "À prendre le matin"
        });

        ord1.OrdonnanceLignes.Add(new OrdonnanceLigne
        {
            MedicamentId = 9, // Amlodipine
            Dosage = "5mg",
            Frequency = "1 fois par jour",
            Duration = "30 jours",
            Quantity = 30,
            Instructions = "À prendre le soir"
        });

        // Ordonnance 2 - Patient 2, sans consultation, 1 médicament
        var ord2 = new Ordonnance
        {
            DoctorId = 4,
            PatientId = 2,
            ConsultationId = 4,
            Date = today.AddDays(-8),
            Notes = "Traitement dermatologique"
        };

        ord2.OrdonnanceLignes.Add(new OrdonnanceLigne
        {
            MedicamentId = 1, // Paracetamol
            Dosage = "500mg",
            Frequency = "3 fois par jour",
            Duration = "5 jours",
            Quantity = 15,
            Instructions = "Après les repas"
        });

        // Ordonnance 3 - Patient 3, 3 médicaments (requis par l'énoncé)
        var ord3 = new Ordonnance
        {
            DoctorId = 5,
            PatientId = 3,
            ConsultationId = 6,
            Date = today.AddDays(-7),
            Notes = "Traitement troubles digestifs"
        };

        ord3.OrdonnanceLignes.Add(new OrdonnanceLigne
        {
            MedicamentId = 5, // Omeprazole
            Dosage = "20mg",
            Frequency = "1 fois par jour",
            Duration = "14 jours",
            Quantity = 14,
            Instructions = "Le matin à jeun"
        });

        ord3.OrdonnanceLignes.Add(new OrdonnanceLigne
        {
            MedicamentId = 1, // Paracetamol
            Dosage = "1000mg",
            Frequency = "2 fois par jour",
            Duration = "7 jours",
            Quantity = 14,
            Instructions = "En cas de douleur"
        });

        ord3.OrdonnanceLignes.Add(new OrdonnanceLigne
        {
            MedicamentId = 3, // Amoxicilline
            Dosage = "500mg",
            Frequency = "3 fois par jour",
            Duration = "7 jours",
            Quantity = 21,
            Instructions = "Pendant les repas"
        });

        // Ordonnance 4 - Patient 4, première ordonnance
        var ord4 = new Ordonnance
        {
            DoctorId = 1,
            PatientId = 4,
            ConsultationId = 8,
            Date = today.AddDays(-6),
            Notes = "Contrôle tension artérielle"
        };

        ord4.OrdonnanceLignes.Add(new OrdonnanceLigne
        {
            MedicamentId = 25, // Ramipril
            Dosage = "5mg",
            Frequency = "1 fois par jour",
            Duration = "30 jours",
            Quantity = 30,
            Instructions = "Le matin"
        });

        // Ordonnance 5 - Patient 4, deuxième ordonnance (patient avec 2 ordonnances)
        var ord5 = new Ordonnance
        {
            DoctorId = 3,
            PatientId = 4,
            ConsultationId = 9,
            Date = today.AddDays(-3),
            Notes = "Traitement céphalées"
        };

        ord5.OrdonnanceLignes.Add(new OrdonnanceLigne
        {
            MedicamentId = 2, // Ibuprofène
            Dosage = "400mg",
            Frequency = "2 fois par jour",
            Duration = "10 jours",
            Quantity = 20,
            Instructions = "Pendant les repas"
        });

        context.Ordonnances.AddRange(new[] { ord1, ord2, ord3, ord4, ord5 });
        context.SaveChanges();
        Console.WriteLine($" 5 ordonnances créées avec lignes");
    }
}
