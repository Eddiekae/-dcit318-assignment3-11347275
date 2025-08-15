using System;

namespace HealthcareSystem
{
    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled,
        NoShow
    }

    public class Appointment
    {
        public int Id { get; }
        public int PatientId { get; }
        public int DoctorId { get; }
        public DateTime AppointmentDate { get; }
        public TimeSpan Duration { get; }
        public string Notes { get; }
        public AppointmentStatus Status { get; set; }

        public Appointment(int id, int patientId, int doctorId, DateTime appointmentDate, 
                          TimeSpan duration, string notes = "")
        {
            Id = id;
            PatientId = patientId;
            DoctorId = doctorId;
            AppointmentDate = appointmentDate;
            Duration = duration;
            Notes = notes;
            Status = AppointmentStatus.Scheduled;
        }

        public override string ToString() => 
            $"[Appointment] ID: {Id}, PatientID: {PatientId}, DoctorID: {DoctorId}, " +
            $"Date: {AppointmentDate:g}, Status: {Status}";
    }
}
