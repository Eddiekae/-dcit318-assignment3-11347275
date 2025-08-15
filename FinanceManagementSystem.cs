using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    // a) Generic Repository for Entity Management
    public class Repository<T>
    {
        private readonly List<T> _items = new();
        
        public void Add(T item) => _items.Add(item);
        
        public List<T> GetAll() => new(_items);
        
        public T? GetById(Func<T, bool> predicate) => _items.FirstOrDefault(predicate);
        
        public bool Remove(Func<T, bool> predicate)
        {
            var item = _items.FirstOrDefault(predicate);
            if (item != null)
            {
                _items.Remove(item);
                return true;
            }
            return false;
        }
    }

    // b) Patient class
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString() => 
            $"[Patient] ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
    }

    // c) Prescription class
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString() => 
            $"[Prescription] ID: {Id}, PatientID: {PatientId}, Medication: {MedicationName}, Date: {DateIssued:d}";
    }

    // Main Healthcare System Application
    public class Program
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new();

        // Seed sample data
        public void SeedData()
        {
            // Add patients
            _patientRepo.Add(new Patient(1, "John Doe", 34, "Male"));
            _patientRepo.Add(new Patient(2, "Jane Smith", 28, "Female"));
            _patientRepo.Add(new Patient(3, "Kwame Mensah", 45, "Male"));

            // Add prescriptions
            _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Today.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Paracetamol", DateTime.Today.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Ibuprofen", DateTime.Today.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Metformin", DateTime.Today.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(5, 2, "Lisinopril", DateTime.Today));
        }

        // Build dictionary mapping PatientId -> prescriptions
        public void BuildPrescriptionMap()
        {
            _prescriptionMap = _prescriptionRepo
                .GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("\n--- Patients ---");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.ContainsKey(patientId) 
                ? _prescriptionMap[patientId] 
                : new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            var prescriptions = GetPrescriptionsByPatientId(patientId);

            if (prescriptions.Count == 0)
            {
                Console.WriteLine($"No prescriptions found for patient ID {patientId}.");
                return;
            }

            Console.WriteLine($"\n--- Prescriptions for Patient ID {patientId} ---");
            foreach (var pres in prescriptions)
            {
                Console.WriteLine(pres);
            }
        }

        public void Run()
        {
            Console.WriteLine("Healthcare System - Patient Records & Prescriptions");
            Console.WriteLine("==================================================");
            
            SeedData();
            BuildPrescriptionMap();
            
            PrintAllPatients();
            
            // Display prescriptions for patient ID 1
            PrintPrescriptionsForPatient(1);
            
            // Interactive mode
            Console.Write("\nEnter Patient ID to view prescriptions (or 0 to exit): ");
            while (int.TryParse(Console.ReadLine(), out int patientId) && patientId != 0)
            {
                PrintPrescriptionsForPatient(patientId);
                Console.Write("\nEnter another Patient ID (or 0 to exit): ");
            }
            
            Console.WriteLine("Healthcare system closed.");
        }

        public static void Main(string[] args)
        {
            new Program().Run();
        }
    }
}
