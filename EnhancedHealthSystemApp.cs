using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    public class EnhancedHealthSystemApp
    {
        private readonly EnhancedRepository<Patient> _patientRepo = new();
        private readonly EnhancedRepository<Prescription> _prescriptionRepo = new();
        private readonly EnhancedRepository<Doctor> _doctorRepo = new();
        private readonly EnhancedRepository<Appointment> _appointmentRepo = new();
        private readonly EnhancedRepository<MedicalRecord> _medicalRecordRepo = new();
        private readonly EnhancedRepository<Billing> _billingRepo = new();

        private Dictionary<int, List<Prescription>> _prescriptionMap = new();
        private Dictionary<int, List<Appointment>> _appointmentMap = new();
        private Dictionary<int, List<MedicalRecord>> _medicalRecordMap = new();
        private Dictionary<int, List<Billing>> _billingMap = new();

        public void SeedData()
        {
            // Seed Doctors
            _doctorRepo.Add(new Doctor(1, "Dr. Smith", "Cardiology", "MD12345", "555-0101", "smith@hospital.com"));
            _doctorRepo.Add(new Doctor(2, "Dr. Johnson", "Pediatrics", "MD67890", "555-0102", "johnson@hospital.com"));
            _doctorRepo.Add(new Doctor(3, "Dr. Williams", "Neurology", "MD54321", "555-0103", "williams@hospital.com"));

            // Seed Patients
            _patientRepo.Add(new Patient(1, "John Doe", 34, "Male"));
            _patientRepo.Add(new Patient(2, "Jane Smith", 28, "Female"));
            _patientRepo.Add(new Patient(3, "Kwame Mensah", 45, "Male"));
            _patientRepo.Add(new Patient(4, "Maria Garcia", 52, "Female"));

            // Seed Prescriptions
            _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Today.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Paracetamol", DateTime.Today.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Ibuprofen", DateTime.Today.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Metformin", DateTime.Today.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(5, 2, "Lisinopril", DateTime.Today));

            // Seed Appointments
            _appointmentRepo.Add(new Appointment(1, 1, 1, DateTime.Today.AddDays(1), TimeSpan.FromMinutes(30), "Follow-up checkup"));
            _appointmentRepo.Add(new Appointment(2, 2, 2, DateTime.Today.AddDays(2), TimeSpan.FromMinutes(45), "Annual physical"));
            _appointmentRepo.Add(new Appointment(3, 3, 3, DateTime.Today.AddDays(3), TimeSpan.FromMinutes(60), "Neurological exam"));

            // Seed Medical Records
            _medicalRecordRepo.Add(new MedicalRecord(1, 1, 1, DateTime.Today.AddDays(-30), "Hypertension", "Medication prescribed", 
                new List<string> { "Headache", "Dizziness" }, "Patient responding well to treatment"));
            _medicalRecordRepo.Add(new MedicalRecord(2, 2, 2, DateTime.Today.AddDays(-15), "Flu", "Rest and fluids", 
                new List<string> { "Fever", "Cough", "Fatigue" }, "Recovery in progress"));

            // Seed Billing
            _billingRepo.Add(new Billing(1, 1, 1, 150.00m, DateTime.Today.AddDays(7), "Consultation fee"));
            _billingRepo.Add(new Billing(2, 2, 2, 200.00m, DateTime.Today.AddDays(14), "Annual physical exam"));

            BuildAllMaps();
        }

        private void BuildAllMaps()
        {
            _prescriptionMap = _prescriptionRepo
                .GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());

            _appointmentMap = _appointmentRepo
                .GetAll()
                .GroupBy(a => a.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());

            _medicalRecordMap = _medicalRecordRepo
                .GetAll()
                .GroupBy(m => m.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());

            _billingMap = _billingRepo
                .GetAll()
                .GroupBy(b => b.PatientId)
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

        public void PrintAllDoctors()
        {
            Console.WriteLine("\n--- Doctors ---");
            foreach (var doctor in _doctorRepo.GetAll())
            {
                Console.WriteLine(doctor);
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.ContainsKey(patientId) ? _prescriptionMap[patientId] : new List<Prescription>();
        }

        public List<Appointment> GetAppointmentsByPatientId(int patientId)
        {
            return _appointmentMap.ContainsKey(patientId) ? _appointmentMap[patientId] : new List<Appointment>();
        }

        public List<MedicalRecord> GetMedicalRecordsByPatientId(int patientId)
        {
            return _medicalRecordMap.ContainsKey(patientId) ? _medicalRecordMap[patientId] : new List<MedicalRecord>();
        }

        public List<Billing> GetBillingByPatientId(int patientId)
        {
            return _billingMap.ContainsKey(patientId) ? _billingMap[patientId] : new List<Billing>();
        }

        public void PrintPatientSummary(int patientId)
        {
            var patient = _patientRepo.GetById(p => p.Id == patientId);
            if (patient == null)
            {
                Console.WriteLine($"Patient with ID {patientId} not found.");
                return;
            }

            Console.WriteLine($"\n--- Patient Summary for {patient.Name} ---");
            Console.WriteLine(patient);

            Console.WriteLine("\nPrescriptions:");
            var prescriptions = GetPrescriptionsByPatientId(patientId);
            if (prescriptions.Count == 0)
                Console.WriteLine("  No prescriptions found.");
            else
                foreach (var pres in prescriptions)
                    Console.WriteLine($"  {pres}");

            Console.WriteLine("\nAppointments:");
            var appointments = GetAppointmentsByPatientId(patientId);
            if (appointments.Count == 0)
                Console.WriteLine("  No appointments found.");
            else
                foreach (var app in appointments)
                    Console.WriteLine($"  {app}");

            Console.WriteLine("\nMedical Records:");
            var records = GetMedicalRecordsByPatientId(patientId);
            if (records.Count == 0)
                Console.WriteLine("  No medical records found.");
            else
                foreach (var record in records)
                    Console.WriteLine($"  {record}");

            Console.WriteLine("\nBilling:");
            var billing = GetBillingByPatientId(patientId);
            if (billing.Count == 0)
                Console.WriteLine("  No billing records found.");
            else
                foreach (var bill in billing)
                    Console.WriteLine($"  {bill}");
        }

        public void Run()
        {
            SeedData();
            
            while (true)
            {
                Console.WriteLine("\n=== Enhanced Healthcare Management System ===");
                Console.WriteLine("1. View All Patients");
                Console.WriteLine("2. View All Doctors");
                Console.WriteLine("3. View Patient Summary");
                Console.WriteLine("4. View Prescriptions for Patient");
                Console.WriteLine("5. View Appointments for Patient");
                Console.WriteLine("6. View Medical Records for Patient");
                Console.WriteLine("7. View Billing for Patient");
                Console.WriteLine("8. Exit");
                Console.Write("Select an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PrintAllPatients();
                        break;
                    case "2":
                        PrintAllDoctors();
                        break;
                    case "3":
                        Console.Write("Enter Patient ID: ");
                        if (int.TryParse(Console.ReadLine(), out int patientId))
                            PrintPatientSummary(patientId);
                        else
                            Console.WriteLine("Invalid Patient ID.");
                        break;
                    case "4":
                        Console.Write("Enter Patient ID: ");
                        if (int.TryParse(Console.ReadLine(), out int presId))
                        {
                            var prescriptions = GetPrescriptionsByPatientId(presId);
                            Console.WriteLine($"\n--- Prescriptions for Patient ID {presId} ---");
                            foreach (var pres in prescriptions)
                                Console.WriteLine(pres);
                        }
                        else
                            Console.WriteLine("Invalid Patient ID.");
                        break;
                    case "5":
                        Console.Write("Enter Patient ID: ");
                        if (int.TryParse(Console.ReadLine(), out int appId))
                        {
                            var appointments = GetAppointmentsByPatientId(appId);
                            Console.WriteLine($"\n--- Appointments for Patient ID {appId} ---");
                            foreach (var app in appointments)
                                Console.WriteLine(app);
                        }
                        else
                            Console.WriteLine("Invalid Patient ID.");
                        break;
                    case "6":
                        Console.Write("Enter Patient ID: ");
                        if (int.TryParse(Console.ReadLine(), out int medId))
                        {
                            var records = GetMedicalRecordsByPatientId(medId);
                            Console.WriteLine($"\n--- Medical Records for Patient ID {medId} ---");
                            foreach (var record in records)
                                Console.WriteLine(record);
                        }
                        else
                            Console.WriteLine("Invalid Patient ID.");
                        break;
                    case "7":
                        Console.Write("Enter Patient ID: ");
                        if (int.TryParse(Console.ReadLine(), out int billId))
                        {
                            var billing = GetBillingByPatientId(billId);
                            Console.WriteLine($"\n--- Billing for Patient ID {billId} ---");
                            foreach (var bill in billing)
                                Console.WriteLine(bill);
                        }
                        else
                            Console.WriteLine("Invalid Patient ID.");
                        break;
                    case "8":
                        Console.WriteLine("Exiting system...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        public static void Main()
        {
            var app = new EnhancedHealthSystemApp();
            app.Run();
        }
    }
}
