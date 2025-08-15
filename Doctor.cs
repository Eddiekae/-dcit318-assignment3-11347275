using System;

namespace HealthcareSystem
{
    public class Doctor
    {
        public int Id { get; }
        public string Name { get; }
        public string Specialization { get; }
        public string LicenseNumber { get; }
        public string Phone { get; }
        public string Email { get; }

        public Doctor(int id, string name, string specialization, string licenseNumber, string phone, string email)
        {
            Id = id;
            Name = name;
            Specialization = specialization;
            LicenseNumber = licenseNumber;
            Phone = phone;
            Email = email;
        }

        public override string ToString() => 
            $"[Doctor] ID: {Id}, Name: {Name}, Specialization: {Specialization}, License: {LicenseNumber}";
    }
}
